using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stretch : MonoBehaviour
{
    [Header("Stretch Parameter")]
    private Vector3 initialStretch;
    public Transform objectToStretch;
    public Vector3 desireStretch;
    public float stretchSpeed;
    public float unstretchSpeed;
    public bool noStretch;

    private void OnEnable()
    {
        initialStretch = objectToStretch.localScale;
        noStretch = false;
    }
    
    private void OnDisable()
    {
        objectToStretch.localScale = initialStretch;
        noStretch = true;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Stretching();
    }

    private void Stretching()
    {
        if(!noStretch)
        {
            objectToStretch.localScale = Vector3.MoveTowards(objectToStretch.localScale, desireStretch , stretchSpeed * Time.fixedDeltaTime);
        }
        else
        {
            objectToStretch.localScale = Vector3.MoveTowards(objectToStretch.localScale, initialStretch, unstretchSpeed * Time.fixedDeltaTime);
            if(objectToStretch.localScale == initialStretch)this.enabled = false;    
        }
    }
}
