using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lazer : MonoBehaviour
{
    private ControllerInputManager input;
    private CharacterControllerRB controller;
    private bool isShooting;

    // create list of child components
    [SerializeField] List<Transform> transformList;

    // get / set transforms
    [SerializeField] private Transform getLazerTransform;
    [SerializeField] private Transform LazerPosition;
    [SerializeField] private GameObject LazerPrefab;
    [SerializeField] private LineRenderer linePos;

    [SerializeField] private GameObject extinhuishPS;
    [SerializeField] private GameObject sparkPS;

    private GameObject spawnedLazer;


    [SerializeField] float hitDistance;

    [SerializeField] float offsetAmount;

    // set mask 
    [SerializeField] LayerMask layerMask;
    [SerializeField] LayerMask layerMaskBorder;

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

        spawnedLazer = Instantiate(LazerPrefab, LazerPosition) as GameObject;
        linePos = spawnedLazer.gameObject.GetComponentInChildren<LineRenderer>();

        disableLazer();
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
                //Debug.DrawRay(LazerPosition.position, LazerPosition.up, Color.yellow);

                enableLazer();
                updateLazer();


                //-----------------------------------------------------------------------------------------------------------------------
                // this raycast will only hit the fire
                RaycastHit hit;
                if (Physics.Raycast(LazerPosition.position, LazerPosition.up, out hit, Mathf.Infinity, LayerMask.GetMask("LazerReciever")))
                {
                    if (hit.transform.gameObject.tag == "Fire")
                    {
                        hit.transform.SendMessage("hitDetected");

                        //Debug.Log("Hit :" + hit.collider.name);
                        //--- draw debug target line form turret head to player postion
                        //Debug.DrawRay(LazerPosition.position, LazerPosition.up, Color.red);

                        Instantiate(extinhuishPS, hit.point, Quaternion.Inverse(LazerPosition.rotation));

                    }

                    if (hit.transform.gameObject.tag == "LazerBorder")
                    {
                        

                        //Debug.Log("Hit :" + hit.collider.name);
                        //--- draw debug target line form turret head to player postion

                        Instantiate(sparkPS, hit.point, Quaternion.Inverse(LazerPosition.rotation) );
                        
                       

                    }

                    linePos.SetPosition(1, new Vector3(0, hit.distance, 0));

                } 

            }

            if (!input.isShootPressed)
            {
                disableLazer();
            }
        }

    }


    //--------------------------------
    void enableLazer()
    {
        spawnedLazer.SetActive(true);

        if (!isShooting)
        {
            GetComponent<AudioSource>().Play();
            isShooting = true;
        }
    }

    //--------------------------------
    void disableLazer()
    {
        isShooting = false;
        spawnedLazer.SetActive(false);
        GetComponent<AudioSource>().Stop();
    }

    //--------------------------------
    void updateLazer()
    {
        if (LazerPosition != null)
        {
            //Vector3 offsetPosition = new Vector3(LazerPosition.position.x, LazerPosition.position.y, LazerPosition.position.z).normalized;
            
            spawnedLazer.transform.position = LazerPosition.position;
            //spawnedLazer.transform.rotation = LazerPosition.transform.rotation;
            // set the position of the laser

            //Debug.Log(LazerPosition.position);
           

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
