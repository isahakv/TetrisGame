using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TetrisGameEnums;

namespace TetrisGameEnums
{    public enum MoveDirection
    {
        Left,
        Right,
        Down
    }

    public enum CollisionTarget
    {
        None,
        GridLeft,
        GridRight,
        GridDown,
        Mino
    }
}

public class TetrisGame : MonoBehaviour
{
    private static TetrisGame instance;

    private AudioSource audioSource;
    private List<GameObject> spawnedTetrominos = new List<GameObject>();
    private GameObject currentTetromino, nextTetromino;
    public GameGrid gameGrid;
    
    Coroutine tetrominoMoveDownCoroutine = null;
    bool isDownKeyPressed = false;
    
    public float moveDownTime = 1.0f, moveDownPressedTime = 0.05f;
    public GameObject[] tetrominoPrefabs;

    public AudioClip gameOverSound, lineClearSingleSound, lineClearDoubleSound, lineClearTripleSound;

    void Awake()
    {
        instance = this;
        audioSource = GetComponent<AudioSource>();
    }

    public Tetromino GetCurrTetromino() { return currentTetromino.GetComponent<Tetromino>(); }

    IEnumerator TetrominoMoveDown()
    {
        while (true)
        {
            if (isDownKeyPressed)
                yield return new WaitForSeconds(moveDownPressedTime);
            else
                yield return new WaitForSeconds(moveDownTime);

            MoveTetromino(MoveDirection.Down);
        }
    }

    public void DownKeyPressed()
    {
        if (!isDownKeyPressed)
        {
            isDownKeyPressed = true;
            StopCoroutine(tetrominoMoveDownCoroutine);
            tetrominoMoveDownCoroutine = StartCoroutine(TetrominoMoveDown());
        }
    }

    void HandleTetrominoOverlap()
    {
        while (true)
        {
            CollisionTarget overlapedObject = gameGrid.CheckHasOverlapAtPos(Vector2.zero);
            if (overlapedObject == CollisionTarget.None)
                return;

            switch (overlapedObject)
            {
                case CollisionTarget.GridLeft:
                    currentTetromino.transform.position += new Vector3(1.0f, 0.0f, 0.0f);
                    break;
                case CollisionTarget.GridRight:
                    currentTetromino.transform.position += new Vector3(-1.0f, 0.0f, 0.0f);
                    break;
                case CollisionTarget.GridDown:
                case CollisionTarget.Mino:
                    currentTetromino.transform.position += new Vector3(0.0f, 1.0f, 0.0f);
                    break;
            }
        }
    }

    void PlayLineClearedSound(int numOfClearedLines)
    {
        switch (numOfClearedLines)
        {
            case 1:
                audioSource.PlayOneShot(lineClearSingleSound, 10.0f);
                break;
            case 2:
                audioSource.PlayOneShot(lineClearDoubleSound, 10.0f);
                break;
            case 3:
            case 4:
                audioSource.PlayOneShot(lineClearTripleSound, 10.0f);
                break;
        }
    }

    void SpawnNextTetromino()
    {
        int index;
        if (!currentTetromino)
        {
            index = Random.Range(0, tetrominoPrefabs.Length);
            currentTetromino = Instantiate(tetrominoPrefabs[index], gameGrid.GetTetrominoSpawnPos(), Quaternion.identity);
        }
        else
        {
            currentTetromino = nextTetromino;
            currentTetromino.transform.position = gameGrid.GetTetrominoSpawnPos();            
        }
        spawnedTetrominos.Add(currentTetromino);
        gameGrid.SetCurrentTetromin(currentTetromino.GetComponent<Tetromino>());

        index = Random.Range(0, tetrominoPrefabs.Length);
        nextTetromino = Instantiate(tetrominoPrefabs[index], gameGrid.GetTetrominoSpawnPos(false), Quaternion.identity);
        
        HandleTetrominoOverlap();
        gameGrid.UpdateGrid();

        isDownKeyPressed = false;
    }

    public void MoveTetromino(MoveDirection direction)
    {
        Vector2 deltaPos = new Vector2();
        switch (direction)
        {
            case MoveDirection.Left:
                deltaPos = new Vector2(-1.0f, 0.0f);
                break;
            case MoveDirection.Right:
                deltaPos = new Vector2(1.0f, 0.0f);
                break;
            case MoveDirection.Down:
                deltaPos = new Vector2(0.0f, -1.0f);
                break;
        }

        if (gameGrid.CheckHasOverlapAtPos(deltaPos) != CollisionTarget.None)
        {
            // Check if tetromino hits the ground.
            if (direction == MoveDirection.Down)
            {
                SpawnNextTetromino();
                int numOfFullRows = gameGrid.CheckForFullRows();                
                if (numOfFullRows == 0) // If haven't Full Rows then play tetromino land sound.
                    currentTetromino.GetComponent<Tetromino>().OnLanded();
                else                    // Else play line clear sound.
                    PlayLineClearedSound(numOfFullRows);

				// Score player.
				GameManager.GetInstance().AddScore(20 + numOfFullRows * 50); // NOTE: Add dependency for time.

                if (gameGrid.IsTetrominoAboveBoard())
                    GameManager.GetInstance().GameOver();

                ClearEmptyTetrominos(); // After deleting minos there may be empty tetrominos.
            }

            return;
        }

        currentTetromino.GetComponent<Tetromino>().Move(deltaPos);
        gameGrid.UpdateGrid();
    }

    public void RotateTetromino()
    {
        currentTetromino.GetComponent<Tetromino>().Rotate();
        HandleTetrominoOverlap();

        gameGrid.UpdateGrid();
    }

    public void StartNewGame()
    {
        audioSource.Play();

        gameGrid.InitGrid();
        for (int i = 0; i < spawnedTetrominos.Count; i++)
            Destroy(spawnedTetrominos[i]);
        spawnedTetrominos.Clear();

        Destroy(nextTetromino);
        currentTetromino = nextTetromino = null;

        SpawnNextTetromino();
        tetrominoMoveDownCoroutine = StartCoroutine(TetrominoMoveDown());
    }

    public void PauseGame()
    {
        audioSource.Pause();
    }

    public void ResumeGame()
    {
        audioSource.UnPause();
    }

    public void GameOver()
    {
        StopAllCoroutines();

        audioSource.Stop();
        audioSource.PlayOneShot(gameOverSound);
    }

    void ClearEmptyTetrominos()
    {
        for (int i = 0; i < spawnedTetrominos.Count; i++)
        {
            Tetromino tetrominoScript = spawnedTetrominos[i].GetComponent<Tetromino>();
            bool isAllNull = true;
            foreach (GameObject mino in tetrominoScript.minos)
            {
                // If at least one mino is not null, then set isAllNull to false.
                if (mino) 
                    isAllNull = false;
            }

            // If all minos is null, then destroy parent tetromino.
            if (isAllNull)
            {
                Destroy(spawnedTetrominos[i]);
                spawnedTetrominos.RemoveAt(i);
                i--;
            }
        }
    }
    
    public static Vector3 Round(Vector3 vec)
    {
        return new Vector3((int)(vec.x), (int)(vec.y), (int)(vec.z));
    }

    public static TetrisGame GetInstance() { return instance; }
}
