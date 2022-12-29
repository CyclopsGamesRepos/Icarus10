using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;
using UnityEditor.ShaderGraph.Internal;

public class CharacterControllerRB : MonoBehaviour
{
    // declare references;
    private InputControl inputControl;
    private Rigidbody rb;
    private GroundCheck groundCheck;

    [Header("Public Variables")] // accesed by animation handler
    public float velocityX = 0f; 
    public float velocityY = 0f; 
    public Vector3 normalizedVelocity;
    public bool isGrounded;


    //public bool isJumping;

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

    [Header("Movement Control")] // tweaks
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
    private float currentMovement;
    private bool isMovementPressed;
    private bool isRunPressed;
    private bool isJumpPressed;
    private bool isJumping;
    private bool isFalling;

    private float maxVelocity = 0f;
    private float acceleration = 0f;
    private float decceleration = 0f;

    // direction
    private bool isRight;
    private bool isLeft;

    //-----------------------------------------------
    public void Awake()
    {
        // initially set reference variables;
        inputControl = new InputControl();
        rb = GetComponent<Rigidbody>();
        groundCheck = GetComponent<GroundCheck>();
     
        //------------------------------------------------------------
        // callback function for movement input;
        inputControl.CharacterControls.Move.started += onMovementInput;
        inputControl.CharacterControls.Move.canceled += onMovementInput;

        // callback function for Run input;
        inputControl.CharacterControls.Run.started += onRunInput;
        inputControl.CharacterControls.Run.canceled += onRunInput;

        // callback function for Jump input;
        inputControl.CharacterControls.Jump.started += onJumpInput;
        inputControl.CharacterControls.Jump.canceled += onJumpInput;
        //------------------------------------------------------------

        // step climb setup / set y position of raycast to stepHeight;
        stepRayUpper.transform.localPosition = new Vector3(stepRayUpper.transform.position.x, stepHeight, stepRayUpper.transform.position.z);
    }

    //-----------------------------------------------
    private void FixedUpdate()
    {
        isGrounded = groundCheck.IsGrounded(); // check if player is touching the ground

        if (isGrounded && isJumpPressed)
        {
            rb.AddForce(jumpForce * Vector3.up, ForceMode.Impulse);
        }

      

        if (isGrounded)
        {
            //--------------
            // Call Handlers
            handleVelocity();
            handleRotation();
            handleSteps();
        }


        velocityY = rb.velocity.y;

        //----- Final Velocity applied to RigidBody -----------
        rb.velocity = new Vector3 (0, rb.velocity.y, velocityX);
        //-----------------------------------------------------
        normalizedVelocity = rb.velocity.normalized;
        //-----------------------------------------------------
    }

    //-----------------------------------------------
    //-------------------Handlers--------------------
    //-----------------------------------------------
    private void handleVelocity()
    {
        // apply changes to acceleration and decceleration
        if (!isRunPressed)
        {
            maxVelocity = walkSpeed;
            acceleration = walkAcceleration;
            decceleration = walkDecceleration;
        }

        if (isRunPressed)
        {
            maxVelocity = runSpeed;
            acceleration = runAcceleration;
            decceleration = runDecceleration;
        }

        //----------------------------------------------------
        //----------------------------------------------------
        // walk right
        if (isRight && isMovementPressed && velocityX < maxVelocity)
        {
            velocityX += Time.fixedDeltaTime * acceleration;
        }

        // slow from walk right
        if (!isMovementPressed && velocityX > 0)
        {
            velocityX -= Time.fixedDeltaTime * decceleration;
        }

        // slow from run right
        if (isRight && isMovementPressed && velocityX > maxVelocity)
        {
            velocityX -= Time.fixedDeltaTime * runDecceleration;
        }

        // switch velocity to prevent sliding on quick turns
        if (isRight && velocityX < 0)
        {
            velocityX *= -switchSpeed;
        }

        // reset velocity when still
        if (isRight && !isMovementPressed && !isRunPressed && velocityX < idleThreshold)
        {
            velocityX = 0;
        }

        //----------------------------------------------------
        //----------------------------------------------------
        //walk left
        if (isLeft && isMovementPressed && velocityX > -maxVelocity)
        {
            velocityX -= Time.fixedDeltaTime * acceleration;
        }

        //slow from walk left
        if (!isMovementPressed && velocityX < 0)
        {
            velocityX += Time.fixedDeltaTime * decceleration;
        }

        // slow from run left
        if (isLeft && isMovementPressed && velocityX < -maxVelocity)
        {
            velocityX += Time.fixedDeltaTime * runDecceleration;
        }

        // switch velocity to prevent sliding on quick turns
        if (isLeft && velocityX > 0)
        {
            velocityX *= -switchSpeed;
        }

        // reset velocity when still
        if (isLeft && !isMovementPressed && !isRunPressed && velocityX > -idleThreshold)
        {
            velocityX = 0;
        }

    }


    //-----------------------------------------------
    private void onMovementInput(InputAction.CallbackContext context)
    {
        // assign movement input to variable / Walk;
        currentMovement = context.ReadValue<float>();

        // isMovementPressed is true if current movement does not equal 0
        isMovementPressed = currentMovement != 0;

        // Check Direction
        if (currentMovement == 1)
        {
            isRight = true;
            isLeft = false;
        }
        else if (currentMovement == -1)
        {
            isRight = false;
            isLeft = true;
        }
    }

    //-----------------------------------------------
    private void onRunInput(InputAction.CallbackContext context)
    {
        isRunPressed = context.ReadValueAsButton();
    }

    //-----------------------------------------------
    private void onJumpInput(InputAction.CallbackContext context)
    {
        isJumpPressed = context.ReadValueAsButton();
    }

    //-----------------------------------------------
    private void handleRotation()
    {
        positionToLookAt = new Vector3(0, 0, currentMovement);

        Quaternion currentRotation;
        currentRotation = transform.rotation;

        if (isMovementPressed)
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

    //-----------------------------------------------
    private void OnEnable()
    {
        inputControl.CharacterControls.Enable();    
    }

    //-----------------------------------------------
    private void OnDisable()
    {
        inputControl.CharacterControls.Disable();
    }
}
