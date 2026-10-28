using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    [SerializeField] private bool invert;
    private Transform cameraTransform;

    private void Awake()
    {
        cameraTransform = Camera.main.transform;
    }

    //Always run after the camera update, so when the camera move, this follows
    private void LateUpdate()
    {
        if (invert)
        {
            //Without this, the UI is inverted. Make sure to set true to the bool invert
            Vector3 dirToCamera = (cameraTransform.position - transform.position).normalized;
            transform.LookAt(transform.position + dirToCamera * -1);
        }
        else
        {
            transform.LookAt(cameraTransform);
        }
    }
}
