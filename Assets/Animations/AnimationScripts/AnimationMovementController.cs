using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;

public class AnimationMovementController : MonoBehaviour
{
    // References
    private PlayerInput playerInput;
    private CharacterController characterController;
    private Animator animator;

    // input values / movement and rotation
    private Vector2 currentMovementInput;
    private Vector3 currentMovement;
    private Vector3 currentRunMovement;
    private bool isMovementPressed;
    private bool isRunPressed;

    // constants
    private float runMultiplier = 3.0f;
    private float rotationFactorPerFrame = 15.0f;
    int zero = 0;

    // gravity
    float groundedGravity = -.5f;
    float gravity = -9.8f;

    // input values / jump
    bool isJumpPressed = false;
    bool isJumping = false;
    float initialJumpVelocity;
    float timeToApex;
    float maxJumpHeight = 4.0f;
    float maxJumpTime = 0.75f;
    bool isJumpAnimating = false;


    // parameter hashing
    int isWalkingHash;
    int isRunningHash;
    int isJumpingHash;

    //--------------------------------------------------------------------
    private void Awake()
    {
        //-----------------------------
        // set reference variables
        playerInput = new PlayerInput();
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        //-----------------------------
        // set hashing to improve performance
        isWalkingHash = Animator.StringToHash("isWalking");
        isRunningHash = Animator.StringToHash("isRunning");
        isJumpingHash = Animator.StringToHash("isJumping");

        //-----------------------------
        // callback function key pressed / released
        playerInput.CharacterControls.Move.started += OnMovementInput;
        playerInput.CharacterControls.Move.canceled += OnMovementInput;

        //-----------------------------
        // callback function control stick
        playerInput.CharacterControls.Move.performed += OnMovementInput;

        //-----------------------------
        // callback function run button pressed / released
        playerInput.CharacterControls.Run.started += OnRun;
        playerInput.CharacterControls.Run.canceled += OnRun;

        //-----------------------------
        // callback function jump pressed / released
        playerInput.CharacterControls.Jump.started += OnJump;
        playerInput.CharacterControls.Jump.canceled += OnJump;

        //-----------------------------
        SetupJumpVariables();
    }

    //--------------------------------------------------------------------                                                      
    void SetupJumpVariables()
    {
        timeToApex = maxJumpTime / 2;
        gravity = (-2 * maxJumpHeight) / Mathf.Pow(timeToApex, 2);
        initialJumpVelocity = (2 * maxJumpHeight) / timeToApex;
    }

    //--------------------------------------------------------------------                                                      
    void OnMovementInput(InputAction.CallbackContext context)
    {
        currentMovementInput = context.ReadValue<Vector2>();
        currentMovement.z = currentMovementInput.x;
        currentRunMovement.z = currentMovementInput.x * runMultiplier;
        isMovementPressed = currentMovementInput.x != zero || currentMovementInput.y != zero;
    }

    //--------------------------------------------------------------------
    void OnRun(InputAction.CallbackContext context)
    {
        isRunPressed = context.ReadValueAsButton();
    }

    //--------------------------------------------------------------------
    void OnJump(InputAction.CallbackContext context)
    {
        isJumpPressed = context.ReadValueAsButton();
    }

    //--------------------------------------------------------------------
    void HandleJump()
    {
        if (!isJumping && characterController.isGrounded && isJumpPressed)
        {
            animator.SetBool(isJumpingHash, true);
            isJumpAnimating = true;
            isJumping = true;
            currentMovement.y = initialJumpVelocity * .5f;
            currentRunMovement.y = initialJumpVelocity * .5f;
        } 
        else if (!isJumpPressed && isJumping && characterController.isGrounded)
        {
            isJumping = false;
        }
    }

    //--------------------------------------------------------------------        
    void HandleRotation()
    {
        Vector3 positionToLookAt;
        positionToLookAt.x = currentMovement.x;
        positionToLookAt.y = zero;
        positionToLookAt.z = currentMovement.z;

        Quaternion currentRotation = transform.rotation;

        if (isMovementPressed)
        {
            Quaternion targetRotation = Quaternion.LookRotation(positionToLookAt);
            transform.rotation = Quaternion.Slerp(currentRotation, targetRotation, rotationFactorPerFrame * Time.deltaTime);
        }
    }

    //--------------------------------------------------------------------
    void HandleGravity()
    {
        bool isFalling = currentMovement.y <= 0.0f || !isJumpPressed;
        float fallMultiplier = 2.0f;

        if (characterController.isGrounded)
        {
            if (isJumpAnimating)
            {
                animator.SetBool(isJumpingHash, false);
                isJumpAnimating = false;
            }

            currentMovement.y = groundedGravity;
            currentRunMovement.y = groundedGravity;
        } 
        else if (isFalling)
        {
            float previousVelocityY = currentMovement.y;
            float newVelocityY = currentMovement.y + (gravity * fallMultiplier * Time.deltaTime);
            float nextVelocityY = (previousVelocityY + newVelocityY) * .5f;
            currentMovement.y = nextVelocityY;
            currentRunMovement.y = nextVelocityY;
        }
        else
        {
            float previousVelocityY = currentMovement.y;
            float newVelocityY = currentMovement.y + (gravity * Time.deltaTime);
            float nextVelocityY = (previousVelocityY + newVelocityY) * .5f;
            currentMovement.y = nextVelocityY;
            currentRunMovement.y = nextVelocityY;
        }
    }

    //--------------------------------------------------------------------
    void HandleAnimation()
    {
        bool isWalking = animator.GetBool(isWalkingHash);
        bool isRunning = animator.GetBool(isRunningHash);

        // set walking animation
        if (isMovementPressed && !isWalking)
        {
            animator.SetBool(isWalkingHash, true);
        }

        else if(!isMovementPressed && isWalking)
        {
            animator.SetBool(isWalkingHash, false);
        }

        // set running animation
        if((isMovementPressed && isRunPressed) && !isRunning)
        {
            animator.SetBool(isRunningHash, true);
        }
        else if ((!isMovementPressed || !isRunPressed) && isRunning)
        {
            animator.SetBool(isRunningHash, false);
        }
    }

    //--------------------------------------------------------------------
    void Update()
    {
        HandleRotation();
        HandleAnimation();

        if (isRunPressed)
        {
            characterController.Move(currentRunMovement * Time.deltaTime);
        }
        else
        {
            characterController.Move(currentMovement * Time.deltaTime);
        }
 
        HandleGravity();
        HandleJump();
    }

    //--------------------------------------------------------------------
    private void OnEnable()
    {
        playerInput.CharacterControls.Enable();
    }

    //--------------------------------------------------------------------
    private void OnDisable()
    {
        playerInput.CharacterControls.Disable();
    }
}
