using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Grappling : MonoBehaviour
{
    //Layer grippable
    public LayerMask grippable;
    //Range of the Grapplin Hook
    public float Range = Mathf.Infinity;
    //LineRenderer visual effect of string
    public LineRenderer grappleString;
    //Point of the string on the gun
    public Vector3 LineOnGun;
    //Point of the string on the object
    public Vector3 LineOnObject;
    //Link to the Script PlayerController
    public PlayerController PM;
    //Transform of the camera so that the Raycast is straight
    public Transform StartRaycast;
    //Transform of the graphics so tht the gun look at the point grabbed 
    public Transform GunObject;

    void Start()
    {
        grappleString = GetComponent<LineRenderer>();
        PM = GetComponentInParent<PlayerController>();
        StartRaycast = GetComponentInParent<Camera>().gameObject.GetComponent<Transform>();
        GunObject = GameObject.Find("GrappleGunL").transform;
    }


    void Update()
    {
        //Update LineOnGun and if LineOnObject don't have move, it too
        if (LineOnGun == LineOnObject)
        {
            LineOnGun = transform.position;
            LineOnObject = LineOnGun;
        }
        else
        {
            LineOnGun = transform.position;
        }
        //Start the function GrappleOn when the key "Fire1" is press and GrappleOff when it release
        if (Input.GetButtonDown("Fire1"))
        {
            GrappleOn();
        }
        else if (Input.GetButtonUp("Fire1"))
        {
            GrappleOff();
        }
        //Set LineOnGun at first position on the LineRenderer and LineOnObject at the second position
        grappleString.SetPosition(0, LineOnGun);
        grappleString.SetPosition(1, LineOnObject);
        //Make the graphics lookAt the point grabbed
        if (LineOnObject != LineOnGun)
        {
            GunObject.LookAt(LineOnObject);
        }
        else
        {
            GunObject.localRotation = Quaternion.Euler(Vector3.zero);
        }
    }


    void GrappleOn()
    {
        //Check if the other grappling hook is used
        if (!PM.isGrappleS)
        {
            //Check if a collider with a grippable layer in a line from the camera to forward in the range
            RaycastHit hit;
            if (Physics.Raycast(StartRaycast.position, StartRaycast.TransformDirection(Vector3.forward), out hit, Range, grippable))
            {
                //Set LineOnObject at the point hit by the raycast
                LineOnObject = hit.point;
                //Start the function in PlayerController called GoTo
                PM.GoTo(hit.point);
            }
        }
    }

    void GrappleOff()
    {
        //Set LineOnObject at the same position as LineOnGun
        LineOnObject = LineOnGun;
        //Start the function in PlayerController called StopGoTo
        PM.StopGoTo();
    }
}
