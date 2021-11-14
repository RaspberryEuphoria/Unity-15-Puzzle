using UnityEngine;

public enum CameraMode
{
    Fixed,
    Roaming
}

[System.Serializable]
public class Border
{
    public BoxCollider collider;
    public bool isHit = false;
}

[System.Serializable]
public class Borders
{
    public Border top;
    public Border right;
    public Border left;
    public Border bottom;

    public void CheckCollision(Collider collider, bool isHit)
    {
        if (collider == top.collider)
        {
            top.isHit = isHit;
        }

        if (collider == right.collider)
        {
            right.isHit = isHit;
        }

        if (collider == left.collider)
        {
            left.isHit = isHit;
        }

        if (collider == bottom.collider)
        {
            bottom.isHit = isHit;
        }
    }
}

[System.Serializable]
public class Views
{
    [Tooltip("Default view when starting a game.")]
    public Transform starting;
    [Tooltip("Four colliders to represent camera limits.")]
    public Borders borders;
}

public class Configuration : MonoBehaviour
{
    [Header("Camera configuration")]
    public Views views;
    public Camera gameCamera;
    public CameraMode cameraMode = CameraMode.Fixed;

    [Header("Textures configuration")]
    public Texture2D customTexture;
    public bool allowTextureSelection = false;

    [Header("Game loader")]
    public MonoBehaviour loader;

    public void SetCustomTexture(Texture2D newCustomTexture)
    {
        customTexture = newCustomTexture;
    }
}
