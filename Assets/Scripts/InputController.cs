using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TetrisGameEnums;

public class InputController : MonoBehaviour
{
    public float continousMoveTime = 0.5f;
    public float keyPressedDelay = 0.5f;
	public float swipeThreshold = 150.0f;

    Coroutine continousMoveCorountine = null;
    static bool isInputEnabled = true;

	// Touch input.
	public bool isDraging = false;
	Vector2 swipeStart, swipeDelta;

    // Update is called once per frame
    void Update()
    {
		Vector2 mouse = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
		if (isInputEnabled && GameManager.IsPointInGrid(mouse))
            GetPlayerInput();
    }

    public static void EnableInput()
    {
        isInputEnabled = true;
    }

    public static void DisableInput()
    {
        isInputEnabled = false;
    }

    void GetPlayerInput()
    {
 #if UNITY_STANDALONE || UNITY_EDITOR
		if (Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.D))
        {
            StopCoroutine(continousMoveCorountine);
            continousMoveCorountine = null;
        }

        if (continousMoveCorountine == null && (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))) // Moving Tetromino to left or right.
        {
            bool isLeft = Input.GetKey(KeyCode.A); // If Key "A" is pressed - move left, otherwise Key "D" is pressed - move right.
            continousMoveCorountine = StartCoroutine(ContinousMove(isLeft));
        }

        if (Input.GetKeyDown(KeyCode.W)) // Rotating Tetromino.
            TetrisGame.GetInstance().RotateTetromino();
        else if (Input.GetKeyDown(KeyCode.S)) // Moving Tetromino to down.
            TetrisGame.GetInstance().DownKeyPressed();

		// Mouse Input.
		if (Input.GetMouseButtonDown(0))
		{
			swipeStart = Input.mousePosition;
		}
		else if (Input.GetMouseButtonUp(0))
		{
			if (!isDraging)
				TetrisGame.GetInstance().RotateTetromino();
			else
				isDraging = false;
		}
		if (Input.GetMouseButton(0))
		{
			// Calculate the distance.
			swipeDelta = (Vector2)Input.mousePosition - swipeStart;

			// Did we cross the threshold?
			if (swipeDelta.magnitude > swipeThreshold)
			{
				isDraging = true;
				// Which direction?
				if (Mathf.Abs(swipeDelta.x) > Mathf.Abs(swipeDelta.y))
				{
					// Left or Right.
					if (swipeDelta.x > 0)
						TetrisGame.GetInstance().MoveTetromino(MoveDirection.Right);
					else
						TetrisGame.GetInstance().MoveTetromino(MoveDirection.Left);
				}
				else
				{
					// Up or Down.
					if (swipeDelta.y < 0)
						TetrisGame.GetInstance().DownKeyPressed();
				}

				swipeStart += swipeDelta;
				swipeDelta = Vector2.zero;
			}
		}
#endif
		if (Input.touches.Length > 0)
		{
			if (Input.touches.Length > 0)
			{
				if (Input.touches[0].phase == TouchPhase.Began)
				{
					swipeStart = Input.touches[0].position;
				}
				else if (Input.touches[0].phase == TouchPhase.Ended || Input.touches[0].phase == TouchPhase.Canceled)
				{
					if (!isDraging)
						TetrisGame.GetInstance().RotateTetromino();
					else
						isDraging = false;
				}
			}
			// Calculate the distance.
			swipeDelta = Input.touches[0].position - swipeStart;
			
			// Did we cross the threshold?
			if (swipeDelta.magnitude > swipeThreshold)
			{
				isDraging = true;
				// Which direction?
				if (Mathf.Abs(swipeDelta.x) > Mathf.Abs(swipeDelta.y))
				{
					// Left or Right.
					if (swipeDelta.x > 0)
						TetrisGame.GetInstance().MoveTetromino(MoveDirection.Right);
					else
						TetrisGame.GetInstance().MoveTetromino(MoveDirection.Left);
				}
				else
				{
					// Up or Down.
					if (swipeDelta.y < 0)
						TetrisGame.GetInstance().DownKeyPressed();
				}

				swipeStart += swipeDelta;
				swipeDelta = Vector2.zero;
			}
		}
	}

	IEnumerator ContinousMove(bool isLeft)
    {
        while (true)
        {
            TetrisGame.GetInstance().MoveTetromino(isLeft ? MoveDirection.Left : MoveDirection.Right);

            yield return new WaitForSeconds(continousMoveTime);
        }
    }
}
