﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputController : MonoBehaviour
{
    // public float continousMoveTimer = 0.0f;
    public float continousMoveTime = 0.5f;
    public float KeyPressedDelay = 0.5f;

    Coroutine continousMoveCorountine = null;

    // Update is called once per frame
    void Update()
    {
        if (!GameController.GetInstance().IsGameOver())
            GetPlayerInput();
    }

    void GetPlayerInput()
    {
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
        {
            GameController.GetInstance().RotateTetromino();
        }
        else if (Input.GetKeyDown(KeyCode.S)) // Moving Tetromino to down.
        {
            GameController.GetInstance().DownKeyPressed();
        }
    }

    IEnumerator ContinousMove(bool isLeft)
    {
        while (true)
        {
            GameController.GetInstance().MoveTetromino(isLeft ? GameController.MoveDirection.Left : GameController.MoveDirection.Right);

            yield return new WaitForSeconds(continousMoveTime);
        }
    }
}
