using System;
using System.Collections.Generic;
using Assets;
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

    private readonly int MinFlockSize = 3;
//
//    private readonly float _mapWidth = 17.1f;
//    private readonly float _mapHeight = 8.1f;

    public float BasicSpeed = 0.3f;
    private float _flockSpeed;
    private float _fleeSpeed;

    public readonly HashSet<GameObject> NeighbourSheeps = new HashSet<GameObject>();
    private readonly HashSet<GameObject> _neighbourDogs = new HashSet<GameObject>();

    private Vector3 _dest;
    private SheepState _currentState;
    private float _currentSpeed;

    private int _cooldown;

    protected Parameters FeParams;
    protected Parameters FkParams;
    protected Parameters WrParams;

    protected float FlockLeaveProbability;
    protected int MaxCooldown;

    public SheepAgent()
    {
        FeParams = new Parameters()
        {
            FleeCoefficient = 0.4f,
            AlignmentCoefficient = 0.55f,
            SeparationCoefficient = 0.05f
        };

        FkParams = new Parameters()
        {
            CohesionCoefficient = 0.85f,
            AlignmentCoefficient = 0.05f,
            SeparationCoefficient = 0.1f
        };

        WrParams = new Parameters()
        {
            RadomnessCoefficient = 0.6f,
            SeparationCoefficient = 0.4f
        };

        MaxCooldown = 500;
        FlockLeaveProbability = 0.9999f;
    }

    private void Start()
    {
        _flockSpeed = 0.8f*BasicSpeed;
        _fleeSpeed = 5f*BasicSpeed;
        SetState(SheepState.Wander);
        _dest = GetRadomness();
        _cooldown = 0;
    }

    private void FixedUpdate()
    {
//        if (name == "57")
//        {
//            Debug.Log(_currentState);
//        }
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
//                if (name == "57") Debug.Log(IsAtTarget());
                if (!IsAtTarget())
                {
                    AdjustRotation();
                    MakeMove();
//                    if (name == "57") Debug.Log("move");
                }

                break;

            case SheepState.Hungry:
                if (IsAtTarget())
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
            NeighbourSheeps.Add(other.gameObject);
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

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Dog"))
        {
            SetState(SheepState.Flee);
//            if (name == "57")
//            {
//                Debug.Log("dogleft");
//            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Sheep"))
        {
            NeighbourSheeps.Remove(other.gameObject);
            if (NeighbourSheeps.Count < MinFlockSize)
            {
                if (_currentState != SheepState.Flee)
                {
                    SetState(SheepState.Wander);
//                    if (name == "57")
//                    {
//                        Debug.Log("wandeer");
//                    }
                }
            }
        }
        else if (other.CompareTag("Dog"))
        {
            _neighbourDogs.Remove(other.gameObject);
            if (_neighbourDogs.Count == 0)
            {
                SetState(SheepState.Wander);
//                if (name == "57")
//                {
//                    Debug.Log("dogleft");
//                }
            }
        }
    }

    private void ApplyWander()
    {
        if (_cooldown > 0) _cooldown -= 1;
        if (NeighbourSheeps.Count >= MinFlockSize && _cooldown == 0)
        {
            SetState(SheepState.Flock);
            ApplyFlock();
            return;
        }

        if (IsAtTarget())
        {
            _dest = transform.position + WrParams.RadomnessCoefficient * GetRadomness() + WrParams.SeparationCoefficient * GetSeparation();
        }

        if (Random.Range(0f, 1f) > 0.99f)
        {
            SetState(SheepState.Hungry);
        }
    }

    private void ApplyFlee()
    {
        _dest = transform.position + FeParams.AlignmentCoefficient * GetAlignment() + FeParams.FleeCoefficient * GetFlee() + FeParams.SeparationCoefficient * GetSeparation();
//        Debug.DrawLine(transform.position, _dest, Color.black);
//        Debug.DrawLine(transform.position, transform.position + FeParams.AlignmentCoefficient * GetAlignment(), Color.white);
//        Debug.DrawLine(transform.position, transform.position + FeParams.FleeCoefficient * GetFlee(), Color.red);
//        Debug.DrawLine(transform.position, transform.position + FeParams.SeparationCoefficient * GetSeparation(), Color.blue);
    }

    private void ApplyHungry()
    {
        if (Random.Range(0f, 1f) > 0.999f)
        {
            SetState(SheepState.Wander);
        }
    }

    private void ApplyFlock()
    {
        _dest = transform.position + FkParams.CohesionCoefficient * GetCohesion() + FkParams.SeparationCoefficient * GetSeparation() + FkParams.AlignmentCoefficient * GetAlignment();

        if (Random.Range(0f, 1f) > FlockLeaveProbability)
        {
            _cooldown = MaxCooldown;
            SetState(SheepState.Wander);
        }
    }

    private Vector3 GetCohesion()
    {
        var coh = new Vector3();
        foreach (var sheep in NeighbourSheeps)
        {
            coh += sheep.transform.position - transform.position;
        }

        return coh/NeighbourSheeps.Count;
    }

    private Vector3 GetAlignment()
    {
        if (NeighbourSheeps.Count == 0) return new Vector3();

        var strength = 0.5f;
        var ali = new Vector3();
        foreach (var sheep in NeighbourSheeps)
        {
            var comp = sheep.GetComponent<SheepAgent>();
            var pDest = comp._dest;
            var p = sheep.transform.position;

            var diff = pDest - p;
            var distanceInv = InverslyScale(diff, strength);

            ali += new Vector3(diff.normalized.x * distanceInv, diff.normalized.y * distanceInv);
        }
        var result = new Vector3(ali.x/NeighbourSheeps.Count, ali.y/NeighbourSheeps.Count);
        return result;
    }

    private Vector3 GetSeparation()
    {
        if (NeighbourSheeps.Count == 0) return new Vector3();

        var strength = 1.2f;
        var sep = new Vector3(0, 0, 0);
        foreach (var sheep in NeighbourSheeps)
        {
            var p = sheep.transform.position;
            var diff = transform.position - p;
            var distanceInv = InverslyScale(diff, strength);

            sep += new Vector3(diff.normalized.x * distanceInv, diff.normalized.y * distanceInv);
        }

        return sep;
    }

    private Vector3 GetFlee()
    {
        if (_neighbourDogs.Count == 0) return new Vector3();

        var strength = 3f;
        var flee = new Vector3(0, 0, 0);
        foreach (var dog in _neighbourDogs)
        {
            var p = dog.transform.position;
            var diff = transform.position - p;
            var distanceInv = InverslyScale(diff, strength);

            flee += new Vector3(diff.normalized.x * distanceInv, diff.normalized.y * distanceInv);
        }

        return flee;
    }

    private Vector3 GetRadomness()
    {
        var radius = 1.5f;
        var randomV = new Vector3(Random.Range(-radius, radius), Random.Range(-radius, radius));
        return randomV;
    }

    private void MakeMove()
    {
//        if (_currentState == SheepState.Flock) Debug.DrawLine(transform.position, _dest, Color.red);
//        if (_currentState == SheepState.Wander) Debug.DrawLine(transform.position, _dest, Color.blue);
//        if (_currentState == SheepState.Hungry) Debug.DrawLine(transform.position, _dest, Color.magenta);
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

    private bool IsAtTarget()
    {
        return Math.Abs(Vector3.Distance(transform.position, _dest)) < 0.2f;
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
