using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraTarget : MonoBehaviour
{
    public List<Transform> targets;

    public Vector3 offset;
    private Vector3 velocity;

    public float smoothTime = 0.5f;

    public float boundsSize;
    public float BoundSizeLimiter = 6f;

    public float CamClampXMin = -26f;
    public float CamClampXMax = 32f;

    public float CamCampZMin = 34f;
    public float CamCampZMax = 60f;

    private Camera cam;

    private void Start()
    {
        cam = GetComponent<Camera>();
    }

    private void LateUpdate()
    {
        if (targets.Count == 0)
            return;

        if(!GameManager.instance.gameStarted)return;
        for (int i = 0; i < targets.Count; i++)
        {
            if (!targets[i])
            {
                targets.Remove(targets[i]);
            }
        }
        
        CamMove();
               
    }


    void CamMove()
    {
        Vector3 centerPoint = GetCenterPoint();

        Vector3 newPosition = centerPoint + ( offset + new Vector3(0, boundsSize /BoundSizeLimiter ,0) );


        transform.position = Vector3.SmoothDamp(transform.position, newPosition, ref velocity, smoothTime);
        transform.position = new Vector3(
            Mathf.Clamp(transform.position.x, CamClampXMin, CamClampXMax) , transform.position.y, Mathf.Clamp(transform.position.z, CamCampZMin, CamCampZMax)
            );
        
    }
    

    Vector3 GetCenterPoint()
    {
        if(targets.Count == 1)
        {
            return targets[0].position;
        }

        var bounds = new Bounds(targets[0].position, Vector3.zero);

        for (int i = 0; i < targets.Count; i++)
        {
            bounds.Encapsulate(targets[i].position);
        }

        boundsSize = bounds.size.x;
        
        return bounds.center;
        
    }
    

}
