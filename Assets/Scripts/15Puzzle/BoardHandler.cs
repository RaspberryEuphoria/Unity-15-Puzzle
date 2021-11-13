using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.UI;

public class BoardHandler : MonoBehaviour
{
    private int rowAndColCount;
    private GameObject tileObject;
    private List<Tile> tiles;
    private Tile hiddenTile;
    // private float tileScale;
    // private float scaleFactor;

    public int[,] matrix;

    private GameState gameState;

    void Update()
    {
        if (!gameState.isReady)
        {
            return;
        }

        if (gameState.isWin)
        {
            if (!Mathf.Approximately(1.0f, hiddenTile.GameObject.transform.position.y / tileObject.transform.position.y))
            {
                float step = 15f * Time.deltaTime;

                hiddenTile.GameObject.transform.position = Vector3.MoveTowards(
                    hiddenTile.GameObject.transform.position,
                    new Vector3(hiddenTile.GameObject.transform.position.x, tileObject.transform.position.y, hiddenTile.GameObject.transform.position.z),
                    step
                );
            }

            return;
        }
    }

    public void Init(int rowAndColCount, List<Tile> tiles, Tile hiddenTile, GameObject tileObject, int[,] matrix)
    {
        this.enabled = true;
        this.rowAndColCount = rowAndColCount;
        this.tiles = tiles;
        this.hiddenTile = hiddenTile;
        this.matrix = matrix;
        this.tileObject = tileObject;

        gameState = GameObject.Find("GameState").GetComponent<GameState>();

        GetComponent<Animation>().Play();
    }

    public void HandleTileClick(Tile tile)
    {
        if (tile.IsSwappable(tiles, hiddenTile, rowAndColCount, matrix))
        {
            SwapTiles(tile, hiddenTile);

            gameState.IncrementGameMoves();

            if (IsBoardSolved())
            {
                gameState.SetVictory();
                hiddenTile.GameObject.SetActive(true);
            }
        }
    }

    void SwapTiles(Tile firstTile, Tile secondTile)
    {
        firstTile.SwapTile(secondTile);

        tiles[firstTile.Index] = firstTile;
        tiles[hiddenTile.Index] = hiddenTile;

        tiles.ForEach(tile => tile.SetIsSwappable(tile.IsSwappable(tiles, hiddenTile, rowAndColCount, matrix)));
    }

    bool IsBoardSolved()
    {
        int lastTileIndex = tiles[tiles.Count - 1].TrueIndex;

        for (int i = 0; i < tiles.Count; i++)
        {
            if (tiles[0].TrueIndex != 0)
            {
                return false;
            }

            if (lastTileIndex != tiles.Count - 1)
            {
                return false;
            }

            int leftTileIndex = tiles[i].TrueIndex;

            // If we weren't kicked at this point, it's a win!
            if (leftTileIndex == lastTileIndex)
            {
                return true;
            }

            int rightTileIndex = tiles[i + 1].TrueIndex;

            if (leftTileIndex != rightTileIndex - 1)
            {
                Debug.Log("Fail 3");
                return false;
            }
        }

        // We should not reach this part, but just in case...
        Debug.Log("Fail 4");
        return false;
    }
}
