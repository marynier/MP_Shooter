using UnityEngine;

public class PlayerCharacter : MonoBehaviour
{
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private float _speed = 2f;
    [SerializeField] private Transform _head;
    [SerializeField] private Transform _cameraPoint;
    [SerializeField] private float _maxHeadAngle = 90;
    [SerializeField] private float _minHeadAngle = -90;
    [SerializeField] private float _jumpForce = 50f;
    private float _inputH;
    private float _inputV;
    private float _rotateY;
    private float _currentRotateX;
    private void Start()
    {
        Transform camera = Camera.main.transform;
        camera.parent = _cameraPoint;        
        camera.localPosition = Vector3.zero;
        camera.localRotation = Quaternion.identity;
    }

    public void SetInput(float h, float v, float rotateY)
    {
        _inputH = h;
        _inputV = v;
        _rotateY += rotateY;
    }
    private void FixedUpdate()
    {
        Move();
        RotateY();
    }
    private void Move()
    {
        //Vector3 direction = new Vector3(_inputH, 0, _inputV).normalized;
        //transform.position += direction * Time.deltaTime * _speed;

        Vector3 velocity = (transform.forward * _inputV + transform.right * _inputH).normalized * _speed;
        velocity.y = _rigidbody.linearVelocity.y;
        _rigidbody.linearVelocity = velocity;
    }
    private void RotateY()
    {
        _rigidbody.angularVelocity = new Vector3(0, _rotateY, 0);
        _rotateY = 0;
    }
    public void RotateX(float value)
    {
        _currentRotateX = Mathf.Clamp(_currentRotateX + value, _minHeadAngle, _maxHeadAngle);
        _head.localEulerAngles = new Vector3(_currentRotateX, 0, 0);
    }

    public void GetMoveInfo(out Vector3 position, out Vector3 velocity)
    {
        position = transform.position;
        velocity = _rigidbody.linearVelocity;
    }
    private bool _isFly = true;
    private void OnCollisionStay(Collision collision)
    {
       var contactPoints = collision.contacts;
        for (int i = 0; i < contactPoints.Length; i++)
        {
            if (contactPoints[i].normal.y > 0.45f) _isFly = false;
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        _isFly = true;
    }
    public void Jump()
    {
        if (_isFly) return;       
        _rigidbody.AddForce(0, _jumpForce, 0, ForceMode.VelocityChange);
    }
}
