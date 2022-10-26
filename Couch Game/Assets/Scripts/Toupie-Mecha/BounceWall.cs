using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BounceWall : MonoBehaviour
{
    public Vector3 playerDirection;
    public Vector3 normalizedWall;
    public SpinnerControler spinerControler;
    public Vector3 playerRotation;
    void OnEnable()
    {
        //Debug.Log("A " + spinerControler.moveDir);
        spinerControler.moveDir = Vector3.Reflect(playerDirection, normalizedWall);
        playerRotation = spinerControler.GetComponentInParent<PlayerManager>().transform.rotation.eulerAngles;
        Vector3 yes = Vector3.Reflect(playerRotation, normalizedWall);
        Debug.Log("reflected vector : " + yes);
        spinerControler.GetComponentInParent<PlayerManager>().transform.rotation = new Quaternion(yes.x, yes.y, yes.z, spinerControler.GetComponentInParent<PlayerManager>().transform.rotation.w);
        Debug.Log("rotation now :" + spinerControler.GetComponentInParent<PlayerManager>().transform.rotation.eulerAngles);
        spinerControler.walled = true;
        //Debug.Log("B " +spinerControler.moveDir);
        
        this.enabled = false;
    }
}
