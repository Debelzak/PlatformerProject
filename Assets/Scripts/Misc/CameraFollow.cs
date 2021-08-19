using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

[RequireComponent(typeof(PixelPerfectCamera))]
public class CameraFollow : MonoBehaviour
{
    public struct CameraLimits {
        public float minX, maxX;
        public float minY, maxY;
    }

    PixelPerfectCamera ppCamera;
    Vector2 cameraSize;

    public Transform follow;
    Vector3 targetPosition;
    
    Bounds areaBounds = new Bounds();

    public CameraLimits cameraLimits;

    float cameraTransitionTimer;
    
    void Start() {
        cameraLimits.minX = -Mathf.Infinity;
        cameraLimits.maxX= Mathf.Infinity;
        cameraLimits.minY = -Mathf.Infinity;
        cameraLimits.maxY = Mathf.Infinity;

        ppCamera = GetComponent<PixelPerfectCamera>();
        cameraSize = GetCameraSize();
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
        targetPosition.x = Mathf.Round(targetPosition.x * ppCamera.assetsPPU) / ppCamera.assetsPPU;
        targetPosition.y = Mathf.Round(targetPosition.y * ppCamera.assetsPPU) / ppCamera.assetsPPU;

        targetPosition.x = (targetPosition.x <= cameraLimits.minX) ? cameraLimits.minX : targetPosition.x;
        targetPosition.x = (targetPosition.x >= cameraLimits.maxX) ? cameraLimits.maxX : targetPosition.x;
        targetPosition.y = (targetPosition.y <= cameraLimits.minY) ? cameraLimits.minY : targetPosition.y;
        targetPosition.y = (targetPosition.y >= cameraLimits.maxY) ? cameraLimits.maxY : targetPosition.y;

        transform.position = targetPosition;
    }

    public void SetBoundaries(Bounds cameraBounds) {
        UIManager.instance.animator.Play("CrossFade_start");

        cameraLimits.minX = cameraBounds.min.x + cameraSize.x / 2;
        cameraLimits.maxX = cameraBounds.max.x - cameraSize.x / 2;
        cameraLimits.minY = cameraBounds.min.y + cameraSize.y / 2;
        cameraLimits.maxY = cameraBounds.max.y - cameraSize.y / 2;

        areaBounds = cameraBounds;

        cameraTransitionTimer = 0.2f;
        Time.timeScale = 0;
    }

    public Bounds GetBoundaries() {
        return areaBounds;
    }

    public Vector2 GetCameraSize() {
        float pixelsPerUnit = ppCamera.assetsPPU;
        float horzSize = ppCamera.refResolutionX;
        float vertSize = ppCamera.refResolutionY;

        return new Vector2(horzSize/pixelsPerUnit, vertSize/pixelsPerUnit);
    }
}
