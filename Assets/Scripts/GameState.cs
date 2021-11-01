using UnityEngine;

public class GameState : MonoBehaviour
{
    public TextMesh gameMovesTextMesh;
    public GameObject fiftenPuzzleObject;
    public Camera gameCamera;
    public Transform tableCameraMarker;

    [HideInInspector]
    public bool isReady = false;
    [HideInInspector]
    public bool isWin = false;

    private int gameMoves = 0;
    private string gameMovesText;

    void Start()
    {
        gameMovesText = gameMovesTextMesh.text;
        UpdateGameMovesText();
    }

    public void IncrementGameMoves()
    {
        gameMoves++;
        UpdateGameMovesText();
    }

    public void UpdateGameMovesText()
    {
        gameMovesTextMesh.text = gameMovesText.Replace("{{value}}", gameMoves.ToString());
    }

    public void SetVictory()
    {
        isWin = true;
    }

    public void SetIsReady()
    {
        isReady = true;
    }
}