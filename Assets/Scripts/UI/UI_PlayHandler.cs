using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_PlayHandler : MonoBehaviour
{
    public LaunchGame launcher;

    void OnMouseDown()
    {
        launcher.StartNewGame("15Puzzle");
    }
}
