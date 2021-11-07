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

    void Simulation()
    {
        for (int ratio = 2; ratio < 5; ratio++)
        {
            int baseN = 100;

            int x = (int) Math.Round(Mathf.Sqrt(baseN / ratio));
            int y = x * ratio;

            int n = x * y;

            Debug.Log("Pour y = " + ratio + "x, et xy =~ " + baseN);
            Debug.Log("x = " + x);
            Debug.Log("y = " + y);
            Debug.Log("Le nombre le plus proche de " + baseN + " = " + n);
        }
    }

    /*
    Est-ce qu'un matheux pourrait m'aider à résoudre un problème, s'il vous plaît ?
    J'ai un nombre **x** dont je ne connais pas valeur
    Un nombre **y** qui est égal à **x*2**
    x*y s'approche le plus possible de 100 (ça peut-être un peu au-dessus ou un peu en-dessous)
    Comment trouver x ?
    */

    public void Init(float ratio, Difficulty difficulty, Orientation orientation)
    {
        int piecesCount = 75;
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

        piecesCount *= modifier;

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
    }
}

public class PrepareJigsawPuzzle : MonoBehaviour
{
    public Texture2D basePuzzleTexture;
    public Difficulty puzzleDifficulty = Difficulty.Easy;

    public JigsawPuzzleProperties puzzleProperties;

    public Texture2D puzzleTexture;
    private List<HandleJigsawaPiece> puzzleShapes = new List<HandleJigsawaPiece>();
    private HandleJigsawaPiece[,] matrix;

    public Orientation puzzleOrientation;
    public float puzzleRatio;
    public int scaleFactor;

    // Start is called before the first frame update
    void Start()
    {
        PrepareTexture();

        this.puzzleProperties = ScriptableObject.CreateInstance<JigsawPuzzleProperties>();
        this.puzzleProperties.Init(this.puzzleRatio, this.puzzleDifficulty, this.puzzleOrientation);

        Debug.Log(this.puzzleProperties.rowCount);
        Debug.Log(this.puzzleProperties.colCount);
        Debug.Log(this.puzzleProperties.piecesCount);

        this.matrix = new HandleJigsawaPiece[this.puzzleProperties.colCount, this.puzzleProperties.rowCount];

        List<Transform> variants = new List<Transform>();

        foreach (Transform child in transform)
        {
            HandleJigsawaPiece handler = child.gameObject.GetComponent<HandleJigsawaPiece>();
            puzzleShapes.Add(handler);

            for (int i = 1; i <= 3; i++)
            {
                GameObject variant = Instantiate(child.gameObject, child.transform.position, child.transform.rotation);
                HandleJigsawaPiece variantHandler = variant.GetComponent<HandleJigsawaPiece>();

                puzzleShapes.Add(variantHandler);
                variants.Add(variant.transform);

                variant.name = child.gameObject.name + "_" + i * 90 + "°";
                variantHandler.RotatePiece(i);
            }
        }

        foreach (Transform child in variants)
        {
            child.SetParent(transform);
        }

        int index = 0;
        for (int x = 0; x < this.puzzleProperties.colCount; x++)
        {
            for (int y = 0; y < this.puzzleProperties.rowCount; y++)
            {
                GameObject pieceShape = GetRandomShape(x, y);
                GameObject piece = Instantiate(pieceShape, pieceShape.transform.position, pieceShape.transform.rotation);

                matrix[x, y] = piece.GetComponent<HandleJigsawaPiece>();

                piece.SetActive(true);
                piece.name = "Piece#" + index + " :: " + pieceShape.name;

                piece.GetComponent<Renderer>().material.SetTexture("_MainTex", GetTexturePart(piece, x, y, index));
                piece.transform.SetParent(transform);
                piece.transform.localPosition = new Vector3(x * -this.scaleFactor, y * -this.scaleFactor, 0);

                index++;
            }
        }
    }

    private GameObject GetRandomShape(int x, int y)
    {
        Shuffle(puzzleShapes);

        if (y == this.puzzleProperties.rowCount - 1) // last row
        {
            Edge topShapeBottomEdge = matrix[x, y - 1].bottomEdge;

            if (x == 0) // first piece of row
            {
                return puzzleShapes.Find(shape =>
                    shape.topEdge != Edge.Straight
                    && shape.topEdge != topShapeBottomEdge
                    && shape.leftEdge == Edge.Straight
                    && shape.rightEdge != Edge.Straight
                    && shape.bottomEdge == Edge.Straight
                ).gameObject;
            }

            Edge leftShapeRightEdge = matrix[x - 1, y].rightEdge;

            if (x == this.puzzleProperties.colCount - 1) // last piece of row
            {
                return puzzleShapes.Find(shape =>
                    shape.topEdge != Edge.Straight
                    && shape.topEdge != topShapeBottomEdge
                    && shape.leftEdge != Edge.Straight
                    && shape.leftEdge != leftShapeRightEdge
                    && shape.rightEdge == Edge.Straight
                    && shape.bottomEdge == Edge.Straight
                ).gameObject;
            }
            else // middle piece
            {
                return puzzleShapes.Find(shape =>
                    shape.topEdge != Edge.Straight
                    && shape.topEdge != topShapeBottomEdge
                    && shape.leftEdge != Edge.Straight
                    && shape.leftEdge != leftShapeRightEdge
                    && shape.rightEdge != Edge.Straight
                    && shape.bottomEdge == Edge.Straight
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
                        shape.topEdge == Edge.Straight
                        && shape.leftEdge == Edge.Straight
                        && shape.rightEdge != Edge.Straight
                        && shape.bottomEdge != Edge.Straight
                    ).gameObject;
                }

                Edge leftShapeRightEdge = matrix[x - 1, y].rightEdge;

                if (x == this.puzzleProperties.colCount - 1) // last piece of row
                {
                    return puzzleShapes.Find(shape =>
                        shape.topEdge == Edge.Straight
                        && shape.leftEdge != Edge.Straight
                        && shape.leftEdge != leftShapeRightEdge
                        && shape.rightEdge == Edge.Straight
                        && shape.bottomEdge != Edge.Straight
                    ).gameObject;
                }
                else // middle pieces
                {
                    return puzzleShapes.Find(shape =>
                        shape.topEdge == Edge.Straight
                        && shape.leftEdge != Edge.Straight
                        && shape.leftEdge != leftShapeRightEdge
                        && shape.rightEdge != Edge.Straight
                        && shape.bottomEdge != Edge.Straight
                    ).gameObject;
                }
            }
            else // middle rows
            {
                Edge topShapeBottomEdge = matrix[x, y - 1].bottomEdge;

                if (x == 0) // first piece of row
                {
                    return puzzleShapes.Find(shape =>
                        shape.topEdge != Edge.Straight
                        && shape.topEdge != topShapeBottomEdge
                        && shape.leftEdge == Edge.Straight
                        && shape.rightEdge != Edge.Straight
                        && shape.bottomEdge != Edge.Straight
                    ).gameObject;
                }

                Edge leftShapeRightEdge = matrix[x - 1, y].rightEdge;

                if (x == this.puzzleProperties.colCount - 1) // last piece of row
                {
                    return puzzleShapes.Find(shape =>
                        shape.topEdge != Edge.Straight
                        && shape.topEdge != topShapeBottomEdge
                        && shape.leftEdge != Edge.Straight
                        && shape.leftEdge != leftShapeRightEdge
                        && shape.rightEdge == Edge.Straight
                        && shape.bottomEdge != Edge.Straight
                    ).gameObject;
                }
                else // middle pieces
                {
                    return puzzleShapes.Find(shape =>
                        shape.topEdge != Edge.Straight
                        && shape.topEdge != topShapeBottomEdge
                        && shape.leftEdge != Edge.Straight
                        && shape.leftEdge != leftShapeRightEdge
                        && shape.rightEdge != Edge.Straight
                        && shape.bottomEdge != Edge.Straight
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

        HandleJigsawaPiece jigsawPiece = piece.GetComponent<HandleJigsawaPiece>();

        if (jigsawPiece.leftEdge == Edge.Hook)
        {
            widthWithEdges += edgeWidth;
        }

        if (jigsawPiece.rightEdge == Edge.Hook)
        {
            widthWithEdges += edgeWidth;
        }

        if (jigsawPiece.topEdge == Edge.Hook)
        {
            heightWithEdges += edgeHeight;
        }

        if (jigsawPiece.bottomEdge == Edge.Hook)
        {
            heightWithEdges += edgeHeight;
        }

        Vector2Int textureSize = new Vector2Int(widthWithEdges, heightWithEdges);

        int bottomLeftPixelX = x * baseWidth;
        int bottomLeftPixelY = puzzleTexture.height - (baseHeight * (y + 1));

        if (jigsawPiece.leftEdge == Edge.Hook)
        {
            bottomLeftPixelX -= edgeWidth;
        }

        if (jigsawPiece.bottomEdge == Edge.Hook)
        {
            bottomLeftPixelY -= edgeHeight;
        }

        jigsawPiece.widthWithEdges = widthWithEdges;
        jigsawPiece.heightWithEdges = heightWithEdges;
        jigsawPiece.bottomLeftPixelX = bottomLeftPixelX;
        jigsawPiece.bottomLeftPixelY = bottomLeftPixelY;

        Color[] pixels = puzzleTexture.GetPixels(bottomLeftPixelX, bottomLeftPixelY, textureSize.x, textureSize.y);
        Texture2D texture = new Texture2D(textureSize.x, textureSize.y, puzzleTexture.format, false);

        texture.SetPixels(pixels);
        texture.Apply();
        texture.wrapMode = TextureWrapMode.Clamp;

        jigsawPiece.SetTexture(texture);

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

    private float percentToValue(float percentage, float total)
    {
        return (percentage * total / 100);
    }

    private float valueToPercent(float value, float total)
    {
        return (value * 100 / total);
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

    private void Shuffle(List<HandleJigsawaPiece> pieces)
    {
        for (int i = 0; i < pieces.Count; i++)
        {
            HandleJigsawaPiece temp = pieces[i];
            int randomIndex = UnityEngine.Random.Range(i, pieces.Count);

            pieces[i] = pieces[randomIndex];

            pieces[randomIndex] = temp;
        }

        puzzleShapes = pieces;
    }

    private void PrepareTexture()
    {
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

    private void ComputePuzzleProperties()
    {
         this.puzzleProperties.Init(this.puzzleRatio, this.puzzleDifficulty, this.puzzleOrientation);
    }
}