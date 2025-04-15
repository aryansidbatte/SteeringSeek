using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class SteeringBehavior : MonoBehaviour
{
    public Vector3 target;
    public KinematicBehavior kinematic;
    public List<Vector3> path;
    // you can use this label to show debug information,
    // like the distance to the (next) target
    public TextMeshProUGUI label;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        kinematic = GetComponent<KinematicBehavior>();
        target = transform.position;
        path = null;
        EventBus.OnSetMap += SetMap;
    }

    // Update is called once per frame
    void Update()
    {
        // Assignment 1: If a single target was set, move to that target
        //                If a path was set, follow that path ("tightly")

        // you can use kinematic.SetDesiredSpeed(...) and kinematic.SetDesiredRotationalVelocity(...)
        //    to "request" acceleration/decceleration to a target speed/rotational velocity

        // Direction to target
        Vector3 direction = target - transform.position;
        // Current heading
        Vector3 heading = transform.forward;
        // Angle between heading and direction
        float angleToTarget = Vector3.SignedAngle(heading, direction, Vector3.up);

        // Distance to target
        float distanceToTarget = direction.magnitude;
        // Normalize direction vector
        direction.Normalize();
        
        // Set desired speed and rotational velocity based on distance to target
        if (path != null && path.Count > 0)
        {
            // If we have a path, follow it
            target = path[0];
            distanceToTarget = Vector3.Distance(transform.position, target);
            if (distanceToTarget < 1.0f)
            {
                path.RemoveAt(0);
            }
        }
        else if (distanceToTarget < 1.0f)
        {
            // If we are close to the target, stop
            kinematic.SetDesiredSpeed(0.0f);
            kinematic.SetDesiredRotationalVelocity(0.0f);
        }
        else
        {
            // Set desired speed and rotational velocity based on distance to target
            kinematic.SetDesiredSpeed(Mathf.Clamp(distanceToTarget, 0.0f, kinematic.max_speed));
            kinematic.SetDesiredRotationalVelocity(angleToTarget * 2.0f);
        }

        // Update label with distance to target
        if (label != null)
        {
            label.text = "Distance to target: " + distanceToTarget.ToString("F2") + " m";
        }
    }

    public void SetTarget(Vector3 target)
    {
        this.target = target;
        EventBus.ShowTarget(target);
    }

    public void SetPath(List<Vector3> path)
    {
        this.path = path;
    }

    public void SetMap(List<Wall> outline)
    {
        this.path = null;
        this.target = transform.position;
    }
}
