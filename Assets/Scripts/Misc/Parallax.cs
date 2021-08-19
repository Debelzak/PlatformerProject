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

    private Vector2 parallaxModifier 
    {
        get {
            return new Vector2(XModifier, YModifier);
        }
    }

    [Header("Options")]
    public bool infiniteScroll;
    public bool autoScroll;
    public Vector2 autoScrollSpeed;

    //Private
    private bool active;

    private GameObject thisArea;
    private Transform cam;
    private CameraFollow cameraFollow;
    private Vector3 camSize => cameraFollow.GetCameraSize();
    private Vector3 camStartPos;
    private Bounds areaBounds;

    private Vector3 originalPos;
    private Vector3 startPos;
    private Vector3 length;

    private Vector2 moveAmount;
    private Vector3 targetPosition;

    void OnEnable() {
        transform.position = new Vector3(transform.position.x, transform.position.y, parallaxModifier.x * 10);
    }

    void Start()
    {
        thisArea = gameObject.transform.parent.gameObject.transform.parent.gameObject;
        
        cam = Camera.main.transform;
        cameraFollow = cam.GetComponent<CameraFollow>();
        camStartPos = cam.position;

        originalPos = transform.position;
        length = GetComponent<SpriteRenderer>().bounds.size;
    }

    void LateUpdate()
    {
        if(GameManager.instance.playerArea==thisArea) {
            if(!active) {
                SetEnabled(true);
                areaBounds = cameraFollow.GetBoundaries();
                startPos = originalPos;
                camStartPos.x = areaBounds.min.x + (camSize.x / 2);
                camStartPos.y = areaBounds.min.y + cameraFollow.GetCameraSize().y / 2;
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
            startPos += (Vector3)deltaMovement;
        }

        targetPosition = (Vector2)startPos + (moveAmount * parallaxModifier);
        transform.position = new Vector3(targetPosition.x, targetPosition.y, transform.position.z);

        if(infiniteScroll) {
            Vector3 persistentDistance = originalPos - camStartPos;
            float frontOfSprite = transform.position.x + length.x/2;
            float backOfSprite = transform.position.x - length.x/2;

            float frontOfCamera = cam.position.x + camSize.x/2;
            float backOfCamera = cam.position.x - camSize.x/2;

            if(frontOfSprite < backOfCamera + 10f + persistentDistance.x) {
                startPos.x += length.x;
            }
            else if(backOfCamera + 10f < backOfSprite - persistentDistance.x) {
                startPos.x -= length.x;
            }
        }
    }

    void SetEnabled(bool i) {
        active = i;
        GetComponent<SpriteRenderer>().enabled = i;
    }
}
