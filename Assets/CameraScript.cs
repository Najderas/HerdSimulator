using UnityEngine;
using System.Collections;
using System;

public class CameraScript : MonoBehaviour {

    private float cameraSpeed = 2.5f;
    private float zoomSpeed = 1.7f;
    private Camera camera;

    // Use this for initialization
    void Start ()
    {

        //camera = this.camera;

    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKey(KeyCode.RightArrow))
        {
            transform.Translate(new Vector3(cameraSpeed * Time.deltaTime, 0, 0));
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            transform.Translate(new Vector2(-cameraSpeed * Time.deltaTime, 0));
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            transform.Translate(new Vector2(0, -cameraSpeed * Time.deltaTime));
        }
        if (Input.GetKey(KeyCode.UpArrow))
        {
            transform.Translate(new Vector2(0, cameraSpeed * Time.deltaTime));
        }

        Camera.main.orthographicSize -=  Input.GetAxis("Mouse ScrollWheel") * zoomSpeed ;


	}
}
