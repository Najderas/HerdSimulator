using System.Collections.Generic;
using System.Linq;
using Assets;
using UnityEngine;

public class Shepherd : MonoBehaviour
{
    private HashSet<GameObject> _sheeps;
    public HashSet<Flock> Flocks;
    public bool DebugDraw = true;

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
