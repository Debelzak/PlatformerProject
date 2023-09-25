using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    //Config
    [Header("Parallax speed")]
    [Range(-1.0f,1.0f)]
    public float XModifier;
    [Range(-1.0f,1.0f)]
    public float YModifier;

    [Header("Options")]
    public bool infiniteScroll;
    public bool autoScroll;
    public Vector2 autoScrollSpeed;

    //Private
    private Vector2 parallaxModifier 
    {
        get {return new Vector2(XModifier, YModifier);}
    }
    private bool active;
    private GameObject thisArea;
    private Bounds areaBounds;

        //Camera info
    private Transform cam;
    private Vector3 camStartPos;
    private CameraManager cameraManager;
    private Vector3 camSize => cameraManager.GetCameraSize();

        //Parallax Sprite info and source position
    private Vector3 startPos, nextPos, length;
    private Vector3 moveAmount, targetPosition;

    void Start()
    {
        thisArea = GetComponentInParent<AreaManager>().gameObject;
        
        cam = Camera.main.transform;
        cameraManager = CameraManager.Instance;
        camStartPos = cam.position;

        startPos = transform.position;
        length = GetComponent<SpriteRenderer>().bounds.size;

        transform.position = new Vector3(transform.position.x, transform.position.y, parallaxModifier.x * 10);
    }

    void LateUpdate()
    {
        if(GameManager.instance.playerArea==thisArea) {
            if(!active) {
                SetEnabled(true);
                areaBounds = cameraManager.GetBoundaries();
                nextPos = startPos;
                camStartPos.x = areaBounds.min.x + (camSize.x / 2);
                camStartPos.y = areaBounds.min.y + cameraManager.GetCameraSize().y / 2;
            }
        } else {
            SetEnabled(false);
        }

        if(active)
            HandleParallax();
    }

    void HandleParallax() {
        moveAmount = cam.position - camStartPos;

        if(autoScroll) {
            Vector2 deltaMovement = ( (autoScrollSpeed * parallaxModifier) - autoScrollSpeed ) * Time.deltaTime;
            nextPos += (Vector3)deltaMovement;
        }

        targetPosition = (Vector2)nextPos + (moveAmount * parallaxModifier);
        transform.position = cameraManager.RoundToPixel(new Vector3(targetPosition.x, targetPosition.y, transform.position.z));

        if(infiniteScroll) {
            Vector3 initialDistance = startPos - camStartPos;
            float frontOfSprite = transform.position.x + length.x/2;
            float backOfSprite = transform.position.x - length.x/2;

            float frontOfCamera = cam.position.x + camSize.x/2;
            float backOfCamera = cam.position.x - camSize.x/2;

            if(frontOfSprite < backOfCamera + initialDistance.x) {
                nextPos.x += length.x;
            }
            else if(backOfCamera < backOfSprite - initialDistance.x) {
                nextPos.x -= length.x;
            }
        }
    }

    void SetEnabled(bool i) {
        active = i;
        GetComponent<SpriteRenderer>().enabled = i;
    }
}