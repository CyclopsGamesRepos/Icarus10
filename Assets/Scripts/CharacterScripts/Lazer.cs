using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lazer : MonoBehaviour
{
    private ControllerInputManager input;
    private CharacterControllerRB controller;

    // create list of child components
    [SerializeField] List<Transform> transformList;

    // get / set transforms
    [SerializeField] private Transform getLazerTransform;
    [SerializeField] private Transform LazerPosition;

    [SerializeField] float hitDistance;

    // set mask 
    [SerializeField] LayerMask layerMask;

    //-------------------------------
    private void Awake()
    {
        // create a list of transforms
        getListOfChildren();

        input = GetComponent<ControllerInputManager>();
        controller = GetComponent<CharacterControllerRB>();

        // assign transforms
        getLazerTransform = transformList[48];
        LazerPosition = transformList[71];
    }

    //--------------------------------
    void FixedUpdate()
    {
        // set lazer object transform to right index finger transform
        LazerPosition.SetPositionAndRotation(getLazerTransform.position, getLazerTransform.rotation);

        if (controller.isAiming) // check if character is aiming
        {
            //--- draw debug target line form turret head to player postion
            Debug.DrawRay(LazerPosition.position, LazerPosition.up, Color.blue);

            if (input.isShootPressed) // check if controlelr inpur nhas been pressed
            {
                //--- draw debug target line form turret head to player postion
                Debug.DrawRay(LazerPosition.position, LazerPosition.up, Color.yellow);

                RaycastHit hit;
                if (Physics.Raycast(LazerPosition.position, LazerPosition.up, out hit, Mathf.Infinity, LayerMask.GetMask("LazerReciever")))
                {
                    if (hit.transform.gameObject.tag == "Fire")
                    {
                        hit.transform.SendMessage("hitDetected");

                        //--- draw debug target line form turret head to player postion
                        Debug.DrawRay(LazerPosition.position, LazerPosition.up, Color.red);
                    }
                }
            }
        }

    }


    //--------------------------------
    public void getListOfChildren()
    {
        //get a list of child game objects
        transformList = new List<Transform>();
        gameObject.GetComponentsInChildren<Transform>(transformList);

    }
}
