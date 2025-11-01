using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController controller;
 
    public float speed = 12f;
    public float gravity = -9.81f * 2;
    public float jumpHeight = 1.5f;
    
    // Sprint fields: hold Left Shift to add this amount to `speed` while held
    [Tooltip("Hold Left Shift to sprint (adds this amount to `speed`).")]
    public float sprintAdd = 10f;
    
    // Camera reference so movement follows camera look (W = forward of camera)
    [Tooltip("Assign the player camera transform here. If left empty the script will use Camera.main.")]
    public Transform cam;
 
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;
 
    Vector3 velocity;
 
    bool isGrounded;

    void Start()
    {
        // If no camera assigned, try to use the main camera
        if (cam == null && Camera.main != null)
        {
            cam = Camera.main.transform;
        }
    }
 
    // Update is called once per frame
    void Update()
    {

        //checking if we hit the ground to reset our falling velocity, otherwise we will fall faster the next time
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
 
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
 
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        // Movement relative to camera look: W moves toward camera forward (ignoring vertical tilt)
        Vector3 right = Vector3.right;
        Vector3 forward = Vector3.forward;

        if (cam != null)
        {
            forward = cam.forward;
            right = cam.right;
            // ignore vertical component so movement stays on the XZ plane
            forward.y = 0f;
            right.y = 0f;
            forward.Normalize();
            right.Normalize();
        }

        Vector3 move = right * x + forward * z;

        // Sprint: hold Left Shift to temporarily add sprintAdd to speed
        float currentSpeed = speed;
        if (Input.GetKey(KeyCode.LeftShift))
        {
            currentSpeed += sprintAdd;
        }

        controller.Move(move * currentSpeed * Time.deltaTime);
 
        //check if the player is on the ground so he can jump
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            //the equation for jumping
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
 
        velocity.y += gravity * Time.deltaTime;
 
        controller.Move(velocity * Time.deltaTime);
    }
}