using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;
using static UnityEngine.ProBuilder.AutoUnwrapSettings;
using UnityEngine.Networking.Types;
using UnityEngine.Windows;

public class CharacterControllerRB : MonoBehaviour
{

    //-- Declare References --//
    private ControllerInputManager input;
    private Rigidbody rb;
    private GroundCheck groundCheck;
    private BoxCollider boxCollider;
    private CollisionManager collisionManager;
    private AnimationHandler animationHandler;
    //-------------------------


    //-------------------------
    [Header("Public Variables")] // accesed by animation handler
    [SerializeField] float rotation;

    public float velocityX = 0f; 
    public float velocityY = 0f;

    public bool isGrounded;
    public bool isJumping;
    public bool isFalling;
    public bool isCrouched;
    public int crouchToggle;
    public int interactToggle;


    //public bool isStanding;
    //public bool actionAllowed;
    //public bool movementAllowed;
    public bool isInteracting;

    //------------------------


    public bool isPiloting;
    public bool isSolvingCablePuzzle;
    public bool isSolvingTerminalPuzzle;
    public bool isAiming;
    //-----------------------


    //-----------------------
    [Header("Walk")] // walk
    [SerializeField] private float walkSpeed = 2f;
    [SerializeField] private float walkAcceleration = 5f;
    [SerializeField] private float walkDecceleration = 5f;

    [Header("Run")] // run
    [SerializeField] private float runSpeed = 4f;
    [SerializeField] private float runAcceleration = 2f;
    [SerializeField] private float runDecceleration = 2f;

    [Header("Crouch")] // run
    [SerializeField] private float crouchSpeed = 1.5f;
    [SerializeField] private float crouchAcceleration = 2f;
    [SerializeField] private float crouchDecceleration = 5f;

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
    //[SerializeField] private float stepHeight = 0.3f;
    [SerializeField] private float stepSmooth = 0.1f;
    [SerializeField] LayerMask platform;


    [Header("Collider")] // Collider Variables;
    [SerializeField] private Vector3 boxColliderSize = new Vector3(0, 0.89f, 0);
    [SerializeField] private Vector3 boxColliderCenter;


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
        boxCollider = GetComponent<BoxCollider>();
        collisionManager =  GetComponent<CollisionManager>();
        animationHandler = GetComponent<AnimationHandler>();  //??

        //-------------------------------------------------------------------------------
        // step climb setup / set y position of raycast to stepHeight transform position;
        //stepRayUpper.transform.localPosition = new Vector3(stepRayUpper.transform.position.x, stepHeight, stepRayUpper.transform.position.z);
    }

    //-----------------------------------------------
    private void FixedUpdate()
    {
       

        //-----------------------------------------------
        if (collisionManager.inCockpitZone)
        {
            if (interactToggle == 1) { isPiloting = true; }

            if (interactToggle == 0 ) { isPiloting = false; }
        }

        if (!collisionManager.inCockpitZone && isPiloting)
        {
            interactToggle = 0;
            isPiloting = false;
        }

        //-----------------------------------------------
        if (collisionManager.inCablePuzzleZone)
        {
            if (interactToggle == 1) { isSolvingCablePuzzle = true; }

            if (interactToggle == 0) { isSolvingCablePuzzle = false; }
        }

        
        if (!collisionManager.inCablePuzzleZone && isSolvingCablePuzzle)
        {
            interactToggle = 0;
            isSolvingCablePuzzle = false;
        }

        //-----------------------------------------------
        if (collisionManager.inTerminalPuzzleZone)
        {
            if (interactToggle == 1) { isSolvingTerminalPuzzle = true; }

            if (interactToggle == 0) { isSolvingTerminalPuzzle = false; }
        }

        if (!collisionManager.inTerminalPuzzleZone && isSolvingTerminalPuzzle)
        {
            interactToggle = 0;
            isSolvingTerminalPuzzle = false;
        }

        //-----------------------------------------------
        if (collisionManager.inFireZone)
        {
            if (interactToggle == 1) { isAiming = true; }

            if (interactToggle == 0) { isAiming = false; }
        }


        if (!collisionManager.inFireZone && isAiming)
        {
            interactToggle = 0;
            isAiming = false;
        }


        //---------------------
        // Reset interaction toggle
        if (interactToggle > 1)
        {
            interactToggle = 0;
        }

        if (isPiloting || isSolvingCablePuzzle || isSolvingTerminalPuzzle)
        {
            isInteracting = true;
        }
        else
        {
            isInteracting = false;
        }

        // set velocityX to 0 to prevent charcter continuing to move
        if (isInteracting)
        {
            velocityX = 0;
        }



        //-----------------------------------------------
        handleMovementLogic();

        //-----------------------------------------------
        // if player is grounded normal motion is applied
        if (isGrounded)       
        {

            handleRotation();

            if (!isInteracting)
            {


                if (!animationHandler.inPilotingTransition && !animationHandler.inPuzzleTransition)
                {
                    // Action Movement
                    handleJump();       // jump
                    handleCrouch();     // crouch

                    // Basic Movement
                    handleSteps();
                    handleDirection();
                    handleVelocity();


                }
            }


        }


        //-----------------------------------------
        // variables for the animator
        if (isGrounded)
        {
            velocityY = 0; 
        }

        if (!isGrounded)
        {
            velocityY = rb.velocity.y;
        }
        
        normalizedVelocity = rb.velocity.normalized;


        //------------------------------------
        // Final Velocity applied to RigidBody
        //------------------------------------
        rb.velocity = new Vector3 (0, rb.velocity.y, velocityX);
        //------------------------------------

    }


    //-----------------------------------------------
    //-------------------Handlers--------------------
    //-----------------------------------------------
    private void handleMovementLogic()
    {
        //-----------------------------------
        // check if player is grounded
        isGrounded = groundCheck.isGrounded();

        //-----------------------------------
        // Check Jumping State
        // Jumping | going up
        if (!isGrounded && normalizedVelocity.y > 0)
        {
            isJumping = true;
            isFalling = false;
        }

        // Jumping | going down
        if (!isGrounded && normalizedVelocity.y < 0)
        {
            isFalling = true;
            isJumping = false;
        }
        //-----------------------------------

        //-----------------------------------
        // Check Crouched State
        if (crouchToggle > 1) { crouchToggle = 0; }          // reset crouch
        if (crouchToggle == 0) { isCrouched = false; }
        if (crouchToggle == 1) { isCrouched = true; }
        if (isCrouched && isFalling) { crouchToggle += 1; }  // disable crouch if jumping

    }

    //-----------------------------------------------
    private void handleCrouch()
    {
        // change mass when crouched as box collider is half the size
        // prevents jumping jigher when crouched
        if (isCrouched)
        {
            // if crouched
            rb.centerOfMass = new Vector3(0, 0.9f, 0);
            boxCollider.center = new Vector3(0, 0.64f, 0);
            boxCollider.size = new Vector3(0.5f, 1.3f, 0.75f);
        }

        if (!isCrouched)
        {
            // if standing
            new Vector3(0, 0.9f, 0);
            boxCollider.center = new Vector3(0, 0.89f, 0);
            boxCollider.size = new Vector3(0.5f, 1.8f, 0.75f);
        }

    }

    //-----------------------------------------------
    private void handleJump()
    {
        if (isGrounded)
        {
            isJumping = false;
            isFalling = false;
        }

        // add force to rigid body to implement jump
        if (input.isJumpPressed)
        {
            rb.AddForce(jumpForce * Vector3.up, ForceMode.Impulse);
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
        // if  walking | running | idle
        if (!input.isRunPressed && !isCrouched)
        {
            maxVelocity = walkSpeed;
            acceleration = walkAcceleration;
            decceleration = walkDecceleration;
        }


        if (input.isRunPressed && !isCrouched)
        {
            maxVelocity = runSpeed;
            acceleration = runAcceleration;
            decceleration = runDecceleration;
        }


        if (isCrouched)
        {
            maxVelocity = crouchSpeed;
            acceleration = crouchAcceleration;
            decceleration = crouchDecceleration;

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

        if (input.isMovementPressed && !isInteracting)
        {
            Quaternion targetRotation = Quaternion.LookRotation(positionToLookAt);
            rb.MoveRotation(Quaternion.Slerp(currentRotation, targetRotation, rotationFactorPerFrame * Time.deltaTime));
            //Debug.Log(currentRotation);
            
        } 
        else if (isPiloting)
        {
            Quaternion targetRotation = Quaternion.LookRotation(new Vector3(0, 0, -1));
            rb.MoveRotation(Quaternion.Slerp(currentRotation, targetRotation, rotationFactorPerFrame * Time.deltaTime));

        }
        else if (isSolvingCablePuzzle || isSolvingTerminalPuzzle)
        {
            //Quaternion targetRotation = Quaternion.LookRotation(new Vector3(0, 0, rotation));
            Quaternion targetRotation = new Quaternion(0, -.75f, 0, .75f);
            rb.MoveRotation(Quaternion.Slerp(currentRotation, targetRotation, rotationFactorPerFrame * Time.deltaTime));
        }

    }

    //-----------------------------------------------
    // from tutorial https://www.youtube.com/watch?v=DrFk5Q_IwG0
    private void handleSteps()
    {
        // check lower step using raycast
        RaycastHit hitLower;
        if (Physics.Raycast(stepRayLower.transform.position, transform.transform.TransformDirection(Vector3.forward), out hitLower, lowerRaycastDistance, platform))
        {

            // check higher step is not hitting anything using raycast
            RaycastHit hitUpper;
            if (!Physics.Raycast(stepRayUpper.transform.position, transform.transform.TransformDirection(Vector3.forward), out hitUpper, upperRaycastDistance, platform))
            {

                //rb.position += new Vector3(0f, stepSmooth, 0f);
                Vector3 interpolatedPosition = Vector3.Lerp(rb.position, rb.position - new Vector3(0f, -stepSmooth, 0f), Time.fixedDeltaTime * 20);
                rb.MovePosition(interpolatedPosition);
            }

        }

        //// check diagonal steps, if player is in mid turn
        //RaycastHit hitLower45;
        //if (Physics.Raycast(stepRayLower.transform.position, transform.transform.TransformDirection(1.25f,0,1), out hitLower45, lowerRaycastDistance, platform))
        //{
        //    RaycastHit hitUpper45;
        //    if (!Physics.Raycast(stepRayUpper.transform.position, transform.transform.TransformDirection(1.25f,0,1), out hitUpper45, upperRaycastDistance, platform))
        //    {
        //        rb.position -= new Vector3(0f, -stepSmooth, 0f);
        //    }
        //}

        //RaycastHit hitLowerMinus45;
        //if (Physics.Raycast(stepRayLower.transform.position, transform.transform.TransformDirection(-1.25f, 0, 1), out hitLowerMinus45, lowerRaycastDistance, platform))
        //{
        //    RaycastHit hitUpperMinus45;
        //    if (!Physics.Raycast(stepRayUpper.transform.position, transform.transform.TransformDirection(-1.25f, 0, 1), out hitUpperMinus45,upperRaycastDistance, platform))
        //    {
        //        rb.position -= new Vector3(0f, -stepSmooth, 0f);
        //    }
        //}
    }


}//--------------------
