using UnityEngine;

public class FlatCamera : MonoBehaviour
{
    [SerializeField] private float _speed;
    [SerializeField] private float _rotationSpeed;
    private float yRotation;

    private void Start()
    {
        yRotation = transform.rotation.eulerAngles.y;
    }

    private void Update()
    {
        // Rotation
        if(Input.GetKey(KeyCode.E))
        {
            yRotation -= _rotationSpeed * Time.deltaTime;
        }
        if(Input.GetKey(KeyCode.Q))
        {
            yRotation += _rotationSpeed * Time.deltaTime;
        }

        Quaternion cameraRotation = Quaternion.Euler(
            new Vector3(transform.rotation.eulerAngles.x,
            yRotation,
            transform.rotation.eulerAngles.z));
        transform.rotation = cameraRotation;

        // Pan Movement
        if(Input.GetKey(KeyCode.W))
        {
            Vector3 forward = transform.forward;
            forward.y = 0f;
            forward.Normalize();
            transform.position += forward * _speed * Time.deltaTime;
        }
        if(Input.GetKey(KeyCode.S))
        {
            Vector3 backwards = -transform.forward;
            backwards.y = 0f;
            backwards.Normalize();
            transform.position += backwards * _speed * Time.deltaTime;
        }
        if(Input.GetKey(KeyCode.A))
        {
            Vector3 left = -transform.right;
            left.y = 0f;
            left.Normalize();
            transform.position += left * _speed * Time.deltaTime;
        }
        if(Input.GetKey(KeyCode.D))
        {
            Vector3 right = transform.right;
            right.y = 0f;
            right.Normalize();
            transform.position += right * _speed * Time.deltaTime;
        }

        // Up and Down
        if(Input.GetKey(KeyCode.R))
        {
            transform.position += Vector3.up * _speed * Time.deltaTime;
        }
        if(Input.GetKey(KeyCode.F))
        {
            transform.position += Vector3.down * _speed * Time.deltaTime;
        }

    }

    public void SetFocus(Vector3 focusPoint, float yRotation = 180f)
    {
        Debug.Log($"FOcus Point {focusPoint}");
        Vector3 offset = new Vector3(0, 7, -3);
        Quaternion rotationAngle = Quaternion.Euler(new Vector3(transform.rotation.eulerAngles.x, yRotation, transform.rotation.eulerAngles.z));
        Vector3 rotatedVector = rotationAngle * offset;
        transform.position = focusPoint + rotatedVector;
        transform.LookAt(focusPoint);
    }
}
