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

    public float BasicSpeed;
    private float FlockSpeed;
    private float FleeSpeed;

    private readonly HashSet<GameObject> _neighbourSheeps = new HashSet<GameObject>();
    private readonly HashSet<GameObject> _neighbourDogs = new HashSet<GameObject>();
    private Vector3 _dest;
    private SheepState _currentState;
    private float _currentSpeed;

    private void Start()
    {
        FlockSpeed = 0.8F*BasicSpeed;
        FleeSpeed = 4.5F*BasicSpeed;
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
            _dest.x = transform.position.x -_dest.x;
            _dest.y = transform.position.y - _dest.y;
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

        if (Math.Abs(Vector3.Distance(transform.position, _dest)) < 0.2F)
        {
            _dest = GetRadomness() + GetSeparation();
        }

        if (Random.Range(0F, 1F) > 0.99)
        {
            SetState(SheepState.Hungry);
        }
    }

    private void ApplyFlee()
    {
        _dest = GetFlee();
    }

    private void ApplyHungry()
    {
        if (Random.Range(0F, 1F) > 0.99)
        {
            SetState(SheepState.Wander);
        }
    }

    private void ApplyFlock()
    {
        _dest = 0.8F * GetCohesion() + 0.2F * GetSeparation();

        if (Random.Range(0F, 1F) > 0.9995)
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
        return new Vector3();
    }

    private Vector3 GetSeparation()
    {
        var sep = new Vector3(0, 0, 0);
        foreach (var sheep in _neighbourSheeps)
        {
            var p = sheep.transform.position;
            var diff = transform.position - p;
            var len = diff.magnitude;
            float distanceInv;
            if (Math.Abs(len) < 0.05F)
            {
                distanceInv = 1.1F;
            }
            else
            {
                distanceInv = 1.1F/ diff.magnitude;
            }
            sep += new Vector3(diff.normalized.x * distanceInv, diff.normalized.y * distanceInv);
        }

        return sep;
    }

    private Vector3 GetFlee()
    {
        var flee = new Vector3(0, 0, 0);
        foreach (var dog in _neighbourDogs)
        {
            var p = dog.transform.position;
            var diff = transform.position - p;
            var len = diff.magnitude;
            float distanceInv;
            if (Math.Abs(len) < 0.05F)
            {
                distanceInv = 3F;
            }
            else
            {
                distanceInv = 3F / diff.magnitude;
            }
            flee += new Vector3(diff.normalized.x * distanceInv, diff.normalized.y * distanceInv);
        }

        return flee;
    }

    private Vector3 GetRadomness()
    {
        var randomV = new Vector3(Random.Range(-0.7F, 0.7F), Random.Range(-0.7F, 0.7F));
        return transform.position + randomV;
    }

    private void MakeMove()
    {
        transform.position = Vector3.MoveTowards(transform.position, _dest, _currentSpeed * Time.deltaTime);
    }

    private void AdjustRotation()
    {
        transform.rotation = Quaternion.LookRotation(Vector3.forward, _dest - transform.position);
    }

    public void SetState(SheepState state)
    {
        _currentState = state;

        switch (state)
        {
                case SheepState.Flee:
                    _currentSpeed = FleeSpeed;
                    break;

                case SheepState.Wander:
                    _currentSpeed = FlockSpeed;
                    break;

                case SheepState.Flock:
                case SheepState.Hungry:
                    _currentSpeed = FlockSpeed;
                    break;
        }
    }
}
