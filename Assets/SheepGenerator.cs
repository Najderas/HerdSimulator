using System.Collections.Generic;
using UnityEngine;

public class SheepGenerator : MonoBehaviour
{
    private IList<GameObject> _sheeps;
    public GameObject SheepTemplate;
    public int SheepsNumber;
    private readonly int _mapWidth = 17;
    private readonly int _mapHeight = 8;

    // Use this for initialization
    private void Start()
    {
        Random.seed = 2;
        _sheeps = new List<GameObject>();
        GenerateSheeps(GeneratePositions(SheepsNumber));
    }

    private IList<Vector3> GeneratePositions(int number)
    {
        var sheepPositions = new List<Vector3>();
        var generated = 0;
        var splitRate = number/20f;

        do
        {
            var flockSize = Random.Range(1, Mathf.Ceil((number - generated) / splitRate));
            var center = GetRandomVector();
            for (var i = 0; i < flockSize; i++)
            {
                Vector3 position;
                do
                {
                    position = GetRandomVectorFromCenter(center, flockSize);
                } while (position.x > _mapWidth || position.x < -_mapWidth || position.y > _mapHeight || position.y < -_mapHeight);
                sheepPositions.Add(position);
                generated += 1;
            }
        } while (number != generated);

        return sheepPositions;
    }

    private Vector3 GetRandomVector()
    {
        var x = (Random.value * 2 * _mapWidth) - _mapWidth;
        var y = (Random.value * 2 * _mapHeight) - _mapHeight;
        return new Vector3(x, y);
    }

    private Vector3 GetRandomVectorFromCenter(Vector3 center, float flockSize)
    {
        var radius = 1f * Mathf.Ceil(flockSize/10f);
        var v = Random.insideUnitCircle;
        var x = (v.x * 2 * radius) - radius + center.x;
        var y = (v.y * 2 * radius) - radius + center.y;
        return new Vector3(x, y);
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

    public void SetSeed(int seed)
    {
        Random.seed = seed;
    }
}
