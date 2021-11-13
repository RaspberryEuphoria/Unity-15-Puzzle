using UnityEngine;
using UnityEngine.InputSystem;

public class CameraHandler : MonoBehaviour
{
    public float travellingSpeed = 1.0f;
    public float roamingSpeed = 1.0f;

    private float startTime;
    private Transform target;
    private bool isLerping = false;
    private float positionTravelLength;
    private float rotationTravelLength;
    private CameraMode cameraMode;

    private System.Action callback;
    private PlayerInput playerInput;

    private void Start()
    {
        playerInput = GetComponent<PlayerInput>();
    }

    private void Update()
    {
        if (isLerping)
        {
            float distCovered = (Time.time - startTime) * travellingSpeed;

            float fractionOfPositionJourney = distCovered / positionTravelLength;
            float fractionOfRotationJourney = distCovered / rotationTravelLength;

            // Use the same value to ensure both position and rotation changes take about the same duration
            float journey = Mathf.Min(fractionOfPositionJourney, fractionOfRotationJourney);

            transform.position = Vector3.Lerp(transform.position, target.position, journey);
            transform.rotation = Quaternion.Lerp(transform.rotation, target.rotation, journey);

            if (Vector3.Distance(transform.position, target.position) < 1.0f
                && Quaternion.Angle(transform.rotation, target.rotation) < 1.0f)
            {
                isLerping = false;

                callback?.Invoke();
            }
        }
    }

    private void FixedUpdate()
    {
        // Zoom => transform.position += cameraSpeed * Time.deltaTime * transform.forward;

        if (cameraMode == CameraMode.Roaming)
        {
            Keyboard keyboard = Keyboard.current;

            if (keyboard.wKey.IsPressed())
            {
                transform.position += roamingSpeed * Time.deltaTime * transform.up;
            }

            if (keyboard.dKey.IsPressed())
            {
                transform.position += roamingSpeed * Time.deltaTime * transform.right;
            }

            if (keyboard.sKey.IsPressed())
            {
                transform.position -= roamingSpeed * Time.deltaTime * transform.up;
            }

            if (keyboard.aKey.IsPressed())
            {
                transform.position -= roamingSpeed * Time.deltaTime * transform.right;
            }
        }
    }

    public void SetCameraMode(CameraMode cameraMode)
    {
        this.cameraMode = cameraMode;
    }

    public void StartTravelling(Transform newTarget, System.Action newCallback = null)
    {
        target = newTarget;
        callback = newCallback;

        startTime = Time.time;

        positionTravelLength = Vector3.Distance(transform.position, target.position);
        rotationTravelLength = Quaternion.Angle(transform.rotation, target.rotation);
        isLerping = true;
    }
}
