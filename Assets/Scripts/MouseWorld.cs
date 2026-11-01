using UnityEngine;
using UnityEngine.InputSystem;

public class MouseWorld : MonoBehaviour
{
    private static MouseWorld instance;

    //Specify which layer this raycast will hit
    [SerializeField] private LayerMask mousePlaneLayerMask;

    private void Awake()
    {
        instance = this;
    }

    // For Visual, dont forget to enable the sphere
    /*private void Update()
    {
        transform.position = MouseWorld.GetPosition();
    }*/

    public static Vector3 GetPosition()
    {
        //Raycast detect all colliders in the scene and return true if it hits something
        Ray ray = Camera.main.ScreenPointToRay(InputManager.Instance.GetMouseSecreenPosition());
        Physics.Raycast(ray, out RaycastHit raycastHit, float.MaxValue, instance.mousePlaneLayerMask);
        return raycastHit.point;
    }
}
