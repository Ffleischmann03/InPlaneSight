using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCam : MonoBehaviour
{
    public bool movementEnabled;

    [Header("References")]
    public Transform orientation;
    public Transform player;
    public Transform playerObj;
    public Rigidbody rb;

    public float rotationSpeed;
    public KeyCode lockRotation;
    [SerializeField] private bool rotationLocked = false;

    public Transform combatLookAt;

    public GameObject thirdPersonCam;
    public GameObject combatCam;
    public GameObject topDownCam;
    public GameObject firstPersonCam;

    [Header("Aiming")]
    public KeyCode selectButton;
    private GameObject aimedObject;
    public LayerMask detectableLayers;
    [SerializeField] private PlayableObjectManager PObjM;


    public CameraStyle currentStyle;

    public enum CameraStyle
    {
        Basic,
        Combat,
        Topdown,
        FirstPerson
    }

    public void InhabitObject(GameObject body)
    {
        playerObj = body.gameObject.transform;
        thirdPersonCam.GetComponent<Cinemachine.CinemachineFreeLook>().Follow = playerObj.GetComponent<InhabitableObject>().cameraFollow.transform;
        thirdPersonCam.GetComponent<Cinemachine.CinemachineFreeLook>().LookAt = playerObj.GetComponent<InhabitableObject>().cameraFollow.transform;
        movementEnabled = true;

    }

    public void UninhabitObject()
    {
        if (aimedObject)
            UnhighlightObject(aimedObject);
    }


    private void Update()
    {
        /*if (Input.GetKeyDown(lockRotation)) rotationLocked = true;
        if (Input.GetKeyUp(lockRotation)) rotationLocked = false;*/

        if (Input.GetKeyDown(lockRotation)) rotationLocked = !rotationLocked;


        //Aiming
        if (movementEnabled) PointerObject();
        else aimedObject = null;

        //Shooting
        if (Input.GetKeyDown(selectButton)) SelectObject();

        //rotate orientation
        Vector3 viewDir = player.position - new Vector3(transform.position.x, player.position.y, transform.position.z);
        orientation.forward = viewDir.normalized;

        //rotate player object
        if (currentStyle == CameraStyle.Basic && !rotationLocked)
        {
            float horizontalInput = Input.GetAxis("Horizontal");
            float verticalInput = Input.GetAxis("Vertical");
            Vector3 inputDir = orientation.forward * verticalInput + orientation.right * horizontalInput;

            if (inputDir != Vector3.zero)
                playerObj.forward = Vector3.Slerp(playerObj.forward, inputDir.normalized, Time.deltaTime * rotationSpeed);
        }

    }

    private void SwitchCameraStyle(CameraStyle newStyle)
    {

        thirdPersonCam.SetActive(false);
        firstPersonCam.SetActive(false);


        if (newStyle == CameraStyle.Basic) thirdPersonCam.SetActive(true);
        if (newStyle == CameraStyle.FirstPerson) firstPersonCam.SetActive(true);

        currentStyle = newStyle;

    }


    private void PointerObject()
    {

        RaycastHit rayHit;
        Ray ray = new Ray(transform.position, transform.forward);
        if (Physics.Raycast(ray, out rayHit, 100f, detectableLayers))
        {
            if (rayHit.transform)
            {
                if (rayHit.transform.tag == "Inactive")
                {
                    if (rayHit.transform.gameObject != aimedObject) //Aiming at new object
                    {
                        if(aimedObject)
                            UnhighlightObject(aimedObject);
                        aimedObject = rayHit.transform.gameObject;
                        HighlightObject(aimedObject);
                    }

                }
                else
                {
                    if(aimedObject)
                        UnhighlightObject(aimedObject);
                    aimedObject = null;
                }
            }
            else
            {
                if (aimedObject)
                    UnhighlightObject(aimedObject);
            }

        }
        else
        {
            if (aimedObject)
                UnhighlightObject(aimedObject);
        }

    }

    private void HighlightObject(GameObject obj)
    {
        obj.GetComponent<MeshRenderer>().material.SetInt("_Highlighted", 1);
    }

    private void UnhighlightObject(GameObject obj)
    {
        obj.GetComponent<MeshRenderer>().material.SetInt("_Highlighted", 0);
    }

    private void SelectObject()
    {
        //shoot the object
        if (aimedObject && aimedObject.tag == "Inactive")
        {
            bool hitPlayer = PObjM.QueryObject(aimedObject);
            if(hitPlayer)
            {
                Debug.Log("Success!");
            }
            else
            {
                Debug.Log("Failure.");
            }
        }

    }
}
