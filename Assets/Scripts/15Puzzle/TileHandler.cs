using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Tile
{
    public GameObject GameObject;
    public int TrueIndex;
    public int Index;

    public int x;
    public int y;
    public bool isSwappable = false;

    private Vector3 targetPosition;
    private bool isMoving = false;

    public Tile(GameObject gameObject, Texture texture, int trueIndex, int index)
    {
        GameObject = gameObject;
        TrueIndex = trueIndex;
        Index = index;

        gameObject.GetComponent<Renderer>().material.SetTexture("_MainTex", texture);
        gameObject.name = "Tile #" + index.ToString();
    }

    public Vector3 GetTargetPosition()
    {
        return targetPosition;
    }

    public bool IsMoving()
    {
        return isMoving;
    }

    public void SetIndex(int index)
    {
        Index = index;
    }

    public void SetXY(int newX, int newY)
    {
        x = newX;
        y = newY;
    }

    public void SetIsSwappable(bool newIsSwappable)
    {
        isSwappable = newIsSwappable;
    }

    public void Hide()
    {
        GameObject.SetActive(false);
        GameObject.transform.position = new Vector3(GameObject.transform.position.x, 20, GameObject.transform.position.z);
    }

    public bool IsSwappable(List<Tile> tiles, Tile hiddenTile, int rowAndColCount, int[,] matrix)
    {
        if (y > 0 && tiles[matrix[x, y - 1]] != null)
        {
            Tile upperTile = tiles[matrix[x, y - 1]];

            if (upperTile.TrueIndex == hiddenTile.TrueIndex)
            {
                return true;
            }
        }

        if (x > 0 && tiles[matrix[x - 1, y]] != null)
        {
            Tile leftTile = tiles[matrix[x - 1, y]];

            if (leftTile.TrueIndex == hiddenTile.TrueIndex)
            {
                return true;
            }
        }

        if (y + 1 < rowAndColCount && tiles[matrix[x, y + 1]] != null)
        {
            Tile lowerTile = tiles[matrix[x, y + 1]];

            if (lowerTile.TrueIndex == hiddenTile.TrueIndex)
            {
                return true;
            }
        }

        if (x + 1 < rowAndColCount && tiles[matrix[x + 1, y]] != null)
        {
            Tile rightTile = tiles[matrix[x + 1, y]];

            if (rightTile.TrueIndex == hiddenTile.TrueIndex)
            {
                return true;
            }
        }

        return false;
    }

    public void SwapTile(Tile hiddenTile)
    {
        Vector3 currentTilePosition = GameObject.transform.position;
        Vector3 hiddenTilePosition = hiddenTile.GameObject.transform.position;

        int currentTileIndex = Index;
        int currentTileX = x;
        int currentTileY = y;

        targetPosition = new Vector3(hiddenTilePosition.x, currentTilePosition.y, hiddenTilePosition.z);
        SetIsMoving(true);

        hiddenTile.GameObject.transform.position = new Vector3(currentTilePosition.x, hiddenTilePosition.y, currentTilePosition.z);

        SetIndex(hiddenTile.Index);
        SetXY(hiddenTile.x, hiddenTile.y);

        hiddenTile.SetIndex(currentTileIndex);
        hiddenTile.SetXY(currentTileX, currentTileY);
    }

    public void SetIsMoving(bool newIsMoving)
    {
        isMoving = newIsMoving;
    }
}

public class TileHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    public GameObject tileBorder;
    public float moveSpeed = 35f;

    private Tile tile;
    private GameState gameState;
    private BoardHandler board;

    private void Start()
    {
        gameState = GameObject.Find("GameState").GetComponent<GameState>();
        board = GameObject.Find("15Puzzle_Board").GetComponent<BoardHandler>();
    }

    private void Update()
    {
        if (!gameState.isReady)
        {
            return;
        }

        Vector3 targetPosition = tile.GetTargetPosition();

        if (!tile.IsMoving())
        {
            return;
        }

        if (!Mathf.Approximately(1.0f, tile.GameObject.transform.position.y / targetPosition.y))
        {
            float step = moveSpeed * Time.deltaTime;

            tile.GameObject.transform.position = Vector3.MoveTowards(
                tile.GameObject.transform.position,
                new Vector3(targetPosition.x, targetPosition.y, targetPosition.z),
                step
            );
        } else
        {
            tile.SetIsMoving(false);
        }
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        if (gameState.isWin)
        {
            return;
        }

        if (tile.isSwappable)
        {
            tileBorder.SetActive(true);
        }
    }

    // ...and the mesh finally turns white when the mouse moves away.
    public void OnPointerExit(PointerEventData eventData)
    {
        tileBorder.SetActive(false);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        board.HandleTileClick(tile);
    }

    public void SetTile(Tile value)
    {
        tile = value;
    }
}
