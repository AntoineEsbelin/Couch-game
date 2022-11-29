using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTarget : MonoBehaviour
{
    public List<Transform> targets;

    public Vector3 offset;
    private Vector3 velocity;

    public float smoothTime = 0.5f;

    public float minZoom = 85f;
    public float maxZoom = 55f;
    public float zoomLimiter = 50f;

    public float boundsSize;
    public float BoundSizeLimiter = 6f;

    private Camera cam;

    private void Start()
    {
        cam = GetComponent<Camera>();
    }

    private void LateUpdate()
    {
        if (targets.Count == 0)
            return;

        CamMove();
        CamZoom();
        
    }

    void CamZoom()
    {
        float newZoom = Mathf.Lerp(maxZoom, minZoom, boundsSize/ zoomLimiter);
       // cam.fieldOfView = Mathf.Lerp(cam.orthographicSize, newZoom, Time.deltaTime);
        
    }

    void CamMove()
    {
        Vector3 centerPoint = GetCenterPoint();

        Vector3 newPosition = centerPoint + ( offset + new Vector3(0, boundsSize /BoundSizeLimiter ,0) );

        transform.position = Vector3.SmoothDamp(transform.position, newPosition, ref velocity, smoothTime);

        
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
