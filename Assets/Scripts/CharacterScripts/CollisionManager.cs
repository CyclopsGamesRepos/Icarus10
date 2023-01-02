using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CollisionManager : MonoBehaviour
{
    public bool inCockpit;

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("Entered: " + other.gameObject.tag);

        if (other.gameObject.tag == "Cockpit")
        {
            inCockpit = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        //Debug.Log("Exited: " + other.gameObject.tag);
        if (other.gameObject.tag == "Cockpit")
        {
            inCockpit = false;
        }
    }


}
