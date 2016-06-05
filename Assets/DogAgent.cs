using UnityEngine;

public class DogAgent : MonoBehaviour
{
    public enum DogState
    {
        Wait,
        Approach,
        Lead
    }

    public DogState CurrentDogState;

    private const float SafeZone = 2f;
    private Vector3 _dest;
    private Vector3 _flockPosition;
    private GameObject _targetObject;
    private float _angle;
    private bool _sign;

    private Vector3 _steerPoint;

    private void Start()
    {
        CurrentDogState = DogState.Wait;
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        switch (CurrentDogState)
        {
                case DogState.Approach:
                    Approach();
                    break;

                case DogState.Lead:
                    Lead();
                    break;
        }

        transform.rotation = Quaternion.LookRotation(Vector3.forward, _dest - transform.position);
        transform.position = Vector3.MoveTowards(transform.position, _dest, 1.5f * Time.deltaTime);
    }

    private void Lead()
    {
        _steerPoint = _targetObject.transform.position - (_flockPosition - _targetObject.transform.position).normalized * 1.2f;

        if (HasMerged())
        {
            CurrentDogState = DogState.Wait;
        }

        _dest = _steerPoint;
//        Debug.DrawLine(transform.position, _dest, Color.cyan);
    }

    private void Approach()
    {
        _steerPoint = _targetObject.transform.position - (_flockPosition - _targetObject.transform.position).normalized * 1.2f;

        if (Vector3.Distance(transform.position, _targetObject.transform.position) <= SafeZone)
        {
            _dest = GoRound(SafeZone, _targetObject.transform.position);
//            Debug.DrawLine(transform.position, _steerPoint, Color.black);
//            Debug.DrawLine(transform.position, _dest, Color.yellow);

            if (Vector3.Distance(_steerPoint, _dest) < 1f && Vector3.Distance(transform.position, _steerPoint) < Vector3.Distance(transform.position, _targetObject.transform.position))
            {
                CurrentDogState = DogState.Lead;
            }
        }
        else
        {
            _dest = _steerPoint;
//            Debug.DrawLine(transform.position, _dest, Color.magenta);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Border"))
        {
            _dest.x = transform.position.x - _dest.x;
            _dest.y = transform.position.y - _dest.y;
            transform.rotation = Quaternion.LookRotation(Vector3.forward, _dest - transform.position);
        }
    }

    private Vector3 GoRound(float radius, Vector3 center)
    {
        var result = GetVectorOnCircle(center, radius, _angle);
        if (IsAtTarget(_dest))
        {
            if (_sign)
            {
                _angle = (_angle - 5) % 360;
            }
            else
            {
                _angle = (_angle + 5) % 360;
            }
        }
        return result;
    }

    private Vector3 GetVectorOnCircle(Vector3 center, float radius, float angle)
    {
        var x = center.x + radius * Mathf.Sin(angle * Mathf.Deg2Rad);
        var y = center.y + radius * Mathf.Cos(angle * Mathf.Deg2Rad);
        return new Vector3(x,y);
    }

    private bool IsAtTarget(Vector3 t)
    {
        return Vector3.Distance(t, transform.position) < 0.3;
    }

    private bool HasMerged()
    {
        return _targetObject.GetComponent<SheepAgent>().NeighbourSheeps.Count > 3;
    }

    public void SetTarget(GameObject targetObject, Vector3 flockPosition)
    {
        CurrentDogState = DogState.Approach;
        _targetObject = targetObject;
        _flockPosition = flockPosition;
        _angle = Angle360(new Vector3(0, 5), _targetObject.transform.position - transform.position);
        _steerPoint = _targetObject.transform.position - (_flockPosition - _targetObject.transform.position).normalized * 1.2f;

        _sign = GetSign(_steerPoint, transform.position, _targetObject.transform.position);
    }

    private static float Angle360(Vector3 v1, Vector3 v2)
    {
        var angle = Vector3.Angle(v1, v2);
        var sign = Mathf.Sign(Vector3.Dot(new Vector3(0, 0, 1), Vector3.Cross(v1, v2)));
        var signedAngle = angle * sign;
        return (signedAngle <= 0) ? 360 + signedAngle : signedAngle;
    }

    private bool GetSign(Vector3 v1, Vector3 v2, Vector3 center)
    {
        var angleFromCenter = Vector3.Angle(v1 - center, v2 - center);
        return 2 * Mathf.PI * (angleFromCenter / 360) > 1.57f;
    }
}
