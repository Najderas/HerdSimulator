using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class SheepAgent : MonoBehaviour
{
    public enum SheepState
    {
        Flee,
        Hungry,
        Flock,
        Wander
    }

    private readonly float _mapWidth = 17.1f;
    private readonly float _mapHeight = 8.1f;

    public float BasicSpeed;
    private float _flockSpeed;
    private float _fleeSpeed;

    private readonly HashSet<GameObject> _neighbourSheeps = new HashSet<GameObject>();
    private readonly HashSet<GameObject> _neighbourDogs = new HashSet<GameObject>();

    private Vector3 _dest;
    private SheepState _currentState;
    private float _currentSpeed;

    private void Start()
    {
        _flockSpeed = 0.8f*BasicSpeed;
        _fleeSpeed = 4.5f*BasicSpeed;
        SetState(SheepState.Wander);
        _dest = GetRadomness();
    }

    private void FixedUpdate()
    {
        switch (_currentState)
        {
            case SheepState.Flee:
                ApplyFlee();
                break;

            case SheepState.Flock:
                ApplyFlock();
                break;

            case SheepState.Hungry:
                ApplyHungry();
                break;

            case  SheepState.Wander:
                ApplyWander();
                break;
        }

        switch (_currentState)
        {
            case SheepState.Wander:
            case SheepState.Flee:
            case SheepState.Flock:

                if (Vector3.Distance(transform.position, _dest) > 0.2)
                {
                    AdjustRotation();
                    MakeMove();
                }

                break;

            case SheepState.Hungry:
                if (Vector3.Distance(transform.position, _dest) < 0.2)
                {
                    _dest = GetRadomness();
                    AdjustRotation();
                }
                break;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Sheep"))
        {
            _neighbourSheeps.Add(other.gameObject);
        }
        else if (other.CompareTag("Dog"))
        {
            _neighbourDogs.Add(other.gameObject);
            SetState(SheepState.Flee);
        }
        else if (other.CompareTag("Border"))
        {
            _dest = transform.position + 1.2f * (transform.position - _dest);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Sheep"))
        {
            _neighbourSheeps.Remove(other.gameObject);
            if (_neighbourSheeps.Count < 3)
            {
                SetState(SheepState.Wander);
            }
        }
        else if (other.CompareTag("Dog"))
        {
            _neighbourDogs.Remove(other.gameObject);
            if (_neighbourDogs.Count == 0)
            {
                SetState(SheepState.Wander);
            }
        }
    }

    private void ApplyWander()
    {
        if (_neighbourSheeps.Count >= 3)
        {
            SetState(SheepState.Flock);
            ApplyFlock();
            return;
        }

        if (Math.Abs(Vector3.Distance(transform.position, _dest)) < 0.2f)
        {
            _dest = 0.8f * GetRadomness() + 0.2f * GetSeparation();
        }

        if (Random.Range(0f, 1f) > 0.99f)
        {
            SetState(SheepState.Hungry);
        }
    }

    private void ApplyFlee()
    {
        _dest = 0.4f * GetFlee() + 0.55f * GetAlignment() + 0.05f * GetSeparation();
    }

    private void ApplyHungry()
    {
        if (Random.Range(0f, 1f) > 0.99f)
        {
            SetState(SheepState.Wander);
        }
    }

    private void ApplyFlock()
    {
        _dest = 0.85f * GetCohesion() + 0.1f * GetSeparation() + 0.05f * GetAlignment();

        if (Random.Range(0f, 1f) > 0.8f)
        {
            SetState(SheepState.Wander);
        }
    }

    private Vector3 GetCohesion()
    {
        var coh = new Vector3();
        foreach (var sheep in _neighbourSheeps)
        {
            coh += sheep.transform.position;
        }

        return coh/_neighbourSheeps.Count;
    }

    private Vector3 GetAlignment()
    {
        if (_neighbourSheeps.Count == 0) return transform.position;

        var strength = 0.5f;
        var ali = new Vector3();
        foreach (var sheep in _neighbourSheeps)
        {
            var comp = sheep.GetComponent<SheepAgent>();
            var pDest = comp._dest;
            var p = sheep.transform.position;

            var diff = pDest - p;
            var distanceInv = InverslyScale(diff, strength);

            ali += new Vector3(diff.normalized.x * distanceInv, diff.normalized.y * distanceInv);
        }
        var result = new Vector3(ali.x/_neighbourSheeps.Count, ali.y/_neighbourSheeps.Count);
        return result + transform.position;
    }

    private Vector3 GetSeparation()
    {
        if (_neighbourSheeps.Count == 0) return transform.position;

        var strength = 1.2f;
        var sep = new Vector3(0, 0, 0);
        foreach (var sheep in _neighbourSheeps)
        {
            var p = sheep.transform.position;
            var diff = transform.position - p;
            var distanceInv = InverslyScale(diff, strength);

            sep += new Vector3(diff.normalized.x * distanceInv, diff.normalized.y * distanceInv);
        }

        return sep + transform.position;
    }

    private Vector3 GetFlee()
    {
        if (_neighbourDogs.Count == 0) return transform.position;

        var strength = 3f;
        var flee = new Vector3(0, 0, 0);
        foreach (var dog in _neighbourDogs)
        {
            var p = dog.transform.position;
            var diff = transform.position - p;
            var distanceInv = InverslyScale(diff, strength);

            flee += new Vector3(diff.normalized.x * distanceInv, diff.normalized.y * distanceInv);
        }

        return flee + transform.position;
    }

    private Vector3 GetRadomness()
    {
        var radius = 1f;
        var randomV = new Vector3(Random.Range(-radius, radius), Random.Range(-radius, radius));
        return transform.position + randomV;
    }

    private void MakeMove()
    {
//        if (_currentState == SheepState.Flock) Debug.DrawLine(transform.position, _dest, Color.red);
//        if (_currentState == SheepState.Wander) Debug.DrawLine(transform.position, _dest, Color.blue);
//        if (_currentState == SheepState.Hungry) Debug.DrawLine(transform.position, _dest, Color.magenta);
        if (_dest.x > _mapWidth || _dest.x < -_mapWidth || _dest.y > _mapHeight || _dest.y < -_mapHeight)
        {
            _dest = transform.position;
        }

        transform.position = Vector3.MoveTowards(transform.position, _dest, _currentSpeed * Time.deltaTime);
    }

    private void AdjustRotation()
    {
        transform.rotation = Quaternion.LookRotation(Vector3.forward, _dest - transform.position);
    }

    private float InverslyScale(Vector3 v, float strength)
    {
        var len = v.magnitude;
        if (Math.Abs(len) < 0.05f)
        {
            return strength;
        }

        return strength / len;
    }

    public void SetState(SheepState state)
    {
        _currentState = state;

        switch (state)
        {
                case SheepState.Flee:
                    _currentSpeed = _fleeSpeed;
                    break;

                case SheepState.Wander:
                    _currentSpeed = _flockSpeed;
                    break;

                case SheepState.Flock:
                case SheepState.Hungry:
                    _currentSpeed = _flockSpeed;
                    break;
        }
    }
}
