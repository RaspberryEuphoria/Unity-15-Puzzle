using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_PlayHandler : MonoBehaviour
{
    public Camera gameCamera;
    public Transform gameSelectionTransform;

    void OnMouseDown()
    {
        this.gameCamera.GetComponent<CameraHandler>().StartTravelling(gameSelectionTransform);
    }
}
