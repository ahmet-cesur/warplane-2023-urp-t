using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TailAdjusterScript : MonoBehaviour
{
    [SerializeField]  private PlayerPlaneScript planeScript;
    Vector2 dir;                                    // joystick input by player
    public Transform elevators;                     // small winglets on horizontal part of tail
    public Transform aileronRight;                  // right winglet on right wing
    public Transform aileronLeft;
    public Transform rudder;                        // vertical rudder on plane  
    private int tip;                                // what type of plae. 0: spitfire, 1:LaGG

    private void Start()
    {
        planeScript = GetComponent<PlayerPlaneScript>();
        tip = PlayerPrefs.GetInt("Type", 0);
    }
    private void Update()
    {       
        ElevatorAileronAdj();
    }
    private void ElevatorAileronAdj()
    {
        dir = planeScript.dir;
        if (tip == 0)           // if it is spitfire type
        {           
            elevators.localRotation = Quaternion.LookRotation(-1f * Vector3.forward - 0.35f*dir.y * Vector3.up);
            aileronRight.localRotation = Quaternion.LookRotation(Vector3.forward, Vector3.up - 0.5f * dir.x * Vector3.right);
            aileronLeft.localRotation = Quaternion.LookRotation(Vector3.forward, Vector3.up - 0.5f * dir.x * Vector3.right);
            rudder.localRotation = Quaternion.LookRotation(Vector3.forward - 0.45f * dir.x * Vector3.up, Vector3.up);              
        }
        else if (tip == 1)     // if it is Lagg-03 plane. different orientation of some components
        {          
            elevators.localRotation = Quaternion.LookRotation( Vector3.forward - 0.3f* dir.y * Vector3.up);
            aileronRight.localRotation = Quaternion.LookRotation(Vector3.forward - 0.4f * dir.x * Vector3.up, Vector3.up);
            aileronLeft.localRotation = Quaternion.LookRotation(Vector3.forward + 0.4f * dir.x * Vector3.up, Vector3.up);
            rudder.localRotation = Quaternion.LookRotation(Vector3.forward - 0.4f * dir.x * Vector3.right, Vector3.up);  
        }
        else if (tip == 2)     // if it is P51 mustang plane. different orientation of some components
        {
            elevators.localRotation = Quaternion.LookRotation(Vector3.forward - 0.3f * dir.y * Vector3.up);
            aileronRight.localRotation = Quaternion.LookRotation(Vector3.forward - 0.4f * dir.x * Vector3.up, Vector3.up);
            aileronLeft.localRotation = Quaternion.LookRotation(Vector3.forward + 0.4f * dir.x * Vector3.up, Vector3.up);
            rudder.localRotation = Quaternion.LookRotation(Vector3.forward, 0.4f * dir.x * Vector3.right+ Vector3.up);
        }
    }
}
