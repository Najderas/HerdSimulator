using System.Collections.Generic;
using UnityEngine;

public class SheepGenerator : MonoBehaviour
{
    private IList<GameObject> _sheeps;
    public GameObject SheepTemplate;
    public int SheepsNumber;

    // Use this for initialization
    private void Start()
    {
        _sheeps = new List<GameObject>();
        GenerateSheeps(GeneratePositions(SheepsNumber));
    }

    private IList<Vector3> GeneratePositions(int number)
    {
        var positions = new List<Vector3>();

        for (var i = 0; i < number; i++)
        {
            var x = Random.Range(-4, 4);
            var y = Random.Range(-4, 4);
            var position = new Vector3(x, y);
            positions.Add(position);
        }

        return positions;
    }

    private void GenerateSheeps(IList<Vector3> positions)
    {
        foreach (var position in positions)
        {
            var rotation = Quaternion.Euler(0, 0, Random.Range(0, 180));
            var sheep = Instantiate(SheepTemplate, position, rotation) as GameObject;
            if (sheep == null) continue;
            sheep.transform.SetParent(transform);
            _sheeps.Add(sheep);
        }
    }
}
