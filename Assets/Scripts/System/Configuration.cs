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
    [Tooltip("Minimal values for x, y, and z positions.")]
    public Transform minLimit;
    [Tooltip("Maximal values for x, y, and z positions.")]
    public Transform maxLimit;
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
