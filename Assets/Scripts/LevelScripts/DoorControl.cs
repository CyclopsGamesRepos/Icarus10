using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class DoorControl : MonoBehaviour
{
    bool isOpening;
    Transform closedPosition;
    Transform openPosition;
    Transform door;

    [SerializeField] float speed = .25f;
    Vector3 doorPosition;

    private void Awake()
    {
        // get child transform positions;
        door = this.gameObject.transform.GetChild(0);
        closedPosition = this.gameObject.transform.GetChild(1);
        openPosition = this.gameObject.transform.GetChild(2);
    }

    private void FixedUpdate()
    {
        // get current door position if playermoves back into trigger before door has opened/ closed
        doorPosition = door.position;

        if (isOpening)
        {
            door.position = Vector3.Lerp(door.position, openPosition.position, Time.fixedDeltaTime * speed);
        }

        if(!isOpening)
        {
            door.position = Vector3.Lerp(door.position, closedPosition.position, Time.fixedDeltaTime * speed);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            isOpening = true;
            GetComponent<AudioSource>().Play();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            isOpening = false;
        }
    }
}
