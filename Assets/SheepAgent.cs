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

    private readonly List<GameObject> _neighbourSheeps = new List<GameObject>();
    public float Speed;
    private float _fleeSpeed;
    private float _flockSpeed;
    private Vector3 _dest;
    private SheepState _currentState;

    private void Start()
    {
        _currentState = SheepState.Wander;
        _dest = GetRadomness();
        _fleeSpeed = 3 * Speed;
        _flockSpeed = 0.8F * Speed;
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
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Sheep"))
        {
            _neighbourSheeps.Add(other.gameObject);
        }
        else if (other.CompareTag("Border"))
        {
            _dest.x = transform.position.x -_dest.x;
            _dest.y = transform.position.y - _dest.y;
            AdjustRotation();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Sheep"))
        {
            _neighbourSheeps.Remove(other.gameObject);
        }
    }

    private void ApplyWander()
    {
        if (Math.Abs(Vector3.Distance(transform.position, _dest)) < 0.2F)
        {
            _dest = GetRadomness();
            AdjustRotation();
        }

        transform.position = Vector3.MoveTowards(transform.position, _dest, Speed * Time.deltaTime);
    }

    private void ApplyFlee()
    {
    }

    private void ApplyHungry()
    {
    }

    private void ApplyFlock()
    {
    }

    private Vector3 GetCohesion()
    {
        return new Vector3();
    }

    private Vector3 GetAlignment()
    {
        return new Vector3();
    }

    private Vector3 GetSeparation()
    {
        return new Vector3();
    }

    private Vector3 GetFlee()
    {
        return new Vector3();
    }

    private Vector3 GetRadomness()
    {
        var randomV = new Vector3(Random.Range(-0.7F, 0.7F), Random.Range(-0.7F, 0.7F));
        return transform.position + randomV;
    }

    private void AdjustRotation()
    {
        transform.rotation = Quaternion.LookRotation(Vector3.forward, _dest - transform.position);
    }
}
