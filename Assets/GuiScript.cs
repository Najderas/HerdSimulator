using System;
using UnityEngine;
using System.Collections;

public class GuiScript : MonoBehaviour
{
    public SheepGenerator sheepGenerator;
    public SimulationScript simulationScript;

    private Rect windowRect = new Rect(10, 10, 240, 230);

    //private float simulationSpeed = 1;
    public float SimulationSpeed
    {
        get { return simulationScript.SimulationSpeed; }
        set { simulationScript.SimulationSpeed = value; }
    }
    public int SheepNumber
    {
        get { return simulationScript.sheepNumber; }
        set { simulationScript.sheepNumber = value; }
    }
    public bool GenerateHerdedSheeps {
        get { return simulationScript.generateHerdedSheeps; }
        set { simulationScript.generateHerdedSheeps = value; }
    }
    public bool ResetSeedWhileRestart
    {
        get { return simulationScript.resetSeedWhileRestart; }
        set { simulationScript.resetSeedWhileRestart = value; }
    }
    public int RandomSeed
    {
        get { return simulationScript.randomSeed; }
        set { simulationScript.randomSeed = value; }
    }

    public bool startSimulation
    {
        get { return simulationScript.startSimulation; }
        set { simulationScript.startSimulation = value; }
    }
    //public bool ResetSimulation { get; set; }
    private string pauseUnpauseString = "pause";

    void OnGUI()
    {
        windowRect = GUI.Window(0, windowRect, DoMyWindow, "Configure simulation");
    }

    void DoMyWindow(int windowID)
    {
        //if (GUI.Button(new Rect(10, 20, 100, 20), "Hello World"))
        //    print("Got a click");
        GUI.Label(new Rect(10, 20, 100, 20), "sim. speed: " + SimulationSpeed);
        SimulationSpeed = GUI.HorizontalSlider(new Rect(110, 20, 120, 20), SimulationSpeed, 0, 5);

        GUI.Label(new Rect(10, 40, 100, 20), "sheep number" );
        SheepNumber = int.Parse(GUI.TextField(new Rect(110, 40, 40, 20), SheepNumber.ToString()));

        //GenerateHerdedSheeps = GUI.Toggle(new Rect(10, 60, 200, 20), GenerateHerdedSheeps, "generate sheeps in herds");

        //GUI.Label(new Rect(10, 80, 100, 20), "herd seed:");
        ResetSeedWhileRestart = GUI.Toggle(new Rect(10, 80, 200, 20), ResetSeedWhileRestart, "reset seed while restart");
        RandomSeed = int.Parse(GUI.TextField(new Rect(10, 100, 120, 20), RandomSeed.ToString()));
        if (GUI.Button(new Rect(140, 100, 90, 20), "random seed")) RandomSeed = DateTime.Now.GetHashCode();


        if (GUI.Button(new Rect(10, 130, 120, 20), "play new sim"))
        {
            print("reset sim");
            play_simulation();
        }
        //if (GUI.Button(new Rect(80, 130, 70, 20), "play"))
        //{
        //    print("play sim");
        //    play_simulation();
        //}
        if (GUI.Button(new Rect(130, 130, 90, 20), pauseUnpauseString))
        {
            print("pause sim");
            pause_simulation();
        }

        if (GUI.Button(new Rect(10, 160, 200, 20), "start multiple simulations"))
        {
            simulationScript.StartMultipleSimulations();
        }

        GUI.Label(new Rect(10, 180, 80, 20), "Haki" );

    }

    //void reset_simulation()
    //{
        
    //}

    void play_simulation()
    {
        if (!simulationScript.RunMultipleSimulations)
        {
            startSimulation = true;
        }
        else print("Multiple simulations are running now");
    }

    private float _previousSpeed=1;
    void pause_simulation()
    {
        if (pauseUnpauseString.Equals("pause"))
        {
            _previousSpeed = SimulationSpeed;
            SimulationSpeed = 0;
            pauseUnpauseString = "unpause";
        }
        else
        {
            SimulationSpeed = _previousSpeed;
            pauseUnpauseString = "pause";
        }
    }

}
