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
    private GameGrid gameGrid;
    
    Coroutine tetrominoMoveDownCoroutine = null;
    bool isDownKeyPressed = false;

	public int gridWidth = 10, gridHeight = 20; // NOTE: Make this changable.
	public float moveDownTime = 1.0f, moveDownPressedTime = 0.05f;
	public Vector2 nextTetrominoSpawnPos = new Vector3(14.0f, 11.0f, 0.0f);

	public GameObject[] tetrominoPrefabs;

    public AudioClip gameOverSound, lineClearSingleSound, lineClearDoubleSound, lineClearTripleSound;

    void Awake()
    {
        instance = this;
        audioSource = GetComponent<AudioSource>();
		gameGrid = new GameGrid(gridWidth, gridHeight);
    }

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
			{
				gameGrid.UpdateGrid();
				return;
			}

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
			currentTetromino.transform.localScale = new Vector3(1.0f, 1.0f);
		}
        spawnedTetrominos.Add(currentTetromino);
        gameGrid.SetCurrentTetromino(currentTetromino.GetComponent<Tetromino>());

        index = Random.Range(0, tetrominoPrefabs.Length);
        nextTetromino = Instantiate(tetrominoPrefabs[index], nextTetrominoSpawnPos, Quaternion.identity);
		nextTetromino.transform.localScale = new Vector3(0.5f, 0.5f);

		HandleTetrominoOverlap();

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
				{
					PlayLineClearedSound(numOfFullRows);
					// After deleting minos there may be empty tetrominos.
					ClearEmptyTetrominos();
				}

				// Score player.
				GameManager.GetInstance().AddScore(20 + numOfFullRows * 50); // NOTE: Add dependency for time.

                if (gameGrid.IsTetrominoAboveBoard())
                    GameManager.GetInstance().GameOver();
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
        audioSource.PlayOneShot(gameOverSound, 3.0f);
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
