using UnityEngine;
using UnityEngine.EventSystems;

public class UI_PlayHandler : MonoBehaviour, IPointerClickHandler
{
    public Camera gameCamera;
    public Transform gameSelectionTransform;

    public void OnPointerClick(PointerEventData eventData)
    {
        gameCamera.GetComponent<CameraHandler>().StartTravelling(gameSelectionTransform);
    }
}
