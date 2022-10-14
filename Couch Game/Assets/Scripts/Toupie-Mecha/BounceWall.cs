using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BounceWall : MonoBehaviour
{
    public Vector3 playerDirection;
    public Vector3 normalizedWall;

    void OnEnable()
    {
        this.GetComponent<SpinnerControler>().moveDir = Vector3.Reflect(playerDirection, normalizedWall);
        
        Debug.Log("REFLECT");
        this.enabled = false;
    }
}
