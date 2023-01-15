using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder.Shapes;

public class SeatControl : MonoBehaviour
{


    [SerializeField] float sitSpeed = .25f;
    [SerializeField] float standSpeed = .25f;
    [SerializeField] int powAmount = 6;
    [SerializeField] float zOffset = .1f;
    [SerializeField] float yOffset = .1f;
    [SerializeField] GameObject player;

    CharacterControllerRB characterControllerRB;
    Transform startPosition;
    Transform pilotPosition;
    Transform seat;
    Transform hipPosition;

    bool isOpening;
    Vector3 seatPosition;
    Quaternion currentRotation;


    [SerializeField] bool _isPiloting;

    private void Awake()
    {
        characterControllerRB = player.GetComponent<CharacterControllerRB>();
        // get child transform positions;
        seat = this.gameObject.transform.GetChild(0);
        startPosition = this.gameObject.transform.GetChild(1);
        pilotPosition = this.gameObject.transform.GetChild(2);

        seat.position = startPosition.position;
        seat.rotation = startPosition.rotation;

    }

    private void FixedUpdate()
    {
        _isPiloting = characterControllerRB.isPiloting;
        hipPosition = player.transform.GetChild(2);


        // get current door position if playermoves back into trigger before door has opened/ closed
        seatPosition = seat.position;
        currentRotation = seat.rotation;

        Vector3 offsetPosition = new Vector3(hipPosition.position.x, pilotPosition.position.y - yOffset, hipPosition.position.z - zOffset);



        if (_isPiloting)
        {
            seat.position = Vector3.Lerp(seat.position, offsetPosition, Time.fixedDeltaTime * sitSpeed);
            seat.rotation = Quaternion.Lerp(currentRotation, pilotPosition.rotation, Time.fixedDeltaTime * sitSpeed);  
        }

        if (!_isPiloting)
        {
            seat.position = Vector3.Lerp(seat.position, startPosition.position, Time.fixedDeltaTime * standSpeed);
            seat.rotation = Quaternion.Lerp(currentRotation, startPosition.rotation, Time.fixedDeltaTime * Mathf.Pow(powAmount, standSpeed));
        }
    }


}
