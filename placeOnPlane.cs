using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//add for xr and ar functions
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
//touch controls
using UnityEngine.InputSystem;
//using System.Security.AccessControl;

[RequireComponent(typeof(ARRaycastManager))]

public class placeOnPlane : MonoBehaviour
{
    //instatiate prefab on the plane at the touch location
    public GameObject placedPrefab;
    //instatiated object
    private GameObject spawnedObject;
    //touch control input
    private TouchControl controls;
    //detect touch input
    bool isPressed;

    ARRaycastManager aRRaycastManager;
    static List<ARRaycastHit> hits = new List<ARRaycastHit>();

    private void Awake()
    {
        aRRaycastManager = GetComponent<ARRaycastManager>();

        controls = new TouchControl();
        //change the is pressed to true (if there is an existing touch)
        controls.control.touch.performed += _ => isPressed = true;
        //change the is pressed to false (if there is no longer a touch)
        controls.control.touch.canceled += _ => isPressed = false;
    }
    //enable touch control
    private void OnEnable()
    {
        controls.control.Enable();
    }
    private void OnDisable()
    {
        controls.control.Disable();
    }

    void Update()
    {
        //check if there is any pointer device connected to the system, or if there is an existing touch input

        if (Pointer.current == null || isPressed == false)
            return;

        //store the current touch position
        var touchPosition = Pointer.current.position.ReadValue();
        //check if the raycast hits any trackables
        if (aRRaycastManager.Raycast(touchPosition, hits, TrackableType.PlaneWithinPolygon))
        {
            //the raycast are sorted by distance, so the first hit means the closest
            var hitPose = hits[0].pose;

            if (spawnedObject == null)
            {
                spawnedObject = Instantiate(placedPrefab, hitPose.position, hitPose.rotation);
            }
            else
            {
                spawnedObject.transform.position = hitPose.position;
                spawnedObject.transform.rotation = hitPose.rotation;
            }

            //spawned object always look at camera
            Vector3 lookPosition = Camera.main.transform.position - spawnedObject.transform.position;
            lookPosition.y = 0;
            spawnedObject.transform.rotation = Quaternion.LookRotation(lookPosition);
        }
    }
}
