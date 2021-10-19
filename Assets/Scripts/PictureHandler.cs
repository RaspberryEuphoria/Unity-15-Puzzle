using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PictureHandler : MonoBehaviour
{
    private GameState gameState;
    private Texture2D pictureTexture;

    private void Start()
    {
        gameState = GameObject.Find("GameState").GetComponent<GameState>();
    }

    void Update()
    {
        if (!gameState.isReady)
        {
            return;
        }

        if (pictureTexture == null)
        {
            pictureTexture = GameObject.Find("Board").GetComponent<LoadBoard>().tileTexture;
            gameObject.GetComponent<Renderer>().material.SetTexture("_MainTex", pictureTexture);
        }
    }
}
