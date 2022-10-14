using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BounceWall : MonoBehaviour
{
    public Vector3 playerDirection;
    public Vector3 normalizedWall;
    public SpinnerControler spinerControler;
    void OnEnable()
    {
        Debug.Log("A " + spinerControler.moveDir);
        spinerControler.moveDir = Vector3.Reflect(playerDirection, normalizedWall);
        spinerControler.walled = true;
        Debug.Log("B " +spinerControler.moveDir);
        
        this.enabled = false;
    }
}
