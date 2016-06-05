using UnityEngine;

public class DogAgent : MonoBehaviour
{
    private Vector3 _dest;

//    private void Start()
//    {
//        _dest = new Vector3(20, 7);
//    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        transform.rotation = Quaternion.LookRotation(Vector3.forward, _dest - transform.position);
        transform.position = Vector3.MoveTowards(transform.position, _dest, 1.5f * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Border"))
        {
            _dest.x = transform.position.x - _dest.x;
            _dest.y = transform.position.y - _dest.y;
            transform.rotation = Quaternion.LookRotation(Vector3.forward, _dest - transform.position);
        }
    }

    public void SetTarget(Vector3 target)
    {
        _dest = target;
    }
}
