using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TetrominoGenerator : MonoBehaviour
{
	public Tetromino[] tetrominoPrefabs;
	public float minPosX = -2.5f, maxPosX = 2.5f;
	public float spawnRate = 0.1f, tetrominoDestroyHeight = -6.0f;

	List<Tetromino> spawnedTetrominos = new List<Tetromino>();
	List<Tetromino> cachedTetrominos = new List<Tetromino>();

	// Start is called before the first frame update
	void Start()
    {
		StartCoroutine(SpawnTetrominoCoroutine());
	}

	void SpawnTetromino()
	{
		Tetromino tetromino;
		Vector2 pos = new Vector2(Random.Range(minPosX, maxPosX), 6.0f);
		float rotZ = Random.Range(-90.0f, 90.0f);

		if (cachedTetrominos.Count == 0)
		{
			int index = Random.Range(0, tetrominoPrefabs.Length);

			tetromino = Instantiate(tetrominoPrefabs[index], transform);
			GameObject[] minos = tetromino.minos;
			foreach (GameObject mino in minos)
				mino.AddComponent<BoxCollider2D>();

			tetromino.gameObject.AddComponent<Rigidbody2D>();
			tetromino.transform.localScale = new Vector3(0.4f, 0.4f);
		}
		else
		{
			tetromino = cachedTetrominos[0];
			cachedTetrominos.RemoveAt(0);
		}

		tetromino.transform.position = pos;
		tetromino.transform.rotation = Quaternion.Euler(0.0f, 0.0f, rotZ);
		tetromino.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
		spawnedTetrominos.Add(tetromino);
	}

	void CleanUpTetrominos()
	{
		for(int i = 0; i < spawnedTetrominos.Count; i++)
		{
			Tetromino tetromino = spawnedTetrominos[i];
			if (tetromino.transform.position.y <= tetrominoDestroyHeight)
			{
				cachedTetrominos.Add(tetromino);
				spawnedTetrominos.RemoveAt(i--);
			}
		}
	}

	IEnumerator SpawnTetrominoCoroutine()
	{
		while (true)
		{
			CleanUpTetrominos();
			SpawnTetromino();

			yield return new WaitForSeconds(spawnRate);
		}
	}
}
