using UnityEngine;

public enum Edge
{
    Empty,
    Straight,
    Hook
}

[System.Serializable]
public class Edges
{
    public Edge Top;
    public Edge Right;
    public Edge Bottom;
    public Edge Left;

    public void Init(Edge top, Edge right, Edge bottom, Edge left)
    {
        this.Top = top;
        this.Right = right;
        this.Bottom = bottom;
        this.Left = left;
    }

    public void Rotate(float angle)
    {
        Edge previousTopEdge = this.Top;
        Edge previousRightEdge = this.Right;
        Edge previousBottomEdge = this.Bottom;
        Edge previousLeftEdge = this.Left;

        if (angle == 90)
        {
            this.Top = previousLeftEdge;
            this.Right = previousTopEdge;
            this.Bottom = previousRightEdge;
            this.Left = previousBottomEdge;
        }
        else if (angle == 90 * 2)
        {
            this.Top = previousBottomEdge;
            this.Right = previousLeftEdge;
            this.Bottom = previousTopEdge;
            this.Left = previousRightEdge;
        }
        else if (angle == 90 * 3)
        {
            this.Top = previousRightEdge;
            this.Right = previousBottomEdge;
            this.Bottom = previousLeftEdge;
            this.Left = previousTopEdge;
        }
    }
}

public class JigsawPiece : MonoBehaviour
{
    public Edges edges;

    public void RotatePiece(int rotation)
    {
        float angle = rotation * 90;

        this.transform.Rotate(0, 0, angle);
        this.edges.Rotate(angle);
    }
}
