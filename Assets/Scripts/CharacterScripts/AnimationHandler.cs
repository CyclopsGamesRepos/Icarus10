using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AnimationHandler : MonoBehaviour
{
    // References
    private CharacterControllerRB characterController;
    private Animator animator;
    private ControllerInputManager controllerInputManager;

    // Parameter Hashing - Performance Optimization
    private int velocityXHash;
    private int velocityYHash;

    // basic movement
    private int isGroundedHash;
    private int isJumpingHash;
    private int isFallingHash;
    private int isCrouchedHash;
    private int isPilotingHash;

    // Cable Puzzle
    private int isSolvingCablePuzzleHash;

    // Terminal Puzzle
    private int terminalPuzzleHash;
    private float terminalBoolToFloat;
    private float terminalLayerWeight;

    // Aiming
    private int aimHeightBlendHash;
    public float aimHeightBlendAmount = 1f;
    private float aimHeightStartPosition = 1f;
    //public float aimHeight = 1f;
    [SerializeField] float aimSpeed = 0.5f;
    [SerializeField] float maxAim = 1.8f;
    [SerializeField] float minAim = 0.25f;

    private float aimingBoolToFloat;
    private float aimingLayerWeight;


    //--------------------
    public bool standCheck;
    public bool idleCheck;
    public float timeCheck;

    //-----------------------------------------------
    private void Awake()
    {

        //----------------------------------------------------------
        // Get CharacterControllerRB Component
        characterController = GetComponent<CharacterControllerRB>();

        // Get Animator Component
        animator = GetComponent<Animator>();

        // Get Controller Input Manager
        controllerInputManager = GetComponent<ControllerInputManager>();

        //----------------------------------------------------------
        // Parameter Hashing movement
        velocityXHash = Animator.StringToHash("velocityX");
        velocityYHash = Animator.StringToHash("velocityY");

        // Parameter Hashing Action
        isGroundedHash = Animator.StringToHash("isGrounded");
        isJumpingHash = Animator.StringToHash("isJumping");
        isFallingHash = Animator.StringToHash("isFalling");
        isCrouchedHash = Animator.StringToHash("isCrouched");

        // Parameter Hashing Interaction
        isPilotingHash = Animator.StringToHash("isPiloting");
        isSolvingCablePuzzleHash = Animator.StringToHash("isSolvingCablePuzzle");


        aimHeightBlendHash = Animator.StringToHash("aimHeightBlend");
        
        //----------------------------------------------------------


    }

    //-----------------------------------------------
    private void FixedUpdate()
    {

        //standCheck = animator.GetCurrentAnimatorStateInfo(0).IsName("Sit to Stand");
        //idleCheck = animator.GetCurrentAnimatorStateInfo(0).IsName("Idle");
        //Debug.Log(animator.IsInTransition(0));

        //if (animator.GetCurrentAnimatorStateInfo(0).IsName("Sit to Stand"))
        //{
        //    timeCheck = animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
        //}
        //if (!characterController.isSitting)
        //{
        //    if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1 && !animator.IsInTransition(0))
        //    {
        //        characterController.isStanding = true;
        //    }
        //}
     
        // transition into aim
        if (characterController.isAiming)
        {
            aimingBoolToFloat = 1f; 
        }

        //---------------------------------
        // Reset Aim Position
        if (!characterController.isAiming)
        {
            aimingBoolToFloat = 0f;
            aimHeightBlendAmount = 1f;
            animator.SetFloat(aimHeightBlendHash, aimHeightStartPosition);
        }

        float currentAimingLayerWeight = animator.GetLayerWeight(animator.GetLayerIndex("AimingLayer"));
        aimingLayerWeight = Mathf.Lerp(currentAimingLayerWeight, aimingBoolToFloat, Time.fixedDeltaTime * 4f);
        animator.SetLayerWeight(animator.GetLayerIndex("AimingLayer"), aimingLayerWeight);


            if (aimHeightBlendAmount <= maxAim && aimHeightBlendAmount >= minAim)
            {
                if (controllerInputManager.isAimPressed)
                {
                    aimHeightBlendAmount += controllerInputManager.currentAim * (Time.fixedDeltaTime * aimSpeed);
                    animator.SetFloat(aimHeightBlendHash, aimHeightBlendAmount);
                }

                if (aimHeightBlendAmount > maxAim) { aimHeightBlendAmount = maxAim; }
                if (aimHeightBlendAmount < minAim) { aimHeightBlendAmount = minAim; }
            }

     


        //---------Constant checks need to control animator logic-----------------
        // set bool isGrounded in animator
        animator.SetBool(isGroundedHash, characterController.isGrounded);

        // use rb velocity y to check if player is jumping
        animator.SetFloat(velocityYHash, Mathf.Abs(characterController.velocityY));
        //------------------------------------------------------------------------


        //--------------------------Interaction Animations------------------------
        handlePuzzles();
        handlePilotAnimation();

        // Movement animations
        handleMovementAnimation();
        handleJumpAnimation();
        //------------------------------------------------------------------------
    }

    //-----------------------------------------------
    private void handleMovementAnimation()
    {
        // apply movement velocity to blend tree 
        animator.SetFloat(velocityXHash, Mathf.Abs(characterController.velocityX));

        // set bool is crouched in animator
        animator.SetBool(isCrouchedHash, characterController.isCrouched);
    }

    //-----------------------------------------------
    private void handleJumpAnimation()
    {
        // Set jumping animation
        animator.SetBool(isJumpingHash, characterController.isJumping);

        //set falling animation
        animator.SetBool(isFallingHash, characterController.isFalling);
    }

    //-----------------------------------------------
    private void handlePilotAnimation()
    {
        // set bool isSitting in animator
        animator.SetBool(isPilotingHash, characterController.isPiloting);

    }


    //-----------------------------------------------
    private void handlePuzzles()
    {

        //----------------------------------------------------------------------------------------
        // Solving cable puzzle animation
        animator.SetBool(isSolvingCablePuzzleHash, characterController.isSolvingCablePuzzle);


        //----------------------------------------------------------------------------------------
        // Solving terminal puzzle animation
        if (characterController.isSolvingTerminalPuzzle)
        {
            terminalBoolToFloat = 1f;
        }

        if (!characterController.isSolvingTerminalPuzzle)
        {
            terminalBoolToFloat = 0f;
        }

        float currentTerminalLayerWeight = animator.GetLayerWeight(animator.GetLayerIndex("TerminalPuzzleLayer"));

        terminalLayerWeight = Mathf.Lerp(currentTerminalLayerWeight, terminalBoolToFloat, Time.fixedDeltaTime * 1.8f);

        animator.SetLayerWeight(animator.GetLayerIndex("TerminalPuzzleLayer"), terminalLayerWeight);
    }


}//-------------------
