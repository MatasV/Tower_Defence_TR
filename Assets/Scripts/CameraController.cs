using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Windows;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class CameraController : MonoBehaviour
{
    #region old
    /*
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
    }*/
    #endregion
    [SerializeField] private float camMovementSpeed;
    [SerializeField] private float camRotationSpeed;
    [SerializeField] private float _smoothing;
    [SerializeField] private float camZoomSpeed;
    [SerializeField] private Vector2 _zoomBounds;
    [SerializeField] private Vector2 _cameraBounds;



    private Vector3 _camPos;
    private Vector3 _motionInput;
    private float _camAngleX;
    private float _camAngleY;
    private float _currentAngleX;
    private float _currentAngleY;

    private void Awake()
    {
        _camAngleY = transform.eulerAngles.y;
        _currentAngleX = _camAngleY;
        _camAngleX= transform.eulerAngles.x;
        _currentAngleX = _camAngleX;

    }

    private void Update()
    {
        HandleInput();
        HandleCameraRotation();
        HandleCameraZoom();
        HandleCameraMovement();
    }

    private void HandleInput()
    {
        float x = UnityEngine.Input.GetAxisRaw("Horizontal");
        float z = UnityEngine.Input.GetAxisRaw("Vertical");

        Vector3 right = transform.right * x;
        Vector3 forward = transform.forward * z;

        _motionInput = (forward + right).normalized;

        if (!UnityEngine.Input.GetMouseButton(2)) return;
        {
            _camAngleY += UnityEngine.Input.GetAxisRaw("Mouse X") * camRotationSpeed;
            _camAngleX -= UnityEngine.Input.GetAxisRaw("Mouse Y") * camRotationSpeed;

            _camAngleX = Mathf.Clamp(_camAngleX, -90f, 90f);
        }
    }

    private void HandleCameraRotation()
    {
        _currentAngleY = Mathf.Lerp(_currentAngleY, _camAngleY, Time.deltaTime * _smoothing);
        _currentAngleX = Mathf.Lerp(_currentAngleX, _camAngleX, Time.deltaTime * _smoothing);

        transform.rotation = Quaternion.Euler(_currentAngleX, _currentAngleY, 0f);
    }

    private void HandleCameraZoom()
    {
        float zoomInput = UnityEngine.Input.GetAxis("Mouse ScrollWheel");
        transform.Translate(Vector3.forward * zoomInput * camZoomSpeed);
        _camPos = transform.position;

        if (!ZoomIsInBounds(_camPos))
        {
            transform.position = _camPos.normalized * _zoomBounds.y;
        }
    }

    private bool ZoomIsInBounds(Vector3 position)
    {
        return position.magnitude > _zoomBounds.x && position.magnitude < _zoomBounds.y;
    }

    private bool CameraIsInBounds(Vector3 position)
    {
        return position.x > -_cameraBounds.x &&
               position.x < _cameraBounds.x &&
               position.z > -_cameraBounds.y &&
               position.z < _cameraBounds.y;
    }

    private void HandleCameraMovement()
    {
        Vector3 nextTargetPosition = _camPos + _motionInput * camMovementSpeed;
        if (CameraIsInBounds(nextTargetPosition)) _camPos = nextTargetPosition;
        transform.position = Vector3.Lerp(transform.position, _camPos, Time.deltaTime * _smoothing);
    }
}
