using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationHandler : MonoBehaviour
{
    // References
    private CharacterControllerRB characterController;
    private Animator animator;

    // Parameter Hashing - Performance Optimization
    private int velocityXHash;
    private int velocityYHash;

    private int isGroundedHash;
    private int isJumpingHash;
    private int isFallingHash;
    private int isCrouchedHash;

    private int isSittingHash;
    private int isPilotingHash;

    private float pilotingBlend;

    //-----------------------------------------------
    private void Awake()
    {
        // Get CharacterControllerRB Component
        characterController = GetComponent<CharacterControllerRB>();

        // Get Animator Component
        animator = GetComponent<Animator>();

        // Parameter Hashing movement
        velocityXHash = Animator.StringToHash("velocityX");
        velocityYHash = Animator.StringToHash("velocityY");

        // Parameter Hashing Action
        isGroundedHash = Animator.StringToHash("isGrounded");
        isJumpingHash = Animator.StringToHash("isJumping");
        isFallingHash = Animator.StringToHash("isFalling");
        isCrouchedHash = Animator.StringToHash("isCrouched");
        isSittingHash = Animator.StringToHash("isSitting");
        isPilotingHash = Animator.StringToHash("isPiloting");
    }

    //-----------------------------------------------
    private void FixedUpdate()
    {
        // use rb velocity y to check if player is jumping
        animator.SetFloat(velocityYHash, Mathf.Abs(characterController.velocityY));

        // set bool isGrounded in animator
        animator.SetBool(isGroundedHash, characterController.isGrounded);

        // set bool is crouched in animator
        animator.SetBool(isCrouchedHash, characterController.isCrouched);

        // set bool isSitting in animator
        animator.SetBool(isSittingHash, characterController.isSitting);

        if (characterController.isSitting)
        {
            pilotingBlend += Time.fixedDeltaTime;
        }

        if (pilotingBlend > 36)
        {
            pilotingBlend = 0;
        }

        animator.SetFloat(isPilotingHash, pilotingBlend);



        handleMovementAnimation();
        handleJumpAnimation();
    }

    //-----------------------------------------------
    private void handleMovementAnimation()
    {
       
        if (characterController.isGrounded)
        {
            // apply movement velocity to blend tree 
            animator.SetFloat(velocityXHash, Mathf.Abs(characterController.velocityX));
        }

    }

    //-----------------------------------------------
    private void handleJumpAnimation()
    {
        // Set jumping animation
        animator.SetBool(isJumpingHash, characterController.isJumping);

        //set falling animation
        animator.SetBool(isFallingHash, characterController.isFalling);

    }


}
