using System.Collections.Generic;
using UnityEngine;

public class LaunchGame : MonoBehaviour
{
    public List<GameObject> games;

    public void StartNewGame(string gameName)
    {
        GameObject newGame = games.Find(g => g.name == gameName);
        Configuration newGameConfiguration = newGame.GetComponent<Configuration>();

        if (newGameConfiguration.allowTextureSelection)
        {
            StartCoroutine(LoadCustomTexture.GetCustomTexture(customTexture => { 
                newGameConfiguration.SetCustomTexture(customTexture);

                ActivateGame(newGameConfiguration);
            }));

            return;
        }


        ActivateGame(newGameConfiguration);
    }

    void ActivateGame(Configuration gameConfiguration)
    {
        gameConfiguration.loader.enabled = true;
    }
}
