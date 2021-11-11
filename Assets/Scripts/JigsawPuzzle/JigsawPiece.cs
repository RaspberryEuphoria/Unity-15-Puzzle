﻿using UnityEngine;

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

    public void Init(Edge Top, Edge Right, Edge Bottom, Edge Left)
    {
        this.Top = Top;
        this.Right = Right;
        this.Bottom = Bottom;
        this.Left = Left;
    }

    public void Rotate(float angle)
    {
        Edge previousTopEdge = Top;
        Edge previousRightEdge = Right;
        Edge previousBottomEdge = Bottom;
        Edge previousLeftEdge = Left;

        if (angle == 90)
        {
            Top = previousLeftEdge;
            Right = previousTopEdge;
            Bottom = previousRightEdge;
            Left = previousBottomEdge;
        }
        else if (angle == 90 * 2)
        {
            Top = previousBottomEdge;
            Right = previousLeftEdge;
            Bottom = previousTopEdge;
            Left = previousRightEdge;
        }
        else if (angle == 90 * 3)
        {
            Top = previousRightEdge;
            Right = previousBottomEdge;
            Bottom = previousLeftEdge;
            Left = previousTopEdge;
        }
    }
}

public class JigsawPiece : MonoBehaviour
{
    public Edges edges;

    private bool isSelected = false;
    private Vector3 offset;

    private void FixedUpdate()
    {
        if (isSelected)
        {
            Vector3 newPosition = this.GetHitPoint() + offset;
            this.transform.position = newPosition;

            if (Input.GetAxis("Mouse ScrollWheel") != 0)
            {
                this.RotatePiece(Input.GetAxis("Mouse ScrollWheel") < 0 ? -90 : 90);
            }
        }
    }
    
    private void OnMouseDown()
    {
        this.isSelected = !this.isSelected;
        this.offset = this.transform.position - this.GetHitPoint();

        if (!this.isSelected)
        {
            //this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, defaultPositionZ);
        }
    }

    /**
     * This method rotate the piece and its edges definition.
     * It is used to create new pieces from existing ones, and
     * should not be used directly by the player.
     * To do a "game rotation", use RotatePiece instead!
     */
    public void RotateModel(int rotation)
    {
        int angle = rotation * 90;

        this.RotatePiece(angle);
        this.edges.Rotate(angle);
    }

    public void RotatePiece(int angle)
    {
        this.transform.Rotate(0, 0, angle);
    }

    private Vector3 GetHitPoint()
    {
        Plane plane = new Plane(Camera.main.transform.forward, transform.position);
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        plane.Raycast(ray, out float dist);

        return ray.GetPoint(dist);
    }
}
