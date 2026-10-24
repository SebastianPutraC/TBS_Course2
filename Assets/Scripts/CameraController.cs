using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class CameraController : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 100f;
    [SerializeField] private float moveSpeed = 10f;

    private const float MIN_FOLLOW_Y_OFFSET = 2f;
    private const float MAX_FOLLOW_Y_OFFSET = 12f;


    [SerializeField] private CinemachineCamera cinemachineCamera;
    [SerializeField] private CinemachineFollow follow;

    [SerializeField] private float zoomAmount = 1f;
    [SerializeField] private float zoomSpeed = 5f;

    private float targetFollowY;

    private void Start()
    {
        targetFollowY = follow.FollowOffset.y;
    }

    private void Update()
    {
        HandleMovement();
        HandleRotation();
        HandleZoom();
    }
    private void HandleMovement()
    {
        Vector3 inputMoveDir = new Vector3(0, 0, 0);
        if (Keyboard.current.wKey.ReadValue() > 0) //Hold input pretty much
        {
            inputMoveDir.z = +1f;
        }
        if (Keyboard.current.sKey.ReadValue() > 0)
        {
            inputMoveDir.z = -1f;
        }
        if (Keyboard.current.aKey.ReadValue() > 0)
        {
            inputMoveDir.x = -1f;
        }
        if (Keyboard.current.dKey.ReadValue() > 0)
        {
            inputMoveDir.x = +1f;
        }

        Vector3 moveVector = transform.forward * inputMoveDir.z + transform.right * inputMoveDir.x;
        transform.position += moveVector * moveSpeed * Time.deltaTime;
    }

    private void HandleRotation()
    {
        Vector3 rotationVector = new Vector3(0, 0, 0);

        if (Keyboard.current.qKey.ReadValue() > 0)
        {
            rotationVector.y = +1f;
        }
        if (Keyboard.current.eKey.ReadValue() > 0)
        {
            rotationVector.y = -1f;
        }

        transform.eulerAngles += rotationVector * rotationSpeed * Time.deltaTime;
    }
    private void HandleZoom()
    {
        float scroll = Mouse.current.scroll.ReadValue().y; // Either -1 (scroll down) or +1 (scroll up), and 0 for not doing anything
        targetFollowY -= scroll * zoomAmount; //-= because we want to zoom in when scrolling up, and out when scrolling down

        targetFollowY = Mathf.Clamp( //Clamp because we don't want the camera to go too high or too low
            targetFollowY,
            MIN_FOLLOW_Y_OFFSET,
            MAX_FOLLOW_Y_OFFSET
        );

        Vector3 followOffsetCopy = follow.FollowOffset;

        followOffsetCopy.y = Mathf.Lerp( // Lerp because we want to smoothly transition between the current follow offset and the target follow offset
            followOffsetCopy.y,
            targetFollowY,
            zoomSpeed * Time.deltaTime
        );

        follow.FollowOffset = followOffsetCopy; // Copy the result back to the follow offset
    }
}

  