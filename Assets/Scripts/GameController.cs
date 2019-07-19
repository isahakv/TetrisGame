using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public enum MoveDirection
    {
        Left,
        Right,
        Down
    }

    enum CollisionTarget
    {
        None,
        BoardLeft,
        BoardRight,
        BoardDown,
        Mino
    }

    private static GameController instance;

    private AudioSource audioSource;
    private List<GameObject> spawnedTetrominos = new List<GameObject>();
    private GameObject currentTetromino, nextTetromino;
    private const int boardWidth = 10, boardHeight = 20;
    private Transform[,] board = new Transform[boardHeight, boardWidth];
    private int score = 0;

    Coroutine tetrominoMoveDownCoroutine = null;
    bool isDownKeyPressed = false;
    bool isGameOver = false;
    
    public float moveDownTime = 1.0f, moveDownPressedTime = 0.05f;
    public GameObject[] tetrominoPrefabs;
    public GameObject gameOver_UIPanel;

    public AudioClip gameOverSound, lineClearSingleSound, lineClearDoubleSound, lineClearTripleSound;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        StartNewGame();
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

    void UpdateBoard()
    {
        Tetromino tetrominoScript = currentTetromino.GetComponent<Tetromino>();
        GameObject[] minos = tetrominoScript.minos;
        for (int y = 0; y < boardHeight; y++)
        {
            for (int x = 0; x < boardWidth; x++)
            {
                if (board[y, x] != null && tetrominoScript.IsParentOf(board[y, x].gameObject))
                    board[y, x] = null;
            }
        }
        
        foreach (GameObject mino in minos)
        {
            Vector3 pos = Round(mino.transform.position);
            // check the height.
            if (pos.y < boardHeight)
                board[(int)pos.y, (int)pos.x] = mino.transform;
        }
    }

    CollisionTarget CheckHasOverlapAtPos(Vector2 deltaPos)
    {
        Vector3 delta = new Vector3(deltaPos.x, deltaPos.y, 0.0f);
        GameObject[] minos = currentTetromino.GetComponent<Tetromino>().minos;
        foreach (GameObject mino in minos)
        {
            Vector3 newPos = mino.transform.position + delta;
            if (newPos.x < 0) // Checking board out of bounce.
                return CollisionTarget.BoardLeft;
            else if (newPos.x >= boardWidth)
                return CollisionTarget.BoardRight;
            else if (newPos.y < 0)
                return CollisionTarget.BoardDown;
            else if ((int)newPos.y < boardHeight && board[(int)newPos.y, (int)newPos.x] != null
                && board[(int)newPos.y, (int)newPos.x].parent != mino.transform.parent) // Check if not null and not the same mino.
                return CollisionTarget.Mino;
        }

        return CollisionTarget.None;
    }

    void HandleTetrominoOverlap()
    {
        while (true)
        {
            CollisionTarget overlapedObject = CheckHasOverlapAtPos(Vector2.zero);
            if (overlapedObject == CollisionTarget.None)
                return;

            switch (overlapedObject)
            {
                case CollisionTarget.BoardLeft:
                    currentTetromino.transform.position += new Vector3(1.0f, 0.0f, 0.0f);
                    break;
                case CollisionTarget.BoardRight:
                    currentTetromino.transform.position += new Vector3(-1.0f, 0.0f, 0.0f);
                    break;
                case CollisionTarget.Mino:
                    currentTetromino.transform.position += new Vector3(0.0f, 1.0f, 0.0f);
                    break;
            }
        }
    }

    bool IsFullRowAt(int y)
    {
        for (int x = 0; x < boardWidth; x++)
        {
            if (board[y, x] == null)
                return false;
        }

        return true;
    }

    void MoveRowsDownFrom(int y)
    {
        Tetromino tetrominoScript = currentTetromino.GetComponent<Tetromino>();
        for (int i = y; i < boardHeight; i++)
        {
            for (int x = 0; x < boardWidth; x++)
            {
                // Checking for null and ignoring current falling tetromino.
                if (board[i, x] == null || tetrominoScript.IsParentOf(board[i, x].gameObject))
                    continue;

                board[i - 1, x] = board[i, x];
                board[i - 1, x].position += new Vector3(0.0f, -1.0f, 0.0f);
                board[i, x] = null;
            }
        }
    }

    void RemoveRow(int y)
    {
        for (int x = 0; x < boardWidth; x++)
        {
            Destroy(board[y, x].gameObject);
            board[y, x] = null;
        }

        MoveRowsDownFrom(y + 1);
    }

    bool CheckForFullRows()
    {
        int numOfFullRows = 0;
        for (int y = 0; y < boardHeight; y++)
        {
            if (IsFullRowAt(y))
            {                
                RemoveRow(y--);
                numOfFullRows++;
            }
        }
        if (numOfFullRows != 0)
        {
            PlayLineClearedSound(numOfFullRows);
            return true;
        }
        return false;
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

    bool IsTetrominoAboveBoard()
    {
        GameObject[] minos = currentTetromino.GetComponent<Tetromino>().minos;
        foreach (GameObject mino in minos)
        {
            if (mino.transform.position.y >= boardHeight)
                return true;
        }

        return false;
    }

    Vector3 GetSpawnPos(bool isCurrentTetromino = true)
    {
        return isCurrentTetromino ? new Vector3(boardWidth / 2, boardHeight, 0) : new Vector3(15.0f, 10.0f, 0.0f);
    }

    void SpawnNextTetromino()
    {
        int index;
        if (!currentTetromino)
        {
            index = Random.Range(0, tetrominoPrefabs.Length);
            currentTetromino = Instantiate(tetrominoPrefabs[index], GetSpawnPos(), Quaternion.identity);
        }
        else
        {
            currentTetromino = nextTetromino;
            currentTetromino.transform.position = GetSpawnPos();            
        }
        spawnedTetrominos.Add(currentTetromino);

        index = Random.Range(0, tetrominoPrefabs.Length);
        nextTetromino = Instantiate(tetrominoPrefabs[index], GetSpawnPos(false), Quaternion.identity);
        
        HandleTetrominoOverlap();
        UpdateBoard();

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

        if (CheckHasOverlapAtPos(deltaPos) != CollisionTarget.None)
        {
            // Check if tetromino hits the ground.
            if (direction == MoveDirection.Down)
            {
                SpawnNextTetromino();
                bool haveFullRows = CheckForFullRows();
                // If haven't Full Rows then play tetromino land sound.
                if (!haveFullRows)
                    currentTetromino.GetComponent<Tetromino>().OnLanded();

                if (IsTetrominoAboveBoard())
                    GameOver();

                ClearEmptyTetrominos(); // After deleting minos there may be empty tetrominos.
            }

            return;
        }

        currentTetromino.GetComponent<Tetromino>().Move(deltaPos);
        UpdateBoard();
    }

    public void RotateTetromino()
    {
        currentTetromino.GetComponent<Tetromino>().Rotate();
        HandleTetrominoOverlap();

        UpdateBoard();
    }

    public void StartNewGame()
    {
        isGameOver = false;
        gameOver_UIPanel.SetActive(false);
        audioSource.Play();

        board = new Transform[boardHeight, boardWidth];
        for (int i = 0; i < spawnedTetrominos.Count; i++)
            Destroy(spawnedTetrominos[i]);
        spawnedTetrominos.Clear();

        Destroy(nextTetromino);
        currentTetromino = nextTetromino = null;

        SpawnNextTetromino();
        tetrominoMoveDownCoroutine = StartCoroutine(TetrominoMoveDown());
    }

    void GameOver()
    {
        StopAllCoroutines();

        isGameOver = true;
        gameOver_UIPanel.SetActive(true);
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

    public bool IsGameOver() { return isGameOver; }

    Vector3 Round(Vector3 vec)
    {
        return new Vector3((int)(vec.x), (int)(vec.y), (int)(vec.z));
    }

    public static GameController GetInstance() { return instance; }
}
