using UnityEngine;

public class PreparationCamera : MonoBehaviour
{
    [SerializeField] private float _speed;
    [SerializeField] private float _rotationSpeed;
    [SerializeField] private Vector2 _cameraRotation;
    [SerializeField] private Transform _cameraTarget;
    [SerializeField] private LayerMask _collisionMask;

    Vector3 moveDirection;

    private void Awake()
    {
        _cameraRotation = new Vector2(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y);
    }

    void Update()
    {
        Vector2 input = new Vector2(-Input.GetAxisRaw("Mouse Y"), Input.GetAxisRaw("Mouse X"));
        _cameraRotation += input * _rotationSpeed * Time.deltaTime; 
        _cameraRotation.x = Mathf.Clamp(_cameraRotation.x, -52f, 70f);
        Quaternion cameraRotationInQuaternion = Quaternion.Euler(_cameraRotation);
        transform.rotation = cameraRotationInQuaternion;

        // Zoom In/Out
        //    if(Input.GetAxis("Mouse ScrollWheel") > 0f)
        //    {
        //        transform.Translate(Vector3.forward * 12 * _speed * Time.deltaTime);
        //    }
        //    if(Input.GetAxis("Mouse ScrollWheel") < 0f)
        //    {
        //        transform.Translate(Vector3.back * 12 * _speed * Time.deltaTime);
        //    }
        //    

        moveDirection = Vector3.zero;

        // Pan Movement
        if (Input.GetKey(KeyCode.W))
       {
            moveDirection += transform.forward;
       }
       if(Input.GetKey(KeyCode.S))
       {
            moveDirection -= transform.forward;
       }
       if(Input.GetKey(KeyCode.A))
       {
            moveDirection -= transform.right;
       }
       if(Input.GetKey(KeyCode.D))
       {
            moveDirection += transform.right;
       }
       if(Input.GetKey(KeyCode.E))
       {
            moveDirection += transform.up;
       }
       if(Input.GetKey(KeyCode.Q))
       {
            moveDirection -= transform.up;
       }
       
       moveDirection = moveDirection.normalized;

       Vector3 newPosition = transform.position + moveDirection * _speed * Time.deltaTime;
        
        if (!Physics.Raycast(transform.position, moveDirection, 0.48f, _collisionMask))
        {
            transform.position = newPosition;
        }
        else
        {
            // Se colidir, ajustar a posição da câmera para ficar perto do objeto colidido
            transform.position = transform.position;
        }
    }
    
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        // Gizmos.DrawLine(transform.position, transform.position + moveDirection);
        Gizmos.DrawRay(transform.position, moveDirection);
    }
    
}
