using System;
using System.Collections.Generic;
using System.Linq;
using Assets;
using UnityEngine;

public class Shepherd : MonoBehaviour
{
    private HashSet<GameObject> _sheeps;
    public HashSet<Flock> Flocks;
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

    private void FixedUpdate()
    {
        Flocks.Clear();
        Flocks = SplitTooSmall(GetFlocksByNeighboursNumber());

        if (_dog.CurrentDogState == DogAgent.DogState.Wait)
        {
            var targetName = FindTarget();
            var targetObject = GameObject.Find(targetName);
            if (targetObject == null)
            {
                Debug.Log("Finished!");
                return;
            }
            var targetFlock = FindClosestFlock(targetObject.transform.position);
            _dog.SetTarget(targetObject, targetFlock);
            //_dog.CurrentDogState = DogAgent.DogState.Approach;
        }
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

    private Flock FindClosestFlock(Vector3 position)
    {
        var target = new Flock();
        var currentFlockDist = float.MaxValue;

        foreach (var flock in Flocks)
        {
            if (flock.GetSheeps().Count != 1)
            {
                var dist = Vector3.Distance(flock.GetCenter(), position);
                if (dist < currentFlockDist)
                {
                    target = flock;
                    currentFlockDist = dist;
                }
            }
        }

        if (Math.Abs(currentFlockDist - float.MaxValue) < 0.005f || target.GetSheeps().Count == 0)
        {
            target = Flocks.First(f => f.GetCenter() != position);
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

    public void DeleteAllSheeps()
    {
        Flocks.Clear();
        foreach (var sheep in _sheeps)
        {
            Destroy(sheep);
        }
        _sheeps.Clear();
    }

    public void StopDog()
    {
        _dog.CurrentDogState = DogAgent.DogState.Wait;
    }

    public bool OneFlockLeft()
    {
        return Flocks.Count == 1;
    }

}
