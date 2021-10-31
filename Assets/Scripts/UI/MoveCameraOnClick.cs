using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCameraOnClick : MonoBehaviour
{
    public bool isHover;
    private GameState gameState;

    void Start()
    {
        gameState = GameObject.Find("GameState").GetComponent<GameState>();
    }

    void Update()
    {
        
    }

    void OnMouseEnter()
    {
        isHover = true;
    }

    void OnMouseDown()
    {
        isHover = false;
        gameState.StartNewGame("15Puzzle");
    }

    void OnMouseExit()
    {
        isHover = false;
    }
}
