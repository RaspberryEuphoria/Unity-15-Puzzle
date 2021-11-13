using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

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

public class JigsawPiece : MonoBehaviour, IPointerDownHandler
{
    public Edges edges;

    private bool isSelected = false;
    private Vector3 offset;
    private float defaultPositionY;

    private void Awake()
    {
        this.defaultPositionY = this.transform.position.y;
    }

    private void FixedUpdate()
    {
        if (isSelected)
        {
            Vector3 newPosition = this.GetHitPoint() + offset;
            this.transform.position = new Vector3(newPosition.x, defaultPositionY, newPosition.z);
            this.transform.localPosition = new Vector3(this.transform.localPosition.x, this.transform.localPosition.y, 1f);

            //if (Input.GetAxis("Mouse ScrollWheel") != 0)
            //{
                //this.RotatePiece(Input.GetAxis("Mouse ScrollWheel") < 0 ? -90 : 90);
            //}
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        this.isSelected = !this.isSelected;
        this.offset = this.transform.position - this.GetHitPoint();

        if (!this.isSelected)
        {
            this.transform.position = new Vector3(this.transform.position.x, defaultPositionY, this.transform.position.z);
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
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        plane.Raycast(ray, out float dist);

        return ray.GetPoint(dist);
    }
}
