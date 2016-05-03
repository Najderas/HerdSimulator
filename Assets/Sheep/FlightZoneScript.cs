using UnityEngine;
using System.Collections;

public class FlightZoneScript : MonoBehaviour {

    public SheepAgent sheepAgent;

    // Use this for initialization
    void Start () {
        sheepAgent = transform.root.gameObject.GetComponent<SheepAgent>();
    }
	
	// Update is called once per frame
	void Update () {

    }

    void OnTriggerEnter(Collider other)
    {

    }

    void OnTriggerStay(Collider other)
    {
        
    }
}
