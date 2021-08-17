using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    //Config
    public enum parallaxCameraType{Real,Simulated}

    public parallaxCameraType cameraType;
    public float simulatedCameraSpeed;

    public bool lockXPosition;
    public bool lockYPosition;

    Transform cam;
    CameraFollow cameraFollow;
    Vector3 camInitialPosition;
    Vector3 initialPosition;
    Vector3 targetPosition;
    Bounds areaBounds;

    float parallaxFactor;

    void OnEnable()
    {
        cam = Camera.main.transform;
        cameraFollow = cam.GetComponent<CameraFollow>();

        camInitialPosition = cam.position;
        initialPosition = transform.position;
    }

    void Update()
    { 
        if(GameManager.instance.playerArea == gameObject.transform.parent.gameObject.transform.parent.gameObject) {
            //HandleParallax
            targetPosition = transform.position;
            parallaxFactor = transform.position.z / 10;

            //Real camera parallax
            if(cameraType == parallaxCameraType.Real) {
                targetPosition.x = initialPosition.x + (cam.position.x - camInitialPosition.x) * parallaxFactor;
                targetPosition.y = initialPosition.y + (cam.position.y - camInitialPosition.y) * parallaxFactor;
            }

            //Simulated camera parallax
            if(cameraType == parallaxCameraType.Simulated) {
                areaBounds = cameraFollow.GetBoundaries();

                if(parallaxFactor < 0) {
                    parallaxFactor = Mathf.Abs(parallaxFactor) + 10;
                }

                if(transform.position.x <= areaBounds.min.x) {
                    transform.position = new Vector3(areaBounds.max.x, transform.position.y, transform.position.z);
                }

                targetPosition.x = transform.position.x - (simulatedCameraSpeed * parallaxFactor * Time.deltaTime);
                targetPosition.y = transform.position.y;
            }


            if(lockXPosition)
                targetPosition.x = initialPosition.x;
            if(lockYPosition)
                targetPosition.y = initialPosition.y;

            transform.position = new Vector3(targetPosition.x, targetPosition.y, transform.position.z);
        } else {
            Reset();
        }
    }

    void Reset() {
        transform.position = initialPosition;
    }
}
