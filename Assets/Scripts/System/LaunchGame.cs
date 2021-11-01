using System.Collections.Generic;
using UnityEngine;

public class LaunchGame : MonoBehaviour
{
    public List<GameObject> games;
    public Camera gameCamera;

    private GameState gameState;

    void Start()
    {
        gameState = GameObject.Find("GameState").GetComponent<GameState>();
    }

    void Update()
    {

    }

    public void StartNewGame(string gameName)
    {
        GameObject newGame = games.Find(g => g.name == gameName);

        Configuration newGameConfiguration = newGame.GetComponent<Configuration>();

        if (newGameConfiguration.allowCustomTexture)
        {
            StartCoroutine(LoadCustomTexture.GetCustomTexture(customTexture => { 
                newGameConfiguration.SetCustomTexture(customTexture);

                ActivateGame(newGame, newGameConfiguration);
            }));

            return;
        }


        ActivateGame(newGame, newGameConfiguration);
    }

    void ActivateGame(GameObject game, Configuration gameConfiguration)
    {
        game.SetActive(true);

        Transform newGameCameraTransform = gameConfiguration.cameraTransform;
        CameraHandler cameraHandler = gameCamera.GetComponent<CameraHandler>();

        cameraHandler.StartTravelling(newGameCameraTransform, () => {
            game.GetComponent<LoadBoard>().enabled = true;
        });
    }
}
