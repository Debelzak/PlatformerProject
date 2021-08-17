using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class CameraFollow : MonoBehaviour
{
    PixelPerfectCamera ppCamera;

    public Transform follow;
    Vector3 targetPosition;
    
    Bounds areaBounds = new Bounds();
    float maxXPosition = Mathf.Infinity;
    float maxYPosition = Mathf.Infinity;
    float minXPosition = -Mathf.Infinity;
    float minYPosition = -Mathf.Infinity;

    float pixelsPerUnit;
    float horzSize;
    float vertSize;

    float cameraTransitionTimer;
    
    void Start() {
        ppCamera = GetComponent<PixelPerfectCamera>();
        pixelsPerUnit = ppCamera.assetsPPU;
        horzSize = ppCamera.refResolutionX;
        vertSize = ppCamera.refResolutionY;
    }

    void Update()
    {
        if(cameraTransitionTimer <= 0) {
            Time.timeScale = 1f;
            UIManager.instance.animator.Play("CrossFade_end");
        } else {
            cameraTransitionTimer -= Time.unscaledDeltaTime;
        }

        targetPosition = new Vector3(follow.position.x, follow.position.y, transform.position.z);
        targetPosition.x = Mathf.Round(targetPosition.x * pixelsPerUnit) / pixelsPerUnit;
        targetPosition.y = Mathf.Round(targetPosition.y * pixelsPerUnit) / pixelsPerUnit;

        targetPosition.x = (targetPosition.x <= minXPosition) ? minXPosition : targetPosition.x;
        targetPosition.x = (targetPosition.x >= maxXPosition) ? maxXPosition : targetPosition.x;
        targetPosition.y = (targetPosition.y <= minYPosition) ? minYPosition : targetPosition.y;
        targetPosition.y = (targetPosition.y >= maxYPosition) ? maxYPosition : targetPosition.y;

        transform.position = targetPosition;
    }

    public void SetBoundaries(Bounds cameraBounds) {
        UIManager.instance.animator.Play("CrossFade_start");

        minXPosition = cameraBounds.min.x + (horzSize / pixelsPerUnit) / 2;
        maxXPosition = cameraBounds.max.x - (horzSize / pixelsPerUnit) / 2;
        minYPosition = cameraBounds.min.y + (vertSize / pixelsPerUnit) / 2;
        maxYPosition = cameraBounds.max.y - (vertSize / pixelsPerUnit) / 2;

        areaBounds = cameraBounds;

        cameraTransitionTimer = 0.2f;
        Time.timeScale = 0;
    }

    public Bounds GetBoundaries() {
        return areaBounds;
    }
}
