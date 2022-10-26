using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawRayWall : MonoBehaviour
{
    public Vector3 normal;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnDrawGizmos()
    {
        if(normal != null)Gizmos.DrawRay(this.transform.position, normal * 10);
    }
}
