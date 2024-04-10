using UnityEngine;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class CameraController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 30f;
    [SerializeField] private Vector2 moveLimit; // x ir y

    [SerializeField] private float scrollSpeed = 5f;
    [SerializeField] private float minY = 3f;
    [SerializeField] private float maxY = 5f;

    [SerializeField] private float rotationSpeed = 100f;

    private void Update()
    {
        Vector3 newPos = CalculateMovement();
        newPos.y -= CalculateScroll() * scrollSpeed * Time.deltaTime;
        newPos = ClampMovement(newPos);
        transform.position = newPos;
        Rotate();
    }

    private Vector3 ClampMovement(Vector3 position)
    {
        Vector3 newPos = position;
        newPos.x = Mathf.Clamp(position.x, -moveLimit.x, moveLimit.x);
        newPos.y = Mathf.Clamp(position.y, minY, maxY);
        newPos.z = Mathf.Clamp(position.z, -moveLimit.y, moveLimit.y);
        return newPos;
    }

    private float CalculateScroll()
    {
        return Input.GetAxis("Mouse ScrollWheel");
    }

    private Vector3 CalculateMovement()
    {
        Vector3 forward = transform.forward;
        forward.y = 0f;
        forward.Normalize();
        Vector3 right = transform.right;
        right.y = 0f;
        right.Normalize();
        
        Vector3 moveDirection = Vector3.zero;

        if (Input.GetKey(KeyCode.W))
        {
            moveDirection += forward;
        }
        if (Input.GetKey(KeyCode.S))
        {
            moveDirection -= forward;
        }
        if (Input.GetKey(KeyCode.D))
        {
            moveDirection += right;
        }
        if (Input.GetKey(KeyCode.A))
        {
            moveDirection -= right;
        }

        Vector3 newPos = transform.position + moveDirection * moveSpeed * Time.deltaTime;
        return newPos;
    }

    private void Rotate()
    {
        if (Input.GetKey(KeyCode.Q))
        {
            transform.Rotate(Vector3.up, -rotationSpeed * Time.deltaTime, Space.World);
        }
        if (Input.GetKey(KeyCode.E))
        {
            transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.World);
        }
    }
}
