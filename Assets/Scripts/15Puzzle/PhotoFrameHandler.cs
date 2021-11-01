using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhotoFrameHandler : MonoBehaviour
{
    public float speed = 1.0f;
    public Transform targetMarker;

    private bool isActive = false;
    private float startTime;
    private float positionTravelLength;
    private float rotationTravelLength;
    private bool isLerping = false;

    private Vector3 startMarkerPosition;
    private Quaternion startMarkerRotation;

    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
        startMarkerPosition = transform.position;
        startMarkerRotation = transform.rotation;
    }

    void Update()
    {
        if (isLerping)
        {
            float distCovered = (Time.time - startTime) * speed;

            float fractionOfPositionJourney = distCovered / positionTravelLength;
            float fractionOfRotationJourney = distCovered / rotationTravelLength;

            Vector3 targetPosition = isActive ? targetMarker.position : startMarkerPosition;
            Quaternion targetRotation = isActive ? targetMarker.rotation : startMarkerRotation;

            transform.position = Vector3.Lerp(transform.position, targetPosition, fractionOfPositionJourney);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, fractionOfRotationJourney);

            if (Vector3.Distance(transform.position, targetPosition) < 1.0f
                && Quaternion.Angle(transform.rotation, targetRotation) < 1.0f)
            {
                isLerping = false;

                if (isActive)
                {
                    animator.Play("Photo_Enveloppe_Shown");
                }
            }
        }
    }

    void OnMouseEnter()
    {
        Debug.Log("OnMouseEnter");
    }

    void OnMouseDown()
    {
        TogglePhoto(!isActive);
    }

    void TogglePhoto(bool _isActive)
    {
        isActive = _isActive;
        isLerping = true;
        startTime = Time.time;

        if (!isActive)
        {
            animator.Play("Photo_Enveloppe_Hidden");
        }

        Vector3 targetPosition = isActive ? targetMarker.position : startMarkerPosition;
        Quaternion targetRotation = isActive ? targetMarker.rotation : startMarkerRotation;

        positionTravelLength = Vector3.Distance(transform.position, targetPosition);
        rotationTravelLength = Quaternion.Angle(transform.rotation, targetRotation);
    }
}
