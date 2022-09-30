using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallBounce : MonoBehaviour
{
    [SerializeField] private float forceMagnitude ;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        Rigidbody rigidbody = collision.collider.attachedRigidbody;
        Vector3 coll = collision.gameObject.transform.position;

        if (rigidbody != null)
        {
            Vector3 forceDirection = new Vector3 ( coll.x, coll.y, -coll.z);
            rigidbody.AddForce(forceDirection * forceMagnitude , ForceMode.Impulse);
        }
    }
}
