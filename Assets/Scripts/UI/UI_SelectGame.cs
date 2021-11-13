using UnityEngine;
using UnityEngine.EventSystems;

public class UI_SelectGame : MonoBehaviour, IPointerClickHandler
{
    public LaunchGame launcher;
    public GameObject gameToLaunch;

    public void OnPointerClick(PointerEventData eventData)
    {
        launcher.StartNewGame(gameToLaunch.name);
    }
}
