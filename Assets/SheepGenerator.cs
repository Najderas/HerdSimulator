using System.Collections.Generic;
using UnityEngine;

public class SheepGenerator : MonoBehaviour
{
    public GameObject WhiteSheepTemplate;
    public GameObject BlackSheepTemplate;
    public GameObject RedSheepTemplate;
    public GameObject BlueSheepTemplate;
    public GuiScript guiScript;
    public SimulationScript simulationScript;

    public int SheepsNumber
    {
        get { return simulationScript.sheepNumber; }
        set { simulationScript.sheepNumber = value; }
    }
    
    private readonly int _mapWidth = 17;
    private readonly int _mapHeight = 8;
    public Shepherd _shepherd;

    // Use this for initialization
    private void Start()
    {
        _shepherd = transform.GetComponent<Shepherd>();
        _shepherd.InitSheep();
        Random.seed = simulationScript.randomSeed;

    }

    void Update()
    {
        if (simulationScript.startSimulation)
        {
            StartSimulation();
            simulationScript.startSimulation = false;
        }
        
    }

    public void StartSimulation()
    {
        StopSimulation();
        if (simulationScript.resetSeedWhileRestart)
            Random.seed = simulationScript.randomSeed;
        GenerateSheeps(GeneratePositions(SheepsNumber));
    }

    public void StopSimulation()
    {
        _shepherd.DeleteAllSheeps();
    }

    private IList<Vector3> GeneratePositions(int number)
    {
        var sheepPositions = new List<Vector3>();
        var generated = 0;
        var splitRate = Mathf.Ceil(number/20f);

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
        var radius = 0.6f * Mathf.Ceil(flockSize/10f);
        var v = Random.insideUnitCircle;
        var x = (v.x * 2 * radius) - radius + center.x;
        var y = (v.y * 2 * radius) - radius + center.y;
        return new Vector3(x, y);
    }

    private void GenerateSheeps(IList<Vector3> positions)
    {
        var count = 0;
        foreach (var position in positions)
        {
            var rotation = Quaternion.Euler(0, 0, Random.Range(0, 180));
            GameObject sheep;
            //TODO: parametrize this constant
            if (Random.Range(0f, 1f) > 0.9f)
            {
                sheep = Instantiate(RedSheepTemplate, position, rotation) as GameObject;
            }
            else
            {
                sheep = Instantiate(WhiteSheepTemplate, position, rotation) as GameObject;
            }

            if (sheep == null) continue;
            sheep.transform.SetParent(transform);
            sheep.name = string.Format("{0}", count);
            count += 1;
            _shepherd.RegisterSheep(sheep);
        }
    }
    
}
