using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }


    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There's more than one InputManager! " + transform + " - " + Instance);
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }


    public Vector2 GetMouseSecreenPosition()
    {
        return Mouse.current.position.ReadValue();
    }

    public bool IsMouseButtonDown()
    {
        return Mouse.current.leftButton.wasPressedThisFrame;
    }

    public Vector2 GetCameraMoveVector()
    {
        Vector2 inputMoveDir = new Vector3(0, 0);
        //This supposed to be inputMoveDir.z instead of y but we will translate it back on camera controller
        if (Keyboard.current.wKey.ReadValue() > 0) //Hold input pretty much
        {
            inputMoveDir.y = +1f;
        }
        if (Keyboard.current.sKey.ReadValue() > 0)
        {
            inputMoveDir.y = -1f;
        }
        if (Keyboard.current.aKey.ReadValue() > 0)
        {
            inputMoveDir.x = -1f;
        }
        if (Keyboard.current.dKey.ReadValue() > 0)
        {
            inputMoveDir.x = +1f;
        }

        return inputMoveDir;
    }

    public float GetCameraRotateAmount()
    {
        float rotateAmount = 0f;

        if (Keyboard.current.qKey.ReadValue() > 0)
        {
            rotateAmount = +1f;
        }
        if (Keyboard.current.eKey.ReadValue() > 0)
        {
            rotateAmount = -1f;
        }

        return rotateAmount;
    }

    public float GetCameraZoomAmount()
    {
        float zoomAmount = Mouse.current.scroll.ReadValue().y;

        return zoomAmount;
    }
}
