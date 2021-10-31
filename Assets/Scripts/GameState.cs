using UnityEngine;

public class GameState : MonoBehaviour
{
    public TextMesh gameMovesTextMesh;
    public GameObject fiftenPuzzleObject;
    public Camera gameCamera;
    public Transform tableCameraMarker;

    [HideInInspector]
    public bool isReady = false;
    [HideInInspector]
    public bool isWin = false;

    private int gameMoves = 0;
    private string gameMovesText;

    // Cameras
    public float cameraSpeed = 1.0f;
    private float startTime;
    private Vector3 startMarkerPosition;
    private Quaternion startMarkerRotation;
    private bool isLerping = false;
    private bool isActive = false;
    private float positionTravelLength;
    private float rotationTravelLength;

    void Start()
    {
        startMarkerPosition = gameCamera.transform.position;
        startMarkerRotation = gameCamera.transform.rotation;

        gameMovesText = gameMovesTextMesh.text;
        UpdateGameMovesText();
    }

    void Update()
    {
        if (isLerping)
        {
            float distCovered = (Time.time - startTime) * cameraSpeed;

            float fractionOfPositionJourney = distCovered / positionTravelLength;
            float fractionOfRotationJourney = distCovered / rotationTravelLength;

            Vector3 targetPosition = isActive ? tableCameraMarker.position : startMarkerPosition;
            Quaternion targetRotation = isActive ? tableCameraMarker.rotation : startMarkerRotation;

            gameCamera.transform.position = Vector3.Lerp(gameCamera.transform.position, targetPosition, fractionOfPositionJourney);
            gameCamera.transform.rotation = Quaternion.Lerp(gameCamera.transform.rotation, targetRotation, fractionOfRotationJourney);

            if (Vector3.Distance(gameCamera.transform.position, targetPosition) < 1.0f
                && Quaternion.Angle(gameCamera.transform.rotation, targetRotation) < 1.0f)
            {
                isLerping = false;
            }
        }
    }

    public void IncrementGameMoves()
    {
        gameMoves++;
        UpdateGameMovesText();
    }

    public void UpdateGameMovesText()
    {
        gameMovesTextMesh.text = gameMovesText.Replace("{{value}}", gameMoves.ToString());
    }

    public void SetVictory()
    {
        isWin = true;
    }

    public void SetIsReady()
    {
        isReady = true;
    }

    public void StartNewGame(string gameName)
    {
        isActive = true;
        isLerping = true;

        if (gameName == "15Puzzle")
        {
            fiftenPuzzleObject.SetActive(true);
        }

        Vector3 targetPosition = isActive ? tableCameraMarker.position : startMarkerPosition;
        Quaternion targetRotation = isActive ? tableCameraMarker.rotation : startMarkerRotation;

        positionTravelLength = Vector3.Distance(gameCamera.transform.position, targetPosition);
        rotationTravelLength = Quaternion.Angle(gameCamera.transform.rotation, targetRotation);
    }
}