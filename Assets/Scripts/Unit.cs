using UnityEngine;
using UnityEngine.InputSystem;

public class Unit : MonoBehaviour
{
    private Vector3 targetPosition;
    [SerializeField] private float moveSpeed = 4f;

    void Update()
    {
        float stoppingDistance = 0.1f;
        //Stops the unit from moving if it is within the stopping distance of the target position
        if (Vector3.Distance(transform.position, targetPosition) > stoppingDistance)
        {
            Vector3 moveDirection = (targetPosition - transform.position).normalized;
            transform.position += moveDirection * moveSpeed * Time.deltaTime;

        }
        if (Keyboard.current.tKey.wasPressedThisFrame)
        {
            Move(new Vector3(Random.Range(-5f, 5f), 0, Random.Range(-5f, 5f)));
        }
    }

    private void Move(Vector3 newPosition)
    {
        targetPosition = newPosition;
    }
}
