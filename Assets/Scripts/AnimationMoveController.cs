using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;


public class AnimationMoveController : MonoBehaviour
{
    // declare references;
    private InputControl inputControl;
    private CharacterController characterController;
    private Animator animator;
    private Rigidbody rb;



    // player movement input variables;
    [Header("Movement")]
    private Vector3 input;
    private float currentMovementInput;
    private float currentMovement;
    private float currentRunMovement;
    [SerializeField] private float walkSpeed = 1.4f;
    [SerializeField] private float runSpeed = 3.4f;

    // Character rotation variables;
    [Header("Rotation")]
    [SerializeField] private float rotationFactorPerFrame = 8f;
    private Vector3 positionToLookAt;
    [SerializeField] private Quaternion currentQ;

    // Animation Variables
    [Header("Animation Bools")]
    [SerializeField] private bool isMovementPressed;
    [SerializeField] private bool isRunPressed;

    // Parameter Hashing - Performance Optimization
    int isWalkingHash;
    int isRunningHash;

    // set climb variables;
    [Header("Player Step Climb")]
    [SerializeField] private GameObject stepRayUpper;
    [SerializeField] private GameObject stepRayLower;
    [SerializeField] private float stepHeight = 0.3f;
    [SerializeField] private float stepSmooth = 0.1f;

    //-----------------------------------------------
    private void Awake()
    {
        // Parameter Hashing
        isWalkingHash = Animator.StringToHash("isWalking");
        isRunningHash = Animator.StringToHash("isRunning");

        // initially set reference variables;
        inputControl = new InputControl();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();

        characterController = GetComponent<CharacterController>();// may not need this??

        // callback function for movement input;
        inputControl.CharacterControls.Move.started += onMovementInput;
        inputControl.CharacterControls.Move.canceled += onMovementInput;

        // callback function for Run input;
        inputControl.CharacterControls.Run.started += onRun;
        inputControl.CharacterControls.Run.canceled += onRun;

        // step climb setup / set y position of raycast to stepHeight;
        stepRayUpper.transform.localPosition = new Vector3(stepRayUpper.transform.position.x, stepHeight, stepRayUpper.transform.position.z);
    }


    //-----------------------------------------------
    //-------------------Update----------------------
    //-----------------------------------------------
    private void Update()
    {
        handleAnimation();
    }

    //-----------------------------------------------
    private void FixedUpdate()
    {
        //--------------
        // Call Handlers
        handleRotation();
        handleSteps();
        //--------------

        if (isRunPressed)
        {
            input = new Vector3(0, 0, currentRunMovement * Time.fixedDeltaTime);
        } 
        else
        {
            input = new Vector3(0, 0, currentMovement * Time.fixedDeltaTime);
        }

        
        // apply movement input to RigidBody
        rb.MovePosition(rb.position + input);
    }


    //-----------------------------------------------
    //-------------------Handlers--------------------
    //-----------------------------------------------
    private void onMovementInput(InputAction.CallbackContext context)
    {
        // assign movement input to variable / Walk;
        currentMovementInput = context.ReadValue<float>();
        currentMovement = currentMovementInput * walkSpeed;
        currentRunMovement = currentMovementInput * runSpeed;

        // check if movement has been pressed;
        isMovementPressed = currentMovement != 0;

    }

    //-----------------------------------------------
    private void onRun(InputAction.CallbackContext context)
    {
        isRunPressed = context.ReadValueAsButton();
    }

    //-----------------------------------------------
    private void handleAnimation()
    {
        // get parameter values from animator
        bool isWalking = animator.GetBool(isWalkingHash);
        bool isRunning = animator.GetBool(isRunningHash);

        //----------------------------------------------
        // Walking Conditionals
        if (isMovementPressed && !isWalking)
        {
            animator.SetBool(isWalkingHash, true);
        }
        else if(!isMovementPressed && isWalking)
        {
            animator.SetBool(isWalkingHash, false);
        }


        //----------------------------------------------
        // Running Conditionals
        if ((isMovementPressed && isRunPressed) && !isRunning)
        {
            animator.SetBool(isRunningHash, true);
        }
        else if ((!isMovementPressed || !isRunPressed) && isRunning)
        {
            animator.SetBool(isRunningHash, false);
        }
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


        // Debug Check Current Rotation
        currentQ = rb.rotation;
    }

    //-----------------------------------------------
    // from tutorial https://www.youtube.com/watch?v=DrFk5Q_IwG0
    private void handleSteps()
    {
        // check step using raycast
        RaycastHit hitLower;
        if (Physics.Raycast(stepRayLower.transform.position, transform.transform.TransformDirection(Vector3.forward), out hitLower, 0.1f))
        {
            RaycastHit hitUpper;
            if (!Physics.Raycast(stepRayUpper.transform.position, transform.transform.TransformDirection(Vector3.forward), out hitUpper, 0.2f))
            {
                rb.position -= new Vector3(0f, -stepSmooth, 0f);
            }
        }

        // check diagonal steps, if player is in mid turn
        RaycastHit hitLower45;
        if (Physics.Raycast(stepRayLower.transform.position, transform.transform.TransformDirection(1.5f,0,1), out hitLower45, 0.1f))
        {
            RaycastHit hitUpper45;
            if (!Physics.Raycast(stepRayUpper.transform.position, transform.transform.TransformDirection(1.5f,0,1), out hitUpper45, 0.2f))
            {
                rb.position -= new Vector3(0f, -stepSmooth, 0f);
            }
        }

        RaycastHit hitLowerMinus45;
        if (Physics.Raycast(stepRayLower.transform.position, transform.transform.TransformDirection(-1.5f, 0, 1), out hitLowerMinus45, 0.1f))
        {
            RaycastHit hitUpperMinus45;
            if (!Physics.Raycast(stepRayUpper.transform.position, transform.transform.TransformDirection(-1.5f, 0, 1), out hitUpperMinus45, 0.2f))
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
