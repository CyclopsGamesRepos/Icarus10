using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lazer : MonoBehaviour
{
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

        // assign transforms
        getLazerTransform = transformList[48];
        LazerPosition = transformList[71];
    }

    //--------------------------------
    void FixedUpdate()
    {
        // set lazer object transform to right index finger transform
        LazerPosition.SetPositionAndRotation(getLazerTransform.position, getLazerTransform.rotation);

       

        RaycastHit hit;
        if (Physics.Raycast(LazerPosition.position, LazerPosition.up, out hit, Mathf.Infinity, LayerMask.GetMask("LazerReciever")))
        {
            hitDistance = hit.distance;

            Debug.Log("Hit Fire");
        }
        //Debug.Log(hit.collider);

        //--- draw debug target line form turret head to player postion
        Debug.DrawRay(LazerPosition.position, LazerPosition.up, Color.blue);


    }

    //--------------------------------
    public void getListOfChildren()
    {
        //get a list of child game objects
        transformList = new List<Transform>();
        gameObject.GetComponentsInChildren<Transform>(transformList);

    }
}
