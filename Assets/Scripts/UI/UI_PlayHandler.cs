using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_PlayHandler : MonoBehaviour
{
    public bool isHover;
    public LaunchGame launcher;

    void Start()
    {
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
        launcher.StartNewGame("15Puzzle");
    }

    void OnMouseExit()
    {
        isHover = false;
    }
}
