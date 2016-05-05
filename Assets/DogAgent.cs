using UnityEngine;
using System.Collections;

public class DogAgent : MonoBehaviour
{

    public float Speed = 1;

    private Rigidbody2D rb;

    // Use this for initialization
    void Start () {
        rb = GetComponent<Rigidbody2D>();
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    void FixedUpdate()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        //var _dest = transform.position + new Vector3(moveHorizontal, moveVertical);
        var _dest = new Vector3(moveHorizontal, moveVertical);

        rb.velocity = _dest;
        //transform.position = Vector3.MoveTowards(transform.position, _dest, Speed * Time.deltaTime);

        transform.rotation = Quaternion.LookRotation(Vector3.forward, _dest);
    }
}
