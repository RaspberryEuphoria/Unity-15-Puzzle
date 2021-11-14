using UnityEngine;
using UnityEngine.InputSystem;

public class CameraHandler : MonoBehaviour
{
    public float travellingSpeed = 10f;
    public float roamingSpeed = 25f;
    public float zoomingSpeed = 75f;

    private float startTime;
    private Transform target;
    private bool isLerping = false;
    private float positionTravelLength;
    private float rotationTravelLength;
    private CameraMode cameraMode;

    private System.Action callback;
    private Borders borders;

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
   

    private void OnCollisionEnter(Collision collision)
    {
        if (borders == null)
        {
            return;
        }

        borders.CheckCollision(collision.collider, true);
    }

    private void OnCollisionExit(Collision collision)
    {
        if (borders == null)
        {
            return;
        }

        borders.CheckCollision(collision.collider, false);
    }

    private void FixedUpdate()
    {
        if (cameraMode == CameraMode.Roaming)
        {
            Keyboard keyboard = Keyboard.current;

            if (!borders.top.isHit && keyboard.wKey.IsPressed())
            {
                transform.position += roamingSpeed * Time.deltaTime * transform.up;
            }

            if (!borders.right.isHit && keyboard.dKey.IsPressed())
            {
                transform.position += roamingSpeed * Time.deltaTime * transform.right;
            }

            if (!borders.bottom.isHit && keyboard.sKey.IsPressed())
            {
                transform.position -= roamingSpeed * Time.deltaTime * transform.up;
            }

            if (!borders.left.isHit && keyboard.aKey.IsPressed())
            {
                transform.position -= roamingSpeed * Time.deltaTime * transform.right;
            }
            
        }

        Mouse mouse = Mouse.current;

        if (mouse.scroll.IsPressed())
        {
            Vector2 axis = mouse.scroll.ReadValue();

            if (axis.y > 0)
            {
                transform.position += zoomingSpeed * Time.deltaTime * transform.forward;
            }
            else
            {
                transform.position -= zoomingSpeed * Time.deltaTime * transform.forward;
            }
        }
    }

    public void SetCameraMode(CameraMode cameraMode)
    {
        this.cameraMode = cameraMode;
    }

    public void SetBorders(Borders borders)
    {
        this.borders = borders;
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
