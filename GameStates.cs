using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStates : MonoBehaviour
{
    public enum gameStates
    {
        Pause,
        Play,
        Slow
    }

    public bool holdADS;

    public gameStates gameState;
    private gameStates lastState;

    // Start is called before the first frame update
    void Start()
    {
        gameState = gameStates.Play;
    }

    private void Update()
    {
        if (gameState == gameStates.Pause)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

        switch (gameState)
        {
            case (gameStates.Slow):
                Time.timeScale = 0.5f;
                Time.fixedDeltaTime = Time.timeScale * 0.02f;
                break;

            case (gameStates.Play):
                Time.timeScale = 1f;
                Time.fixedDeltaTime = Time.timeScale / 50;
                break;

            default:
                break;
        }
    }
     
    public void Pause(bool pause)
    {
        if (pause)
        {
            lastState = gameState;
            gameState = gameStates.Pause;
        }
        else
        {
            gameState = lastState;
        }
    }

    public void SlowTime(bool slow)
    {
        if (slow && gameState != gameStates.Slow)
        {
            gameState = gameStates.Slow;
            //reduce gravity
            //Physics.gravity = Physics.gravity / 2;
        }
        else if (!slow && gameState != gameStates.Play)
        {
            gameState = gameStates.Play;
            //incrase gravity
            //Physics.gravity = Physics.gravity * 2;
        }
    }
}
