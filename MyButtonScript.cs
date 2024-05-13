using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MyButtonScript : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{

    // this scripted is used by Warplane project
    public bool buttonPressed = false;          // is button for firing pressed on keyboard or UI
    private bool triggerStatus = false;         // old status of button press. used for checks in changes of value of buttonpressed variable
    PlayerPlaneScript pps;                      // player controller script

    private void Start()
    {
        pps = GameObject.FindWithTag("Player").GetComponent<PlayerPlaneScript>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        buttonPressed = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        buttonPressed = false;
    }

    void Update()
    {
        if (Input.GetButton("Fire1"))
        {
            buttonPressed = true;
        }
        if (Input.GetButtonUp("Fire1"))
        {
            buttonPressed = false;
        }

        if(triggerStatus!=buttonPressed)        // this check is made with purpose to prevent unnecessary communication between scripts
        {
            pps.isFiring = buttonPressed;
            triggerStatus = buttonPressed;
        }
    }
}
