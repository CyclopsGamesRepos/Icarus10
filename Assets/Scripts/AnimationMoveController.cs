using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;


public class AnimationMoveController : MonoBehaviour
{
    // declare references;
    InputControl inputControl;
    CharacterController characterController;
    Animator animator;
    Rigidbody rb;

    // player movement input variables;
    float currentMovementInput;
    float currentMovement;
    float currentRunMovement;
    [SerializeField] float walkSpeed = 1.2f;
    [SerializeField] float runSpeed = 3.0f;
    Vector3 input;
    bool isMovementPressed;
    bool isRunPressed;

    // Character rotation variables;
    float rotationFactorPerFrame = 5f;
    Vector3 positionToLookAt;

    // set climb variables;
    [Header("Player Step Climb")]
    [SerializeField] GameObject stepRayUpper;
    [SerializeField] GameObject stepRayLower;
    [SerializeField] float stepHeight = 0.3f;
    [SerializeField] float stepSmooth = 0.1f;

    //-----------------------------------------------
    private void Awake()
    {
        //initially set reference variables;
        inputControl = new InputControl();
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();

        // callback function for movement input;
        inputControl.CharacterControls.Move.started += onMovementInput;
        inputControl.CharacterControls.Move.canceled += onMovementInput;

        // callback function for Run input;
        inputControl.CharacterControls.Move.started += onRun;
        inputControl.CharacterControls.Move.canceled += onRun;

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
        handleRotation();

        input = new Vector3(0, 0, currentMovement * (walkSpeed * Time.fixedDeltaTime));

        rb.MovePosition(rb.position + input);

        handleSteps();
    }


    //-----------------------------------------------
    //-------------------Handlers--------------------
    //-----------------------------------------------
    void onMovementInput(InputAction.CallbackContext context)
    {
        // assign movement input to variable / Walk;
        currentMovementInput = context.ReadValue<float>();
        currentMovement = currentMovementInput;

        // check if movement has been pressed;
        isMovementPressed = currentMovement != 0;
    }

    //-----------------------------------------------
    void onRun(InputAction.CallbackContext context)
    {
        isRunPressed = context.ReadValueAsButton();
    }

    //-----------------------------------------------
    void handleAnimation()
    {
        // get parameter values from animator
        bool isWalking = animator.GetBool("isWalking");
        bool isRunning = animator.GetBool("isRunning");

        if (isMovementPressed && !isWalking)
        {
            animator.SetBool("isWalking", true);
        }
        else if(!isMovementPressed && isWalking)
        {
            animator.SetBool("isWalking", false);
        }
    }

    //-----------------------------------------------
    void handleRotation()
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
    void handleSteps()
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
