using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour
{
    //the sensitivity of the mouse
    public float mouseSensitivity = 5f;
    //the rotation of the camera
    public float rotation;
    //the transform of the player
    public Transform PlayerBody;

    private void Start()
    {
        PlayerBody = GameObject.Find("Player").transform;
    }

    void Update()
    {
        //Get the "Mouse Y" axis and multiply it by the mouse sensitivity
        float mouseLookY = Input.GetAxis("Mouse Y") * mouseSensitivity;
        //Subtract mouseLookY at rotation for do the rotation
        rotation -= mouseLookY;
        //Check if rotation stay in [-90f;90f]
        rotation = Mathf.Clamp(rotation, -90f, 90f);
        //Set the rotation of the camera on the X axis
        transform.localRotation = Quaternion.Euler(rotation, 0f, 0f);

        //Get the "Mouse X" axis and multiply it by the mouse sensitivity
        float mouseLookX = Input.GetAxis("Mouse X") * mouseSensitivity;
        //Rotate the playerBody on the Y axis
        PlayerBody.Rotate(Vector3.up * mouseLookX);
    }
}
