using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinningAnim : MonoBehaviour
{
    public float rotationDegree = 180f;
    private float rotation;
    private void OnEnable()
    {
        rotation = rotationDegree;
    }
    private void FixedUpdate()
    {
        /*if(rotation >= 0)
        {
            rotation = Mathf.Clamp(rotation - 1, 0f, rotationDegree);
        }*/
        this.transform.Rotate(new Vector3(0f, rotation, 0f), Space.Self);
    }

    private void OnDisable()
    {
        rotation = 0;
    }
}
