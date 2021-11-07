using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_SelectGame : MonoBehaviour
{
    public LaunchGame launcher;
    public GameObject gameToLaunch;

    void OnMouseDown()
    {
        this.launcher.StartNewGame(gameToLaunch.name);
    }
}
