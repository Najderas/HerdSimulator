using System;
using UnityEngine;
using System.Collections;

public class GuiScript : MonoBehaviour
{
    private Rect windowRect = new Rect(10, 10, 240, 160);
    public float simulationSpeed = 1;
    public int sheepNumber = 50;
    private bool generateHerdedSheeps = false;
    private bool resetSeedWhileRestart = false;
    public int randomSeed = DateTime.Now.GetHashCode();
    void OnGUI()
    {
        windowRect = GUI.Window(0, windowRect, DoMyWindow, "Configure simulation");
    }
    void DoMyWindow(int windowID)
    {
        //if (GUI.Button(new Rect(10, 20, 100, 20), "Hello World"))
        //    print("Got a click");
        GUI.Label(new Rect(10, 20, 100, 20), "simul. speed");
        simulationSpeed = GUI.HorizontalSlider(new Rect(110, 20, 120, 20), simulationSpeed, 0, 3);

        GUI.Label(new Rect(10, 40, 100, 20), "sheep number" );
        sheepNumber = int.Parse(GUI.TextField(new Rect(110, 40, 40, 20), sheepNumber.ToString()));

        generateHerdedSheeps = GUI.Toggle(new Rect(10, 60, 200, 20), generateHerdedSheeps, "generate sheeps in herds");

        //GUI.Label(new Rect(10, 80, 100, 20), "herd seed:");
        resetSeedWhileRestart = GUI.Toggle(new Rect(10, 80, 200, 20), resetSeedWhileRestart, "reset seed while restart");
        randomSeed = int.Parse(GUI.TextField(new Rect(10, 100, 120, 20), randomSeed.ToString()));
        if (GUI.Button(new Rect(140, 100, 90, 20), "random seed")) randomSeed = DateTime.Now.GetHashCode();


        if (GUI.Button(new Rect(10, 130, 70, 20), "reset")) print("reset sim");
        if (GUI.Button(new Rect(80, 130, 70, 20), "play")) print("play sim");
        if (GUI.Button(new Rect(150, 130, 70, 20), "pause")) print("pause sim");

    }
}
