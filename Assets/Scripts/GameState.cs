using UnityEngine;
using System.Text.RegularExpressions;

public class GameState : MonoBehaviour
{
    public bool isReady = false;
    public bool isWin = false;
    public int gameMoves = 0;
    public TextMesh gameMovesTextMesh;

    private string gameMovesText;

    private void Start()
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