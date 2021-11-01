using UnityEngine;

public class Configuration : MonoBehaviour
{
    public Transform cameraTransform;
    public bool allowCustomTexture = false;

    public Texture2D customTexture;

    public void SetCustomTexture(Texture2D newCustomTexture)
    {
        customTexture = newCustomTexture;
    }
}
