using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BounceDebug : MonoBehaviour
{
    Transform playerTransform;
    public LayerMask layer;

    void Start()
    {
        playerTransform = this.transform;
        Debug.Log("coucou");
    }

    void FixedUpdate() 
    {
        Debug.DrawRay(playerTransform.position, playerTransform.forward * 10f, Color.white);
        //Debug.DrawLine(playerTransform.position, playerTransform.forward, Color.white);
        RaycastHit hit;
        if (Physics.Raycast(playerTransform.position, playerTransform.forward, out hit, Mathf.Infinity, layer))
        {
            //Debug.Log("AAAAAAAAAAAAAAAAAAAAA");
            Vector3 bounceDir = Vector3.Reflect(playerTransform.forward * 10f, hit.normal);
            Debug.DrawRay(hit.point, bounceDir, Color.red);

            Debug.DrawRay(hit.point, hit.normal * 3f, Color.blue);
        }
    }
}
