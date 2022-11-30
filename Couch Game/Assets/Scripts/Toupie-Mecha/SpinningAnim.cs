using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinningAnim : MonoBehaviour
{
    public float rotationDegree = 180f;
    private float rotation;
    [SerializeField] private bool isRotating;
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
        if(isRotating)this.transform.Rotate(new Vector3(0f, 0f, rotation), Space.Self);
    }

    private void OnDisable()
    {
        rotation = 0;
    }

    public void SetRotate(bool rotating)
    {
        isRotating = rotating;
    }
}
