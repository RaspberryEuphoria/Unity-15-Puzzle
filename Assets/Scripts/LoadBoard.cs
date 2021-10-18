using SFB;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class LoadBoard : MonoBehaviour
{
    [Header("Board configuration")]
    [Tooltip("The base tile object used to fill the board.")]
    private GameObject tileObject;
    [Tooltip("A texture to split on the tiles.")]
    public Texture2D tileTexture;
    [Tooltip("A number of rows and columns for the board grid.")]
    public int rowAndColCount = 3;

    [Header("Debug options")]
    [Tooltip("Disable tiles shuffle")]
    public bool _DEBUG_DISABLE_SHUFFLE = false;

    private List<Tile> tiles = new List<Tile>();
    private Tile hiddenTile;
    private float tileScale;
    private float scaleFactor;

    private GameState gameState;

    private int[,] matrix;

    void Start()
    {
        gameState = GameObject.Find("GameState").GetComponent<GameState>();

        StartCoroutine(PrepareBoard());
    }

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

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit raycastHit, 100f))
        {
            if (raycastHit.transform != null)
            {
                GameObject gameObject = raycastHit.transform.gameObject;

                if (gameObject.CompareTag("Tile"))
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        HandleTileClick(gameObject);
                    } 
                }
            }
        }
    }

    IEnumerator PrepareBoard()
    {
        ExtensionFilter[] extensions = new[] { new ExtensionFilter("Image Files", "jpg", "png") };

        string[] paths = StandaloneFileBrowser.OpenFilePanel("Title", "", extensions, false);

        if (paths.Length > 0)
        {
            string url = new System.Uri(paths[0]).AbsoluteUri;

            UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
            yield return request.SendWebRequest();
            tileTexture = DownloadHandlerTexture.GetContent(request);
        }

        tileObject = GameObject.Find("Tile");

        SetupBoard();
    }

    void SetupBoard()
    {
        if (rowAndColCount == 4)
        {
            tileObject.transform.localScale = new Vector3(0.7f, 0.7f, 0.7f);
            scaleFactor = 0.5f;
        }
        else
        {
            scaleFactor = 0;
        }

        tileScale = tileObject.transform.localScale.x * 10f;
        List<Texture2D> splitedTexture = SplitTextures();

        int index = 0;
        for (int x = 0; x < rowAndColCount; x++)
        {
            for (int y = 0; y < rowAndColCount; y++)
            {
                GameObject newTileObject = Instantiate(tileObject, GetTileNewPosition(x, y), Quaternion.identity);

                Tile tile = new Tile(newTileObject, splitedTexture[index], index, index);
                tiles.Add(tile);
                newTileObject.GetComponent<TileHandler>().SetTile(tile);

                index++;
            }
        }

        tileObject.SetActive(false);
        hiddenTile = tiles[tiles.Count - 1];

        ShuffleTiles();
    }

    void ShuffleTiles()
    {
        if (!_DEBUG_DISABLE_SHUFFLE)
        {
            tiles = LoadBoard.Shuffle(tiles);
        }

        matrix = new int[rowAndColCount, rowAndColCount];

        int index = 0;
        for (int y = 0; y < rowAndColCount; y++)
        {
            for (int x = 0; x < rowAndColCount; x++)
            {
                matrix[x, y] = tiles[index].Index;

                Tile tile = tiles[index];

                tile.SetXY(x, y);
                tile.GameObject.transform.position = GetTileNewPosition(x, y);

                index++;
            }
        }

        if (this.IsBoardSolvable(tiles)) {
            StartGame();
        } else
        {
            ShuffleTiles();
        }
    }

    public void StartGame()
    {
        gameState.SetIsReady();
        hiddenTile.Hide();
        tiles.ForEach(tile => tile.SetIsSwappable(tile.IsSwappable(tiles, hiddenTile, rowAndColCount, matrix)));

        this.GetComponent<Animation>().Play();
    }

    public void HandleTileClick(GameObject gameObject)
    {
        Tile tile = tiles.Find(t => t.GameObject.GetInstanceID() == gameObject.GetInstanceID());

        if ( tile == null)
        {
            return;
        }

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

    public Vector3 GetTileNewPosition(float x, float y)
    {
        return new Vector3(-x * tileScale + scaleFactor, tileObject.transform.position.y, y * tileScale - scaleFactor);
        // return new Vector3(-y * tileScale + scaleFactor, tileObject.transform.position.y, x * tileScale - scaleFactor);
    }

    public void SwapTiles(Tile firstTile, Tile secondTile)
    {
        firstTile.SwapTile(secondTile);

        tiles[firstTile.Index] = firstTile;
        tiles[hiddenTile.Index] = hiddenTile;

        tiles.ForEach(tile => tile.SetIsSwappable(tile.IsSwappable(tiles, hiddenTile, rowAndColCount, matrix)));
    }

    /**
     * A grid with an odd count of rows/cols is solvable
     * if the number of "inverted" pairs is even.
     * For an even count, the number of inverted pairs 
     * should be odd.
     * 
     * @see https://datawookie.dev/blog/2019/04/sliding-puzzle-solvable/ for reference
     */
    public bool IsBoardSolvable(List<Tile> tiles)
    {
        int inversions = 0;

        for (int i = 0; i < tiles.Count; i++)
        {
            int left = tiles[i].TrueIndex;

            for (int j = i + 1; j < tiles.Count; j++)
            {
                int right = tiles[j].TrueIndex;

                if (left > right && left != hiddenTile.TrueIndex && right != hiddenTile.TrueIndex)
                {
                    inversions++;
                }
            }
        }
        
        if (rowAndColCount % 3 == 0)
        {
            return inversions % 2 == 0;
        } else
        {
            return inversions % 2 != 0; // @Todo verify if this works :)
        }

    }

    public bool IsBoardSolved()
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

    public static List<Tile> Shuffle(List<Tile> _tiles)
    {
        for (int i = 0; i < _tiles.Count; i++)
        {
            Tile temp = _tiles[i];
            int randomIndex = Random.Range(i, _tiles.Count);

            _tiles[i] = _tiles[randomIndex];
            _tiles[i].SetIndex(i);

            _tiles[randomIndex] = temp;
            _tiles[randomIndex].SetIndex(randomIndex);
        }

        return _tiles;
    }

    public List<Texture2D> SplitTextures()
    {
        if (tileTexture.width != tileTexture.height)
        {
            CutSquareInTexture();
        }

        int sourceWidth = tileTexture.width;
        int sourceHeight = tileTexture.height;

        Vector2Int textureSize = new Vector2Int(sourceWidth / rowAndColCount, sourceHeight / rowAndColCount);

        List<Texture2D> splitedTexture = new List<Texture2D>();

        // Use a reverse for loop because we want to start from the top of the image
        for (int y = rowAndColCount - 1; y >= 0; y--)
        {
            for (int x = 0; x < rowAndColCount; x++)
            {
                int bottomLeftPixelX = x * textureSize.x;
                int bottomLeftPixelY = y * textureSize.y;

                Color[] pixels = tileTexture.GetPixels(bottomLeftPixelX, bottomLeftPixelY, textureSize.x, textureSize.y);
                Texture2D texture = new Texture2D(textureSize.x, textureSize.y, tileTexture.format, false);

                texture.SetPixels(pixels);
                texture.Apply();

                splitedTexture.Add(texture);
            }
        }

        return splitedTexture;
    }

    private void CutSquareInTexture()
    {
        int x = tileTexture.width; 
        int y = tileTexture.height;

        int smallestSize = Mathf.Min(x, y);

        if (smallestSize == y)
        {
            Color[] pixels = tileTexture.GetPixels((x - y) / 2, 0, y, y);
            Texture2D texture = new Texture2D(y, y, tileTexture.format, false);

            texture.SetPixels(pixels);
            texture.Apply();

            tileTexture = texture;
        } else
        {
            Color[] pixels = tileTexture.GetPixels(0, (y - x) / 2, x, x);
            Texture2D texture = new Texture2D(x, x, tileTexture.format, false);

            texture.SetPixels(pixels);
            texture.Apply();

            tileTexture = texture;
        }
    }
}
