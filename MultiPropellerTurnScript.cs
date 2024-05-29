using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiPropellerTurnScript : MonoBehaviour
{
    public Transform[] propTr;            // transform of propeller
    public int[] axis;                    // -1 if turns reverse direction, +1 if turns same direction
    public Vector3 turnAxis;            // to select on which axis propellev will turn
    public float turnSpeed;             // turning speed of propeller

    private void Update()
    {
        for (int i = 0; i < propTr.Length; i++)
        {
            propTr[i].Rotate(axis[i]* turnSpeed * turnAxis * Time.deltaTime, Space.Self);
        }    
    }
}
