using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BouncePlayer : MonoBehaviour
{
    public GameObject explosion;

    void OnEnable()
    {
        Instantiate(explosion, this.transform.position, Quaternion.identity);
        this.enabled = false;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
