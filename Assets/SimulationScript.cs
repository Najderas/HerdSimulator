using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class SimulationScript : MonoBehaviour
{
    //public GuiScript _guiScript;
    public SheepGenerator sheepGenerator;

    struct SimulationVariableParameters
    {
        public int sheepNumber;
        public int flocksNumber;
        public float outlierToLightRatio;
    }

    private List<SimulationVariableParameters> parametersList;

    private float simulationSpeed = 1;
    public float SimulationSpeed
    {
        get { return simulationSpeed; }
        set
        {
            simulationSpeed = value;
            Time.timeScale = value;
        }
    }
    public int sheepNumber = 50;
    public int flockNumber = 5;
    public float outlierToLightRatio = 0.1f;
    //public int SheepNumber {get { return sheepNumber; } set { SheepNumber = value; } }
    public bool generateHerdedSheeps = false;
    public bool resetSeedWhileRestart = false;
    public int randomSeed = DateTime.Now.GetHashCode();

    public bool startSimulation = false;

    //public bool StartSimulation
    //{
    //    get {return startSimulation;}
    //    set { if (!runMultipleSimulations) startSimulation = value; }
    //}

    private bool runMultipleSimulations = false;
    public bool RunMultipleSimulations {
        get { return runMultipleSimulations; }
        set {  }
    }
    

    // Use this for initialization
    void Start () {
        //_guiScript = transform.GetComponent<GuiScript>();
        GenerateSimulationParameters();
    }
	
	// Update is called once per frame
	void Update () {
	    if (runMultipleSimulations)
	    {
            if (sheepGenerator._shepherd.OneFlockLeft())
            {
                nextSimulationEnded();
                StartNextSimulation();
            }
        }
	    
	}

    public void StartMultipleSimulations()
    {
        if (runMultipleSimulations) return;
        runMultipleSimulations = true;
        // init file
        StartNextSimulation();
    }

    void StartNextSimulation()
    {
        var param = parametersList.FirstOrDefault();
        parametersList.Remove(param);
        sheepNumber = param.sheepNumber;
        flockNumber = param.flocksNumber;
        outlierToLightRatio = param.outlierToLightRatio;
        // TODO: write down them to file
        print("Next params - sheeps: " + param.sheepNumber + ", flocks: " + param.flocksNumber + ", ratio: " + param.outlierToLightRatio);
        startSimulation = true;
    }

    void nextSimulationEnded()
    {
        //TODO: write down time consumed
    }

    void GenerateSimulationParameters()
    {
        parametersList = new List<SimulationVariableParameters>();  
        for (int sheeps = 20; sheeps < 200; sheeps += 10)
        {
            for (int flocks = 2; flocks < sheeps/4; flocks++)
            {
                for (float ratio = 0.05f; ratio < 0.7f; ratio+=0.05f)
                {
                    parametersList.Add(new SimulationVariableParameters { sheepNumber = sheeps, flocksNumber = flocks, outlierToLightRatio = ratio });
                }
            }
        }
    }

}
