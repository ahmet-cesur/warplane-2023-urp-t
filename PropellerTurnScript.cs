using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropellerTurnScript : MonoBehaviour
{

    public Transform propTr;            // transform of propeller
    public Vector3 turnAxis;            // to select on which axis propellev will turn
    public float turnSpeed;             // turning speed of propeller
    [SerializeField] private PlayerPlaneScript planeScript;
    public bool isNPC;

    private void Start()
    {
        if (!isNPC)
        {
            planeScript = GetComponent<PlayerPlaneScript>();
        }       
    }

    private void Update()
    {
        if (!isNPC)
        {
            turnSpeed = Mathf.Lerp(turnSpeed, (planeScript.speedRatio + 2f) * 1000f, 0.2f * Time.deltaTime);
            propTr.Rotate(turnSpeed * turnAxis * Time.deltaTime, Space.Self);
        }
        else
        {
            propTr.Rotate(turnSpeed * turnAxis * Time.deltaTime, Space.Self);
        }
    }

}
