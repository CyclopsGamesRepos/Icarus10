using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundCheck : MonoBehaviour
{
    [SerializeField] Transform groundCheck;
    [SerializeField] LayerMask platform;

    //------------------------------
    public bool IsGrounded()
    {
        return Physics.CheckSphere(groundCheck.position, .2f, platform);
    }
}
