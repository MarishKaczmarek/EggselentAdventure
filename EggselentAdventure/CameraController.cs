using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    //What gameObject are we meant to be following, both Benedict and Yolko have their dedicated cameras to follow them around.
    public GameObject followObject;
    //This references the position of either Egg, which is where we are to end up/move towards per frame.
    private Vector3 endMarker;
    //How fast are we moving about in the world.
    public float cameraSpeed = 5f;

    private void FixedUpdate()
    {
        // We use FixedUpdate instead of Update or LateUpdate because there's an odd "jittery" as the Camera moves about in those two. FixedUpdate provides a smooth camera transition.

        if (followObject != null)
        {
            //We use a custom function that returns our new end position per frame, by giving it the X and Y of the object we follow.
            endMarker = GetNewCameraPosition(followObject.transform.position.x, followObject.transform.position.y);
            //Our new position should "lerp" to the endMarker by the speed we defined.
            transform.position = Vector3.Lerp(transform.position, endMarker, Time.deltaTime * cameraSpeed);
        }

        else
        {
            //We didn't assign anything to follow.
            Debug.Log("We have an unassigned camera to follow object with.");
        }
    }

    private Vector3 GetNewCameraPosition(float x, float y)
    {
        // z must always be -10.
        // If z is anything but -10, then the camera will move to the Z position of the sprite, causing it to jump behind the camera, and we won't see it, or anything
        // for that matter.

        // We take our X, Y and create a new cameraPosition that we then return, giving us a new endMarker.
        Vector3 cameraPosition = new Vector3 (x, y, -10);
        return cameraPosition;
    }
}
