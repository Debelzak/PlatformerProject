using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class MovingPlatform : SolidObject
{
    [Serializable]
    public struct Waypoint
    {
        public Vector2 position;
        public float waitTime;

        public Waypoint(Vector2 position, float waitTime)
        {
            this.position = position;
            this.waitTime = waitTime;
        }
    }

    // Public
    public bool isCyclic;
    public float speed;
    public List<Waypoint> wayPoints;

    // Private
    private Vector3 initialPosition;
    private List<Waypoint> inGameWaypoints;
    private int currentWaypointIndex;
    private float timer;

    // Unity methods
    protected override void Start()
    {
        base.Start();
        SetupWaypoints();
    }
    protected override void FixedUpdate()
    {
        velocity = CalculateVelocity();
        base.FixedUpdate();
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Vector2 position = transform.position;

        if(Application.isPlaying)
        {
            position = initialPosition;
        }

        if(wayPoints.Count > 0)
        {
            for(int i = 0; i < wayPoints.Count - 1; i++)
            {
                Gizmos.DrawSphere(position + wayPoints[i].position, 0.1f);
                Gizmos.DrawLine(position + wayPoints[i].position, position + wayPoints[i+1].position);
            }

            Gizmos.DrawSphere(position + wayPoints[wayPoints.Count - 1].position, 0.1f);
            if(wayPoints.Count >= 2 && isCyclic)
            {
                Gizmos.DrawLine(position + wayPoints[wayPoints.Count - 1].position, position + Vector2.zero);
            }
        }
    }

    // Custom methods
    private Vector2 CalculateVelocity()
    {
        if(timer > 0)
        {
            timer -= Time.deltaTime;
            return Vector2.zero;
        }

        if(inGameWaypoints.Count <= 1)
        {
            return Vector2.zero;
        }

        if(currentWaypointIndex == inGameWaypoints.Count - 1)
        {
            currentWaypointIndex = 0;
            if(!isCyclic)
            {
                inGameWaypoints.Reverse();
            }
        }

        int nextWayPointIndex = currentWaypointIndex + 1;
        Vector2 nextWayPoint = inGameWaypoints[nextWayPointIndex].position;
        Vector2 currentPosition = rb.position;
        Vector2 direction = (nextWayPoint - currentPosition).normalized;
        Vector2 moveAmount = direction * speed;
        float distanceNow = Vector2.Distance(currentPosition, nextWayPoint);
        float distanceAfterMovement = Vector2.Distance(currentPosition + moveAmount * Time.fixedDeltaTime, nextWayPoint);
        
        if(distanceAfterMovement >= distanceNow) // Significa que ele irá passar do ponto final
        {
            moveAmount = nextWayPoint - currentPosition;  // Move para fim deste waypoint
            currentWaypointIndex++; // Move para próximo
            timer = inGameWaypoints[nextWayPointIndex].waitTime;
        }
    
        return moveAmount;
    }
    private void SetupWaypoints()
    {
        //Setup waypoints
        initialPosition = rb.position;
        inGameWaypoints = new List<Waypoint>();
        foreach(Waypoint waypoint in wayPoints)
        {
            Waypoint toAdd = new Waypoint(waypoint.position + rb.position, waypoint.waitTime);

            inGameWaypoints.Add(toAdd);
        }
        if(wayPoints.Count > 1 && isCyclic)
        {
            Waypoint toAdd = new Waypoint(Vector2.zero + rb.position, wayPoints[wayPoints.Count - 1].waitTime);
            inGameWaypoints.Add(toAdd);
        }
    }
}
