
using UnityEngine;

public class AutoPilotScript : MonoBehaviour
{
    public Transform[] waypoints; // Array of waypoints (positions)
    public float moveSpeed ; // Movement speed
    public float turnSpeed ; // Rotation speed (degrees per second)
    private Quaternion roti;
    private int currentWaypointIndex = 0; // Index of the current waypoint

    void Update()
    {
        // Check if the object has reached the current waypoint
        if (Vector3.Distance(transform.position, waypoints[currentWaypointIndex].position) < 1f)
        {
            // Move to the next waypoint
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
        }

        // Calculate the direction to the next waypoint
        Vector3 targetDirection = waypoints[currentWaypointIndex].position - transform.position;

        // Rotate towards the target direction
        
        roti=Quaternion.LookRotation(targetDirection, Vector3.up+transform.right* Vector3.Dot( transform.right,targetDirection.normalized));
        transform.rotation =Quaternion.Slerp(transform.rotation, roti, turnSpeed*Time.deltaTime);
        //transform.rotation= Quaternion.Loo
     //  transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);

        // Move towards the current waypoint
        //  transform.position = Vector3.MoveTowards(transform.position, waypoints[currentWaypointIndex].position, moveSpeed * Time.deltaTime);
        transform.position += transform.forward * moveSpeed * Time.deltaTime;
    }
}
