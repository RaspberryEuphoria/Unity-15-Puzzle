using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class LoadBoard : MonoBehaviour
{
    [Header("Board configuration")]
    [Tooltip("Number of rows and columns for the board grid.")]
    public int rowAndColCount = 3;
    [Tooltip("Base board object. Should not be changed lightly!")]
    public GameObject boardObject;
    [Tooltip("Base tile object used to fill the board. Should not be changed lightly!")]
    public GameObject tileObject;
    [Tooltip("Base enveloppe object used to show a preview of the completed puzzle. Should not be changed lightly!")]
    public GameObject photoEnveloppeObject;
    [Tooltip("Default board texture. Will be replaced by user custom texture if there is one.")]
    public Texture2D tileTexture;

    [Header("Debug options")]
    [Tooltip("Disable tiles shuffle")]
    public bool _DEBUG_DISABLE_SHUFFLE = false;

    private List<Tile> tiles = new List<Tile>();
    private Tile hiddenTile;
    private float tileScale;
    private float scaleFactor;

    private GameState gameState;

    private int[,] matrix;

    private Configuration configuration;

    void Start()
    {
        gameState = GameObject.Find("GameState").GetComponent<GameState>();
        configuration = gameObject.GetComponentInParent<Configuration>();

        tileTexture = configuration.customTexture;

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
        
        configuration.gameCamera.GetComponent<CameraHandler>().StartTravelling(configuration.views.starting, () => {
            boardObject.GetComponent<BoardHandler>().Init(rowAndColCount, tiles, hiddenTile, tileObject, matrix);
        });
    }

    public Vector3 GetTileNewPosition(float x, float y)
    {
        return new Vector3(-x * tileScale + scaleFactor, tileObject.transform.position.y, y * tileScale - scaleFactor);
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

        configuration.SetCustomTexture(tileTexture);
        photoEnveloppeObject.GetComponent<PhotoFrameHandler>().SetPhotoTexture();
    }
}
