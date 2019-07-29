using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TetrisGameEnums;

public class GameGrid
{
	private int gridWidth = 10, gridHeight = 20;
    private Transform[,] grid;
    private Tetromino currentTetromino;

	public GameGrid(int _gridWith, int _gridHeight)
	{
		gridWidth = _gridWith;
		gridHeight = _gridHeight;
		InitGrid();
	}

    public void InitGrid()
    {
        grid = new Transform[gridHeight, gridWidth];
    }

    public void UpdateGrid()
    {
        GameObject[] minos = currentTetromino.minos;
        for (int y = 0; y < gridHeight; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                if (grid[y, x] != null && currentTetromino.IsParentOf(grid[y, x].gameObject))
                    grid[y, x] = null;
            }
        }

        foreach (GameObject mino in minos)
        {
            Vector3 pos = TetrisGame.Round(mino.transform.position);
            // check the height.
            if (pos.y < gridHeight)
                grid[(int)pos.y, (int)pos.x] = mino.transform;
        }
    }

    public void SetCurrentTetromino(Tetromino currTetromino)
    {
        currentTetromino = currTetromino;
    }

    public CollisionTarget CheckHasOverlapAtPos(Vector2 deltaPos)
    {
        Vector3 delta = new Vector3(deltaPos.x, deltaPos.y, 0.0f);
        GameObject[] minos = currentTetromino.minos;
        foreach (GameObject mino in minos)
        {
            Vector3 newPos = mino.transform.position + delta;
            if (newPos.x < 0) // Checking board out of bounce.
                return CollisionTarget.GridLeft;
            else if (newPos.x >= gridWidth) // Checking board out of bounce.
				return CollisionTarget.GridRight;
            else if (newPos.y < 0) // Checking board out of bounce.
				return CollisionTarget.GridDown;
            else if ((int)newPos.y < gridHeight && grid[(int)newPos.y, (int)newPos.x] != null
                && grid[(int)newPos.y, (int)newPos.x].parent != mino.transform.parent) // Check if not null and not the same mino.
                return CollisionTarget.Mino;
        }

        return CollisionTarget.None;
    }

    bool IsFullRowAt(int y)
    {
        for (int x = 0; x < gridWidth; x++)
        {
            if (grid[y, x] == null)
                return false;
        }

        return true;
    }

    void MoveRowsDownFrom(int y)
    {
        for (int i = y; i < gridHeight; i++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                // Checking for null and ignoring current falling tetromino.
                if (grid[i, x] == null || currentTetromino.IsParentOf(grid[i, x].gameObject))
                    continue;

                grid[i - 1, x] = grid[i, x];
                grid[i - 1, x].position += new Vector3(0.0f, -1.0f, 0.0f);
                grid[i, x] = null;
            }
        }
    }

    void RemoveRow(int y)
    {
        for (int x = 0; x < gridWidth; x++)
        {
			Object.Destroy(grid[y, x].gameObject);
            grid[y, x] = null;
        }

        MoveRowsDownFrom(y + 1);
    }

    /// <summary>
    /// Checks for full rows and deletes if there are any.
    /// </summary>
    /// <returns>Returns num of full rows.</returns>
    public int CheckForFullRows()
    {
        int numOfFullRows = 0;
        for (int y = 0; y < gridHeight; y++)
        {
            if (IsFullRowAt(y))
            {
                RemoveRow(y--);
                numOfFullRows++;
            }
        }
        return numOfFullRows;
    }

    public bool IsTetrominoAboveBoard()
    {
        GameObject[] minos = currentTetromino.minos;
        foreach (GameObject mino in minos)
        {
            if (mino.transform.position.y >= gridHeight)
                return true;
        }

        return false;
    }

    public Vector3 GetTetrominoSpawnPos()
    {
        return new Vector3(gridWidth / 2, gridHeight, 0);
    }
}
