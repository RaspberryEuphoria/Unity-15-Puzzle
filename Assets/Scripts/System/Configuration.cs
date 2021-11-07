using UnityEngine;

public class Configuration : MonoBehaviour
{
    public Transform cameraTransform;
    public bool allowCustomTexture = false;
    public Camera gameCamera;
    public Texture2D customTexture;
    public MonoBehaviour loader;

    public void SetCustomTexture(Texture2D newCustomTexture)
    {
        customTexture = newCustomTexture;
    }
}
