using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PictureHandler : MonoBehaviour
{
    [Tooltip("A texture to split on the tiles.")]
    public Texture2D tileTexture;

    void Start()
    {
        tileTexture = GameObject.Find("Board").GetComponent<LoadBoard>().tileTexture;
        gameObject.GetComponent<Renderer>().material.SetTexture("_MainTex", tileTexture);
    }

    void Update()
    {
        
    }
}
