using UnityEngine;

public class EnemyCharacter : MonoBehaviour
{
    public Vector3 targetPosition { get; private set; } = Vector3.zero;
    private float _velocityMagnitude = 0;
    private void Start()
    {
        targetPosition = transform.position;
    }
    private void Update()
    {
        if (_velocityMagnitude > 0.1f)
        {
            float maxDistance = _velocityMagnitude * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, maxDistance);
        }
        else
        {
            transform.position = targetPosition;
        }

    }

    //public void SetPosition(Vector3 position)
    //{
    //    transform.position = position;
    //}

    public void SetMovement(in Vector3 position, in Vector3 velocity, in float averageInterval)
    {
        targetPosition = position + (velocity * averageInterval);
        _velocityMagnitude = velocity.magnitude;
    }
}
