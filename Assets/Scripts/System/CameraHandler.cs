using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraHandler : MonoBehaviour
{
    public float cameraSpeed = 1.0f;

    private float startTime;
    private Transform target;
    private bool isLerping = false;
    private float positionTravelLength;
    private float rotationTravelLength;

    private System.Action callback;

    void Update()
    {
        if (isLerping)
        {
            float distCovered = (Time.time - startTime) * cameraSpeed;

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
