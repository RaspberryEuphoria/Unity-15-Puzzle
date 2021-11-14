using UnityEngine;

public enum CameraMode
{
    Fixed,
    Roaming
}

[System.Serializable]
public class Views
{
    [Tooltip("Default view when starting a game.")]
    public Transform starting;
    [Tooltip("A gameobject with four colliders to represent camera limits.")]
    public GameObject borders;
}

public class Configuration : MonoBehaviour
{
    public Views views;
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
