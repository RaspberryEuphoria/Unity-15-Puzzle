using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Orientation
{
    Portrait,
    Vertical,
    Horizontal
}

public enum Difficulty
{
    Easy,
    Medium,
    Hard,
    VeryHard
}

public class JigsawPuzzleProperties : ScriptableObject
{
    public int rowCount;
    public int colCount;
    public int piecesCount;
    public float scaleModifier;

    public void Init(float ratio, Difficulty difficulty, Orientation orientation)
    {
        int defaultPiecesCount = 75;
        int modifier = 1;

        switch (difficulty)
        {
            case Difficulty.Easy:
                modifier = 1;
                break;
            case Difficulty.Medium:
                modifier = 4;
                break;
            case Difficulty.Hard:
                modifier = 8;
                break;
            case Difficulty.VeryHard:
                modifier = 16;
                break;
        }

        int piecesCount = defaultPiecesCount * modifier;

        switch (orientation)
        {
            case Orientation.Portrait:
                this.colCount = (int) Mathf.Sqrt(piecesCount);
                this.rowCount = this.colCount;
                break;
            case Orientation.Vertical:
                this.colCount = (int) Math.Round(Mathf.Sqrt(piecesCount / ratio));
                this.rowCount = (int) (this.colCount * ratio);
                break;
            case Orientation.Horizontal:
                this.rowCount = (int) Math.Round(Mathf.Sqrt(piecesCount / ratio));
                this.colCount = (int) (this.rowCount * ratio);
                break;
        }


        this.piecesCount = this.colCount * this.rowCount;
        this.scaleModifier = -((modifier - 1f) / 100f);
    }
}

[System.Serializable]
public class DifficultyCamera
{
    public Transform easyTransform;
    public Transform mediumTransform;
    public Transform hardTransform;
    public Transform veryHardTransform;

    public Transform GetTransform(Difficulty difficulty)
    {
        switch (difficulty)
        {
            case Difficulty.Easy:
                return easyTransform;
            case Difficulty.Medium: 
                return mediumTransform;
            case Difficulty.Hard:
                return hardTransform;
            case Difficulty.VeryHard:
            default:
                return veryHardTransform;
        }
    }
}

public class LoadJigsawBoard : MonoBehaviour
{
    [Tooltip("Base board object. Should not be changed lightly!")]
    public GameObject boardObject;
    public GameObject support;
    public Difficulty puzzleDifficulty = Difficulty.Easy;
    public int scaleFactor;
    public DifficultyCamera difficultyCamera;

    private JigsawPuzzleProperties puzzleProperties;

    private Texture2D puzzleTexture;
    private List<JigsawPiece> puzzleShapes = new List<JigsawPiece>();
    private JigsawPiece[,] matrix;

    private Orientation puzzleOrientation;
    private float puzzleRatio;
    private Configuration configuration;

    void Start()
    {
        Vector3 initialRotation = this.transform.eulerAngles;
        configuration = gameObject.GetComponentInParent<Configuration>();

        PrepareTexture();
        SetupPieces();

        StartGame(initialRotation);
    }

    private void StartGame(Vector3 rotation)
    {
        this.transform.rotation = Quaternion.Euler(rotation);
        this.transform.localScale = new Vector3(1f + puzzleProperties.scaleModifier, 1f, 1f + puzzleProperties.scaleModifier);
        this.support.transform.localScale = new Vector3((1f * this.puzzleProperties.colCount) / 2f, 1, (1f * this.puzzleProperties.rowCount) / 2f);

        CameraHandler camera = configuration.gameCamera.GetComponent<CameraHandler>();

        camera.StartTravelling(this.configuration.cameraTransform, () => {
            camera.SetCameraMode(this.configuration.cameraMode);
        });
    }

    private void SetupPieces()
    {
        // Reset rotation to prevent issues while cuting pieces texture
        this.transform.rotation = Quaternion.Euler(0, 0, 0);

        this.puzzleProperties = ScriptableObject.CreateInstance<JigsawPuzzleProperties>();
        this.puzzleProperties.Init(this.puzzleRatio, this.puzzleDifficulty, this.puzzleOrientation);
        this.matrix = new JigsawPiece[this.puzzleProperties.colCount, this.puzzleProperties.rowCount];

        List<Transform> variants = new List<Transform>();

        foreach (Transform child in boardObject.transform)
        {
            JigsawPiece jigsawpiece = child.gameObject.GetComponent<JigsawPiece>();
            puzzleShapes.Add(jigsawpiece);

            for (int i = 1; i <= 3; i++)
            {
                GameObject variant = Instantiate(child.gameObject, child.transform.position, child.transform.rotation);
                JigsawPiece variantHandler = variant.GetComponent<JigsawPiece>();

                puzzleShapes.Add(variantHandler);
                variants.Add(variant.transform);

                variant.name = child.gameObject.name + "_" + i * 90 + "°";
                variantHandler.RotateModel(i);
            }
        }

        foreach (Transform child in variants)
        {
            child.SetParent(boardObject.transform);
        }

        int index = 0;
        for (int x = 0; x < this.puzzleProperties.colCount; x++)
        {
            for (int y = 0; y < this.puzzleProperties.rowCount; y++)
            {
                GameObject pieceShape = GetRandomShape(x, y);
                GameObject jigsawpiece = Instantiate(pieceShape, pieceShape.transform.position, pieceShape.transform.rotation);

                matrix[x, y] = jigsawpiece.GetComponent<JigsawPiece>();

                jigsawpiece.SetActive(true);
                jigsawpiece.name = "Piece#" + index + " :: " + pieceShape.name;

                jigsawpiece.GetComponent<Renderer>().material.SetTexture("_MainTex", GetTexturePart(jigsawpiece, x, y, index));
                jigsawpiece.transform.SetParent(boardObject.transform);
                jigsawpiece.transform.localPosition = new Vector3(x * -this.scaleFactor, y * -this.scaleFactor, 0);

                index++;
            }
        }

        foreach (Transform child in boardObject.transform)
        {
            if (!child.gameObject.activeSelf)
            {
                Destroy(child.gameObject);
            }
        }
    }

    private GameObject GetRandomShape(int x, int y)
    {
        Shuffle(puzzleShapes);

        if (y == this.puzzleProperties.rowCount - 1) // last row
        {
            Edge topShapeBottomEdge = matrix[x, y - 1].edges.Bottom;

            if (x == 0) // first piece of row
            {
                return puzzleShapes.Find(shape =>
                    shape.edges.Top != Edge.Straight
                    && shape.edges.Top != topShapeBottomEdge
                    && shape.edges.Left == Edge.Straight
                    && shape.edges.Right != Edge.Straight
                    && shape.edges.Bottom == Edge.Straight
                ).gameObject;
            }

            Edge leftShapeRightEdge = matrix[x - 1, y].edges.Right;

            if (x == this.puzzleProperties.colCount - 1) // last piece of row
            {
                return puzzleShapes.Find(shape =>
                    shape.edges.Top != Edge.Straight
                    && shape.edges.Top != topShapeBottomEdge
                    && shape.edges.Left != Edge.Straight
                    && shape.edges.Left != leftShapeRightEdge
                    && shape.edges.Right == Edge.Straight
                    && shape.edges.Bottom == Edge.Straight
                ).gameObject;
            }
            else // middle piece
            {
                return puzzleShapes.Find(shape =>
                    shape.edges.Top != Edge.Straight
                    && shape.edges.Top != topShapeBottomEdge
                    && shape.edges.Left != Edge.Straight
                    && shape.edges.Left != leftShapeRightEdge
                    && shape.edges.Right != Edge.Straight
                    && shape.edges.Bottom == Edge.Straight
                ).gameObject;
            }
        }
        else
        {
            if (y == 0) // first row
            {
                if (x == 0) // first piece of row
                {
                    return puzzleShapes.Find(shape =>
                        shape.edges.Top == Edge.Straight
                        && shape.edges.Left == Edge.Straight
                        && shape.edges.Right != Edge.Straight
                        && shape.edges.Bottom != Edge.Straight
                    ).gameObject;
                }

                Edge leftShapeRightEdge = matrix[x - 1, y].edges.Right;

                if (x == this.puzzleProperties.colCount - 1) // last piece of row
                {
                    return puzzleShapes.Find(shape =>
                        shape.edges.Top == Edge.Straight
                        && shape.edges.Left != Edge.Straight
                        && shape.edges.Left != leftShapeRightEdge
                        && shape.edges.Right == Edge.Straight
                        && shape.edges.Bottom != Edge.Straight
                    ).gameObject;
                }
                else // middle pieces
                {
                    return puzzleShapes.Find(shape =>
                        shape.edges.Top == Edge.Straight
                        && shape.edges.Left != Edge.Straight
                        && shape.edges.Left != leftShapeRightEdge
                        && shape.edges.Right != Edge.Straight
                        && shape.edges.Bottom != Edge.Straight
                    ).gameObject;
                }
            }
            else // middle rows
            {
                Edge topShapeBottomEdge = matrix[x, y - 1].edges.Bottom;

                if (x == 0) // first piece of row
                {
                    return puzzleShapes.Find(shape =>
                        shape.edges.Top != Edge.Straight
                        && shape.edges.Top != topShapeBottomEdge
                        && shape.edges.Left == Edge.Straight
                        && shape.edges.Right != Edge.Straight
                        && shape.edges.Bottom != Edge.Straight
                    ).gameObject;
                }

                Edge leftShapeRightEdge = matrix[x - 1, y].edges.Right;

                if (x == this.puzzleProperties.colCount - 1) // last piece of row
                {
                    return puzzleShapes.Find(shape =>
                        shape.edges.Top != Edge.Straight
                        && shape.edges.Top != topShapeBottomEdge
                        && shape.edges.Left != Edge.Straight
                        && shape.edges.Left != leftShapeRightEdge
                        && shape.edges.Right == Edge.Straight
                        && shape.edges.Bottom != Edge.Straight
                    ).gameObject;
                }
                else // middle pieces
                {
                    return puzzleShapes.Find(shape =>
                        shape.edges.Top != Edge.Straight
                        && shape.edges.Top != topShapeBottomEdge
                        && shape.edges.Left != Edge.Straight
                        && shape.edges.Left != leftShapeRightEdge
                        && shape.edges.Right != Edge.Straight
                        && shape.edges.Bottom != Edge.Straight
                    ).gameObject;
                }
            }
        }
    }

    private int ComputePieceSize()
    {
        switch (this.puzzleOrientation)
        {
            case Orientation.Vertical:
                return this.puzzleTexture.width / this.puzzleProperties.colCount;
            case Orientation.Horizontal:
                return this.puzzleTexture.height / this.puzzleProperties.rowCount;
            default:
            case Orientation.Portrait:
                return this.puzzleTexture.width / this.puzzleProperties.colCount;
        }
    }

    private Texture2D GetTexturePart(GameObject piece, int x, int y, int index)
    {
        int baseWidth = ComputePieceSize();
        int baseHeight = baseWidth;
        int widthWithEdges = baseWidth;
        int heightWithEdges = baseHeight;

        int edgePercent = 30; // % qu'un "bord" prend sur toute la taille d'une pièce de puzzle // true value = 29.799
        int edgeWidth = edgePercent * widthWithEdges / 100;
        int edgeHeight = edgePercent * heightWithEdges / 100;

        JigsawPiece jigsawPiece = piece.GetComponent<JigsawPiece>();

        if (jigsawPiece.edges.Left == Edge.Hook)
        {
            widthWithEdges += edgeWidth;
        }

        if (jigsawPiece.edges.Right == Edge.Hook)
        {
            widthWithEdges += edgeWidth;
        }

        if (jigsawPiece.edges.Top == Edge.Hook)
        {
            heightWithEdges += edgeHeight;
        }

        if (jigsawPiece.edges.Bottom == Edge.Hook)
        {
            heightWithEdges += edgeHeight;
        }

        Vector2Int textureSize = new Vector2Int(widthWithEdges, heightWithEdges);

        int bottomLeftPixelX = x * baseWidth;
        int bottomLeftPixelY = puzzleTexture.height - (baseHeight * (y + 1));

        if (jigsawPiece.edges.Left == Edge.Hook)
        {
            bottomLeftPixelX -= edgeWidth;
        }

        if (jigsawPiece.edges.Bottom == Edge.Hook)
        {
            bottomLeftPixelY -= edgeHeight;
        }

        Color[] pixels = puzzleTexture.GetPixels(bottomLeftPixelX, bottomLeftPixelY, textureSize.x, textureSize.y);
        Texture2D texture = new Texture2D(textureSize.x, textureSize.y, puzzleTexture.format, false);

        texture.SetPixels(pixels);
        texture.Apply();
        texture.wrapMode = TextureWrapMode.Clamp;

        if (piece.transform.localRotation.eulerAngles.y != 0)
        {
            float degs = 1;
            switch (piece.transform.localRotation.eulerAngles.y)
            {
                case 90:
                    degs = 1.58f;
                    break;
                case 180:
                    degs = 3.15f;
                    break;
                case 270:
                    degs = 4.71f;
                    break;
            }

            // thanks to https://forum.unity.com/threads/solved-surface-shader-rotating-texture-and-normal-map-is-working-but-lighting-response-is-not.452242/
            piece.GetComponent<Renderer>().sharedMaterial.SetFloat("_Angle", degs);
        }

        return texture;
    }

    private float GetRatio(float leftValue, float rightValue)
    {
        float ratio = Mathf.Max(leftValue, rightValue) / Mathf.Min(leftValue, rightValue);

        return ratio;
    }

    private int AdjustValue(float value, float ratio)
    {
        return (int) (value * ratio);
    }

    private void Shuffle(List<JigsawPiece> pieces)
    {
        for (int i = 0; i < pieces.Count; i++)
        {
            JigsawPiece temp = pieces[i];
            int randomIndex = UnityEngine.Random.Range(i, pieces.Count);

            pieces[i] = pieces[randomIndex];

            pieces[randomIndex] = temp;
        }

        puzzleShapes = pieces;
    }

    private void PrepareTexture()
    {
        Texture2D basePuzzleTexture = this.configuration.customTexture;
        int x = basePuzzleTexture.width & ~1; // round to nearest even int (down)
        int y = basePuzzleTexture.height & ~1;

        if (x == y)
        {
            this.puzzleTexture = basePuzzleTexture;
            this.puzzleRatio = 1;
            this.puzzleOrientation = Orientation.Portrait;

            return;
        }

        int smallestSize = Mathf.Min(x, y);
        float ratio = GetRatio(x, y);

        if (smallestSize == y) // Horizontal
        {
            int newX = AdjustValue(y, ratio);

            Color[] pixels = basePuzzleTexture.GetPixels((x - newX) / 2, 0, newX, y);
            Texture2D texture = new Texture2D(newX, y, basePuzzleTexture.format, false);

            texture.SetPixels(pixels);
            texture.Apply();

            this.puzzleTexture = texture;
            this.puzzleOrientation = ratio > 1 ? Orientation.Horizontal : Orientation.Portrait;
        }
        else // Vertical
        {

            int newY = AdjustValue(x, ratio);

            Color[] pixels = basePuzzleTexture.GetPixels(0, (y - newY) / 2, x, newY);
            Texture2D texture = new Texture2D(x, newY, basePuzzleTexture.format, false);

            texture.SetPixels(pixels);
            texture.Apply();

            this.puzzleTexture = texture;
            this.puzzleOrientation = ratio > 1 ? Orientation.Vertical : Orientation.Portrait;
        }

        this.puzzleRatio = ratio;
    }
}