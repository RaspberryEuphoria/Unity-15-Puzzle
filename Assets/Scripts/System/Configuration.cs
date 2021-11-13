using UnityEngine;

public enum CameraMode
{
    Fixed,
    Roaming
}

public class Configuration : MonoBehaviour
{
    public Transform cameraTransform;
    public bool allowCustomTexture = false;
    public Camera gameCamera;
    public Texture2D customTexture;
    public MonoBehaviour loader;
    public CameraMode cameraMode = CameraMode.Fixed;

    public void SetCustomTexture(Texture2D newCustomTexture)
    {
        customTexture = newCustomTexture;
    }
}
