using System.Collections.Generic;
using System.Linq;
using Assets;
using UnityEngine;

public class Shepherd : MonoBehaviour
{
    private HashSet<GameObject> _sheeps;
    public HashSet<Flock> Flocks;
    public bool DebugDraw = true;
    private DogAgent _dog;

    private void Start()
    {
        _dog = GameObject.FindGameObjectWithTag("Dog").GetComponent<DogAgent>();
    }

    public void RegisterSheep(GameObject sheep)
    {
        _sheeps.Add(sheep);
    }

    public void InitSheep()
    {
        Flocks = new HashSet<Flock>();
        _sheeps = new HashSet<GameObject>();
    }

    private GameObject targetObject;
    private Vector3 targetFlock;

    private void FixedUpdate()
    {
        Flocks.Clear();
        Flocks = SplitTooSmall(GetFlocksByNeighboursNumber());

        if (targetObject == null || HasMerged() || IsAtTarget())
        {
            var targetName = FindTarget();
            targetObject = GameObject.Find(targetName);
            targetFlock = FindClosestFlock(targetObject.transform.position);
        }

//        Debug.Log(targetObject.name);
        var singlePos = targetObject.transform.position;
        var diff = (targetFlock - singlePos).normalized * 0.7f;
        _dog.SetTarget(singlePos - diff);
    }

    private bool IsAtTarget()
    {
        return Vector3.Distance(targetFlock, _dog.transform.position) < 0.3;
    }

    private bool HasMerged()
    {
        return targetObject.GetComponent<SheepAgent>().NeighbourSheeps.Count > 3;
    }

    private string FindTarget()
    {
        var targetName = string.Format("{0}", float.MaxValue);
        var currentDistance = float.MaxValue;

        foreach (var flock in Flocks)
        {
            if (flock.GetSheeps().Count == 1)
            {
                var single = flock.GetSheeps().First();
                var dist = Vector3.Distance(single.transform.position, _dog.transform.position);
                if (dist < currentDistance)
                {
                    targetName = single.name;
                    currentDistance = dist;
                }
            }
        }

        return targetName;
    }

    private Vector3 FindClosestFlock(Vector3 position)
    {
        var target = new Vector3(float.MaxValue, float.MaxValue);
        var currentFlockDist = float.MaxValue;

        foreach (var flock in Flocks)
        {
            if (flock.GetSheeps().Count != 1)
            {
                var dist = Vector3.Distance(flock.GetCenter(), position);
                if (dist < currentFlockDist)
                {
                    target = flock.GetCenter();
                    currentFlockDist = dist;
                }
            }
        }

        return target;
    }

    private HashSet<Flock> GetFlocksByNeighboursNumber()
    {
        var flocks = new HashSet<Flock>();
        var list = _sheeps.OrderByDescending(s => s.GetComponent<SheepAgent>().NeighbourSheeps.Count);
        var used = new HashSet<GameObject>();
        foreach (var target in list)
        {
            if (!used.Contains(target))
            {
                var flock = new Flock();
                flock.AddSheep(target);
                used.Add(target);
                foreach (var s in GetNeighboursForFlock(new HashSet<GameObject>(), target, flock))
                {
                    flock.AddSheep(s);
                    used.Add(s);
                }

                flocks.Add(flock);
            }
        }

        return flocks;
    }

    private HashSet<Flock> SplitTooSmall(HashSet<Flock> flocks)
    {
        var result = new HashSet<Flock>();
        foreach (var flock in flocks)
        {
            if (flock.GetSheeps().Count < 4)
            {
                foreach (var sheep in flock.GetSheeps())
                {
                    var newFlock = new Flock();
                    newFlock.AddSheep(sheep);
                    result.Add(newFlock);
                }
            }
            else
            {
                result.Add(flock);
            }
        }

        return result;
    }

    private HashSet<GameObject> GetNeighboursForFlock(HashSet<GameObject> result, GameObject sheep, Flock flock)
    {
        var neighbours = sheep.GetComponent<SheepAgent>().NeighbourSheeps;
        foreach (var neighbour in neighbours)
        {
            if (!flock.GetSheeps().Contains(neighbour) && !result.Contains(neighbour))
            {
                result.Add(neighbour);
                GetNeighboursForFlock(result, neighbour, flock);
            }
        }
        return result;
    }
}
