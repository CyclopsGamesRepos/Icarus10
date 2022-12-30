using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;
using UnityEditor.ShaderGraph.Internal;
using static UnityEngine.ProBuilder.AutoUnwrapSettings;
using UnityEngine.Networking.Types;

public class CharacterControllerRB : MonoBehaviour
{
    //-- Declare References --//
    private ControllerInputManager input;
    private Rigidbody rb;
    private GroundCheck groundCheck;

    [Header("Public Variables")] // accesed by animation handler
    public float velocityX = 0f; 
    public float velocityY = 0f; 
    public bool isGrounded;
    public bool isJumping;
    public bool isFalling;

    [Header("Walk")] // walk
    [SerializeField] private float walkSpeed = 2f;
    [SerializeField] private float walkAcceleration = 5f;
    [SerializeField] private float walkDecceleration = 5f;

    [Header("Run")] // run
    [SerializeField] private float runSpeed = 4f;
    [SerializeField] private float runAcceleration = 2f;
    [SerializeField] private float runDecceleration = 2f;

    [Header("Jump")] // Jump
    [SerializeField] private float jumpForce = 6f;

    [Header("Movement Control")] // motion tweaks
    [SerializeField] private float idleThreshold = 0.05f;
    [SerializeField] private float switchSpeed = 0.5f;

    [Header("Rotation")] // Character rotation variables;
    [SerializeField] private float rotationFactorPerFrame = 8f;
    private Vector3 positionToLookAt;
    
    [Header("Step Climb")] // Step climb variables;
    [SerializeField] private GameObject stepRayUpper;
    [SerializeField] private float upperRaycastDistance = 0.2f;
    [SerializeField] private GameObject stepRayLower;
    [SerializeField] private float lowerRaycastDistance = 0.1f;
    [SerializeField] private float stepHeight = 0.3f;
    [SerializeField] private float stepSmooth = 0.1f;

    //------------------------------------------------
    // movement
    private float maxVelocity = 0f;
    private float acceleration = 0f;
    private float decceleration = 0f;
    private Vector3 normalizedVelocity;

    // direction
    private bool isRight;
    private bool isLeft;

    //-----------------------------------------------
    public void Awake()
    {
        //-----------------------------------
        // initially set reference variables;
        rb = GetComponent<Rigidbody>();
        groundCheck = GetComponent<GroundCheck>();
        input = GetComponent<ControllerInputManager>();

        //-------------------------------------------------------------------------------
        // step climb setup / set y position of raycast to stepHeight transform position;
        stepRayUpper.transform.localPosition = new Vector3(stepRayUpper.transform.position.x, stepHeight, stepRayUpper.transform.position.z);
    }

    //-----------------------------------------------
    private void FixedUpdate()
    {
   
        //-----------------------------------
        // check if player is grounded
        isGrounded = groundCheck.isGrounded();

        //-----------------------------------
        // jump
        handleJump();

        //-----------------------------------------------
        // if player is grounded normal motion is applied
        if (isGrounded)       
        {
            handleDirection();
            handleVelocity();
            handleRotation();
            handleSteps();
        }



        //------------------------------------
        // Final Velocity applied to RigidBody
        //------------------------------------
        velocityY = rb.velocity.y;

        rb.velocity = new Vector3 (0, velocityY, velocityX);


        //-----------------------------------------
        normalizedVelocity = rb.velocity.normalized;    

    }


    //-----------------------------------------------
    //-------------------Handlers--------------------
    //-----------------------------------------------
    private void handleJump()
    {
        if (isGrounded)
        {
            isJumping = false;
            isFalling = false;
      
        }

        if (isGrounded && input.isJumpPressed)
        {
            rb.AddForce(jumpForce * Vector3.up, ForceMode.Impulse);
        }

        // going up
        if (!isGrounded && normalizedVelocity.y > 0)
        {
            isJumping = true;
            isFalling = false;
        }

        // going down
        if (!isGrounded && normalizedVelocity.y < 0)
        {
            isFalling = true;
            isJumping = false;
        }
    }

    //-----------------------------------------------
    private void handleDirection()
    {
        if (input.currentMovement == 1)
        {
            isRight = true;
            isLeft = false;
        }
        else if (input.currentMovement == -1)
        {
            isLeft = true;
            isRight = false;
        }
    }

    //-----------------------------------------------
    private void handleVelocity()
    {
        // apply changes to acceleration and decceleration
        if (!input.isRunPressed)
        {
            maxVelocity = walkSpeed;
            acceleration = walkAcceleration;
            decceleration = walkDecceleration;
        }

        if (input.isRunPressed)
        {
            maxVelocity = runSpeed;
            acceleration = runAcceleration;
            decceleration = runDecceleration;
        }

        //----------------------------------------------------
        //----------------------------------------------------
        // walk | right
        if (isRight && input.isMovementPressed && velocityX < maxVelocity)
        {
            velocityX += Time.fixedDeltaTime * acceleration;
        }

        // slow from walk | right
        if (!input.isMovementPressed && velocityX > 0)
        {
            velocityX -= Time.fixedDeltaTime * decceleration;
        }

        // slow from run | right
        if (isRight && input.isMovementPressed && velocityX > maxVelocity)
        {
            velocityX -= Time.fixedDeltaTime * runDecceleration;
        }

        // switch velocity to prevent sliding on quick turns
        if (isRight && velocityX < 0)
        {
            velocityX *= -switchSpeed;
        }

        // reset velocity when still
        if (isRight && !input.isMovementPressed && !input.isRunPressed && velocityX < idleThreshold)
        {
            velocityX = 0;
        }

        //----------------------------------------------------
        //----------------------------------------------------
        //walk | left
        if (isLeft && input.isMovementPressed && velocityX > -maxVelocity)
        {
            velocityX -= Time.fixedDeltaTime * acceleration;
        }

        //slow from walk | left
        if (!input.isMovementPressed && velocityX < 0)
        {
            velocityX += Time.fixedDeltaTime * decceleration;
        }

        // slow from run | left
        if (isLeft && input.isMovementPressed && velocityX < -maxVelocity)
        {
            velocityX += Time.fixedDeltaTime * runDecceleration;
        }

        // switch velocity to prevent sliding on quick turns
        if (isLeft && velocityX > 0)
        {
            velocityX *= -switchSpeed;
        }

        // reset velocity when still
        if (isLeft && !input.isMovementPressed && !input.isRunPressed && velocityX > -idleThreshold)
        {
            velocityX = 0;
        }

    }

    //-----------------------------------------------
    private void handleRotation()
    {
        positionToLookAt = new Vector3(0, 0, input.currentMovement);

        Quaternion currentRotation;
        currentRotation = transform.rotation;

        if (input.isMovementPressed)
        {
            Quaternion targetRotation = Quaternion.LookRotation(positionToLookAt);
            rb.MoveRotation(Quaternion.Slerp(currentRotation, targetRotation, rotationFactorPerFrame * Time.deltaTime));
        }

    }

    //-----------------------------------------------
    // from tutorial https://www.youtube.com/watch?v=DrFk5Q_IwG0
    private void handleSteps()
    {
        // check lower step using raycast
        RaycastHit hitLower;
        if (Physics.Raycast(stepRayLower.transform.position, transform.transform.TransformDirection(Vector3.forward), out hitLower, lowerRaycastDistance))
        {

            // check higher step is not hitting anything using raycast
            RaycastHit hitUpper;
            if (!Physics.Raycast(stepRayUpper.transform.position, transform.transform.TransformDirection(Vector3.forward), out hitUpper, upperRaycastDistance))
            {
                
                rb.position -= new Vector3(0f, -stepSmooth, 0f);
            }

        }

        // check diagonal steps, if player is in mid turn
        RaycastHit hitLower45;
        if (Physics.Raycast(stepRayLower.transform.position, transform.transform.TransformDirection(1.25f,0,1), out hitLower45, lowerRaycastDistance))
        {
            RaycastHit hitUpper45;
            if (!Physics.Raycast(stepRayUpper.transform.position, transform.transform.TransformDirection(1.25f,0,1), out hitUpper45, upperRaycastDistance))
            {
                rb.position -= new Vector3(0f, -stepSmooth, 0f);
            }
        }

        RaycastHit hitLowerMinus45;
        if (Physics.Raycast(stepRayLower.transform.position, transform.transform.TransformDirection(-1.25f, 0, 1), out hitLowerMinus45, lowerRaycastDistance))
        {
            RaycastHit hitUpperMinus45;
            if (!Physics.Raycast(stepRayUpper.transform.position, transform.transform.TransformDirection(-1.25f, 0, 1), out hitUpperMinus45,upperRaycastDistance))
            {
                rb.position -= new Vector3(0f, -stepSmooth, 0f);
            }
        }
    }



}//--------------------
