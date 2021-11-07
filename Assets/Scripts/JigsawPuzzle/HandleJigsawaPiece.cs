using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Edge
{
    Empty,
    Straight,
    Hook
}

public class HandleJigsawaPiece : MonoBehaviour
{
    public Edge topEdge;
    public Edge rightEdge;
    public Edge bottomEdge;
    public Edge leftEdge;

    public Texture2D texture;

    public int widthWithEdges;
    public int heightWithEdges;
    public int bottomLeftPixelX;
    public int bottomLeftPixelY;

    // Start is called before the first frame update
    void Start()
    {
        //RotatePiece();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetTexture(Texture2D newTexture)
    {
        texture = newTexture;
    }

    public void RotatePiece(int rotation)
    {
        float angle = rotation * 90;

        transform.Rotate(0, 0, angle);

        Edge previousTopEdge = topEdge;
        Edge previousRightEdge = rightEdge;
        Edge previousBottomEdge = bottomEdge;
        Edge previousLeftEdge = leftEdge;

        if (angle == 90)
        {
            topEdge = previousLeftEdge;
            rightEdge = previousTopEdge;
            bottomEdge = previousRightEdge;
            leftEdge = previousBottomEdge;
        } else if (angle == 90 * 2)
        {
            topEdge = previousBottomEdge;
            rightEdge = previousLeftEdge;
            bottomEdge = previousTopEdge;
            leftEdge = previousRightEdge;
        }
        else if (angle == 90 * 3)
        {
            topEdge = previousRightEdge;
            rightEdge = previousBottomEdge;
            bottomEdge = previousLeftEdge;
            leftEdge = previousTopEdge;
        }
    }
}
