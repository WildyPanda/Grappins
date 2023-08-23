using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    ///Move///
    public float speed = 10f;
    ///Move///
    
    ///Jump///
    public float jumpHeight = 5f;
    ///Jump///

    ///Gravity///
    public Vector3 velocity;
    public Transform GroundCheck;
    public LayerMask GroundMask;
    public bool isGrounded;
    public Transform HeadCheck;
    ///Gravity///

    ///Grapple///
    public Vector3 Path;
    public Vector3 Target;
    public float grappleSpeed = 30f;
    public bool isGrapple;
    ///Grapple///
    
    ///GrappleSecond///
    public Vector3 PathS;
    public Vector3 TargetS;
    public float grappleSpeedS = 1f;
    public bool isGrappleS;
    public Vector3 grappleSecondVelocity;
    ///GrappleSecond///

    ///Dash///
    public float DashDistance = 40f;
    public Vector3 dash;
    public Vector3 Drag = new Vector3(1, 0, 1);
    public Vector3 CancelDash;
    public float actDash = 0;
    public bool isAfterDash;
    ///Dash///


    public CharacterController Controller;

    void Start()
    {
        //Lock the cursor in the game window
        Cursor.lockState = CursorLockMode.Locked;
        GroundCheck = GameObject.Find("GroundCheck").transform;
        HeadCheck = GameObject.Find("HeadCheck").transform;
        Controller = GetComponent<CharacterController>();
    }
    

    void Update()
    {
        //check if the player is grounded by checking a sphere at the position of an empty gameObject at the foot of the player
        isGrounded = Physics.CheckSphere(GroundCheck.position, 0.4f, GroundMask);

        ////////Dash//////
        //Make the player dash by increase his speed during 20 frames
        if (Input.GetButtonDown("Dash") && !isAfterDash && !isGrapple && !isGrappleS)
        {
            dash += Vector3.Scale(transform.forward, (DashDistance * new Vector3((Mathf.Log(1f / (Time.deltaTime * Drag.x + 1)) / -Time.deltaTime), 0, (Mathf.Log(1f / (Time.deltaTime * Drag.z + 1)) / -Time.deltaTime))));
            isAfterDash = true;
        }

        //Make that the player don't fall during the dash
        if (actDash <= 20 && actDash != 0)
        {
            velocity.y = 0;
        }

        //manage the cooldown of the dash
        if (isAfterDash && actDash == 20)
        {
            velocity += dash;
            dash = Vector3.zero;
            actDash += 1;
        }
        else if (isAfterDash && actDash == 40)
        {
            isAfterDash = false;
        }
        else if (isAfterDash)
        {
            actDash += 1;
        }
        else
        {
            actDash = 0;
        }
        ////////Dash////////

        ////////Move////////
        //Get the "Horizontal" and "Vertical" axis for move
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        //Adapt the move at the rotation of the playerBody
        Vector3 move = transform.right * x + transform.forward * z;
        //Use the CharacterController to move in the direction of move
        Controller.Move(move * speed * Time.deltaTime);
        ////////Move////////

        ////////Gravity////////
        //Applied an effect of gravity at the player
        if (velocity.y > 0)
        {
            velocity += Physics.gravity * Time.deltaTime * 2;
        }
        else
        {
            velocity += Physics.gravity * Time.deltaTime;
        }
        ////////Gravity////////

        ////////Jump////////
        //Make the player Jump
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * Physics.gravity.y);
        }
        ////////Jump////////

        ////////GrapplingHook////////
        //Offset the effect of the GrapplingHook
        if (isGrapple)
        {
            velocity -= Path * grappleSpeed;
        }
        
        //Stop the gravity effect if the player is Grounded or if he use the GrapplingHook and is don't use the second GrapplingHook
        if ((isGrounded && velocity.y < 0 || isGrapple) && !isGrappleS)
        {
            velocity.y = -2f;
        }

        //Offset the movement remaining from the grapplingHook
        if (velocity.x != 0 || velocity.z != 0)
        {
            //Check if X or Z axis is negative
            bool Xneg = velocity.x < 0;
            bool Zneg = velocity.z < 0;
            //If On the Ground and no GrapplingHook used
            //Else if Just after a Dash
            //Else if Not on the Ground and no GrapplingHook used
            if (isGrounded && !isGrapple && !isGrappleS)
            {
                velocity.x = Convert.ToSingle(Math.Sqrt(Math.Pow(velocity.x, 2) * 0.8));
                velocity.z = Convert.ToSingle(Math.Sqrt(Math.Pow(velocity.z, 2) * 0.8));
                if (Xneg)
                {
                    velocity.x *= -1;
                }

                if (Zneg)
                {
                    velocity.z *= -1;
                }
            }
            else if (isAfterDash)
            {
                velocity.x = Convert.ToSingle(Math.Sqrt(Math.Pow(velocity.x, 2) * 0.7));
                velocity.z = Convert.ToSingle(Math.Sqrt(Math.Pow(velocity.z, 2) * 0.7));
                if (Xneg)
                {
                    velocity.x *= -1;
                }

                if (Zneg)
                {
                    velocity.z *= -1;
                }
            }
            else if (!isGrapple && !isGrappleS)
            {
                velocity.x = Convert.ToSingle(Math.Sqrt(Math.Pow(velocity.x, 2) * 0.95));
                velocity.z = Convert.ToSingle(Math.Sqrt(Math.Pow(velocity.z, 2) * 0.95));
                if (Xneg)
                {
                    velocity.x *= -1;
                }

                if (Zneg)
                {
                    velocity.z *= -1;
                }
            }
        }

        //Effect of the first GrapplingHook
        if (Target != Vector3.zero && !isGrappleS)
        {
            //Path is the path that need to be traveled
            Path = Target - transform.position;
            //get the total of move of Path to have the point of the Path lower that 1
            float total = Math.Abs(Path.x) + Math.Abs(Path.y) + Math.Abs(Path.z);
            Path.x /= total;
            Path.y /= total;
            Path.z /= total;
            //exemple Path = (0.1,0.2,-0.7)
            //applied Path multiply by grappleSpeed at the velocity
            velocity += Path * grappleSpeed;
        }
        ////////GrapplingHook////////

        ////////SecondGrapplingHook////////
        //transfer the Y axis velocity of the Second GrapplingHook at the velocity
        if (!isGrappleS && grappleSecondVelocity.y != 0)
        {
            velocity.y += grappleSecondVelocity.y;
            grappleSecondVelocity.y = 0;
        }

        //Offset the movement remaining from the grapplingHook
        if (grappleSecondVelocity.x != 0 || grappleSecondVelocity.z != 0)
        {
            //Check if X or Z axis is negative
            bool Xneg = grappleSecondVelocity.x < 0;
            bool Zneg = grappleSecondVelocity.z < 0;
            //If On the Ground and no GrapplingHook used
            //Else
            if (isGrounded && !isGrapple && !isGrappleS)
            {
                grappleSecondVelocity.x = Convert.ToSingle(Math.Sqrt(Math.Pow(grappleSecondVelocity.x, 2) * 0.8));
                grappleSecondVelocity.z = Convert.ToSingle(Math.Sqrt(Math.Pow(grappleSecondVelocity.z, 2) * 0.8));
            }
            else
            {
                grappleSecondVelocity.x = Convert.ToSingle(Math.Sqrt(Math.Pow(grappleSecondVelocity.x, 2) * 0.95));
                grappleSecondVelocity.z = Convert.ToSingle(Math.Sqrt(Math.Pow(grappleSecondVelocity.z, 2) * 0.95));
            }

            if (Xneg)
            {
                grappleSecondVelocity.x *= -1;
            }

            if (Zneg)
            {
                grappleSecondVelocity.z *= -1;
            }
            
        }
        
        //Effect of the second GrapplingHook
        if (TargetS != Vector3.zero && !isGrapple)
        {
            //PathS is the path that need to be traveled
            PathS = TargetS - transform.position;
            //get the total of move of Path to have the point of the Path lower that 1
            float total = Math.Abs(PathS.x) + Math.Abs(PathS.y) + Math.Abs(PathS.z);
            //we need to increase the Horizontal move to get a better movement
            PathS.x = PathS.x / total + 0.5f * (PathS.x / total);
            PathS.y /= total;
            PathS.z = PathS.z / total + 0.5f * (PathS.z / total);
            grappleSecondVelocity += PathS * grappleSpeedS;
            grappleSecondVelocity += transform.forward * 0.5f;
        }
        ////////SecondGrapplingHook////////


        //Make the player take all velocity to move;
        Controller.Move(dash * Time.deltaTime);
        Controller.Move(grappleSecondVelocity * Time.deltaTime);
        Controller.Move(velocity * Time.deltaTime);


    }

    public void GoTo(Vector3 Objectif)
    {
        //check is the other grapple is not used
        if (!isGrappleS)
        {
            //Set Target to the hit point of the raycast
            Target = Objectif;
            //set isGrapple to true
            isGrapple = true;
        }
    }

    public void StopGoTo()
    {
        //reset Path and Target and set isgrapple to false
        Target = Vector3.zero;
        Path = Vector3.zero;
        isGrapple = false;
    }

    public void GoToSecond(Vector3 Objectif)
    {
        //Check if the other GrapplingHook is used
        if (!isGrapple)
        {
            //check if the started gravity is enouth else increase it
            if(velocity.y > -20f)
            {
                velocity.y = -20f;
            }
            //Set TargetS to the hit point of the raycast
            TargetS = Objectif;
            //set isGrappleS to true
            isGrappleS = true;
        }
    }

    public void StopGoToSecond()
    {
        //reset PathS and TargetS and set isgrappleS to false
        TargetS = Vector3.zero;
        PathS = Vector3.zero;
        isGrappleS = false;
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        //When it collide, if the head collide, reset the vertical velocity
        bool isHead = Physics.CheckSphere(HeadCheck.position, 0.4f, hit.gameObject.layer);
        if (isHead)
        {
            velocity.y = -2f;
        }
    }
}
