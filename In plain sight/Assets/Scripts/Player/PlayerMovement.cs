using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public bool movementEnabled;
    private GameObject playerObject;

    [Header("Movement")]
    private float moveSpeed;
    public float walkSpeed;
    public float sprintSpeed;

    public float groundDrag;

    [Header("Jumping")]
    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    bool readyToJump;
    public GameObject footLocation;


    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode sprintKey = KeyCode.LeftShift;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask whatIsGround;
    bool grounded;

    [Header("Slope Handling")]
    public float maxSlopeAngle;
    private RaycastHit slopeHit;
    private bool exitingSlope;

    public Transform orientation;

    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;

    Rigidbody rb;

    //DO NOT REMOVE - FIXES MOVEMENT GLITCH
    private Vector3 objectPosition;

    public MovementState state;
    public enum MovementState
    {
        walking,
        sprinting,
        crouching,
        air
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        readyToJump = true;

        movementEnabled = false;
    }

    public void InhabitObject(GameObject body)
    {
        playerObject = body;

        footLocation = playerObject.GetComponent<InhabitableObject>().footLocation;

        movementEnabled = true;

        rb.isKinematic = false;
    }

    private void Update()
    { 
        //ground check
        grounded = Physics.Raycast(footLocation.transform.position, Vector3.down, 0.2f, whatIsGround);

        if (movementEnabled)
            MyInput();
        else
            ZeroInput();


        SpeedControl();


        if (movementEnabled)
            StateHandler();



        //Handle drag
        if (grounded)
            rb.drag = groundDrag;
        else
            rb.drag = 0;

        if(playerObject)
            objectPosition = playerObject.transform.position;

    }

    private void FixedUpdate()
    {
        if(playerObject)
            this.transform.position = objectPosition;
        MovePlayer();

    }

    private void MyInput()
    {

        //rb.isKinematic = false;
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        //when to jump
        if (Input.GetKey(jumpKey) && readyToJump && grounded)
        {
            readyToJump = false;

            Jump();

            Invoke(nameof(ResetJump), jumpCooldown);
        }

    }

    private void ZeroInput()
    {

        horizontalInput = 0f;
        verticalInput = 0f;
        //rb.isKinematic = true;

    }

    private void StateHandler()
    {

        // Mode - Sprinting
        if (grounded && Input.GetKey(sprintKey))
        {
            state = MovementState.sprinting;
            moveSpeed = sprintSpeed;
        }

        // Mode - Walking
        else if (grounded)
        {
            state = MovementState.walking;
            moveSpeed = walkSpeed;
        }

        // Mode - air
        else
        {
            state = MovementState.air;
        }

    }

    private void MovePlayer()
    {


        // calculate movement direction
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;


        // on slope
        if (OnSlope() && !exitingSlope)
        {
            rb.AddForce(GetSlopeMoveDirection() * moveSpeed * 15f, ForceMode.Force);

            if(rb.velocity.y > 0)
            {
                rb.AddForce(Vector3.down * 80f, ForceMode.Force);
            }
        }



        // on ground
        if (grounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);


        // in air
        else if (!grounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);



        // turn gravity off while on slope
        rb.useGravity = !OnSlope();

 
    }

    private void SpeedControl()
    {

        // limiting speed on slope
        if (OnSlope() && !exitingSlope)
        {
            if (rb.velocity.magnitude > moveSpeed)
                rb.velocity = rb.velocity.normalized * moveSpeed;
        }

        // limiting speed on ground or in air
        else
        {
            Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

            // limit velocity if needed
            if (flatVel.magnitude > moveSpeed)
            {
                Vector3 limitedVel = flatVel.normalized * moveSpeed;
                rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
            }
        }



    }

    private void Jump()
    {

        exitingSlope = true;

        //reset y velicty
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);

    }

    private void ResetJump()
    {

        readyToJump = true;

        exitingSlope = false;

    }

    private bool OnSlope()
    {

        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f + 0.23f))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlopeAngle && angle != 0;
        }



        return false;
    }

    private Vector3 GetSlopeMoveDirection()
    {

        return Vector3.ProjectOnPlane(moveDirection, slopeHit.normal).normalized;
    }
}
