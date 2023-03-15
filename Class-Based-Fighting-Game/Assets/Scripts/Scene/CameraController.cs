using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    //public Transform[] targets;

    public List<Transform> targets;

    public float zoomFactor = 1.5f;
    public float followTime = 0.8f;

    private Camera cam;
    // Start is called before the first frame update
    void Start()
    {
        cam = this.gameObject.GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        if(targets.Count >= 1)
        {
            followTargets();
        }
    }

    // Spherically Interpolate the position of the camera to the midpoint of all of the targets
    private void followTargets()
    {
        if(targets == null) {
            return;
        }
        
        Vector3 midpoint = targets[0].position;
        
        float distance = 5;
        
        for(int i = 1; i < targets.Count; i++)
        {
            midpoint += targets[i].position;

            distance = Mathf.Max(distance, Mathf.Clamp((midpoint / i - targets[i].position).magnitude, 5, 10));
        }

        midpoint /= targets.Count;

        Vector3 destination = midpoint - this.transform.forward * distance * zoomFactor;

        cam.orthographicSize = distance;

        cam.transform.position = Vector3.Slerp(transform.position, destination, followTime);

        if ((destination - transform.position).magnitude <= 0.05f)
        {
            transform.position = destination;
        }
    }
}
