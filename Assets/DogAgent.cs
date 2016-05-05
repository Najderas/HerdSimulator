using UnityEngine;

public class DogAgent : MonoBehaviour
{
    private Vector3 _dest;

    private void Start()
    {
        _dest = GetRadomness();
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        if (Vector3.Distance(transform.position, _dest)  <0.2)
        {
            _dest = GetRadomness();
            transform.rotation = Quaternion.LookRotation(Vector3.forward, _dest - transform.position);
        }
        transform.position = Vector3.MoveTowards(transform.position, _dest, 2F * Time.deltaTime);
    }

    private Vector3 GetRadomness()
    {
        var randomV = new Vector3(Random.Range(-1.2F, 1.2F), Random.Range(-1.2F, 1.2F));
        return transform.position + randomV;
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
}
