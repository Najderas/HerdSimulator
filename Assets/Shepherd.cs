using System.Collections.Generic;
using System.Linq;
using Assets;
using UnityEngine;

public class Shepherd : MonoBehaviour
{
    private HashSet<GameObject> _sheeps;
    private HashSet<Flock> _flocks;

    public void RegisterSheep(GameObject sheep)
    {
        _sheeps.Add(sheep);
    }

    public void InitSheep()
    {
        _flocks = new HashSet<Flock>();
        _sheeps = new HashSet<GameObject>();
    }

    private void FixedUpdate()
    {
        _flocks.Clear();
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

                _flocks.Add(flock);
            }
        }
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
