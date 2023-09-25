using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MinimapManager : MonoBehaviour
{
    public RectTransform mapRectTransform;

    private Vector3 mapInitialPosition;
    private Vector3 mapTargetPosition;

    void Start() {
        mapInitialPosition = mapRectTransform.anchoredPosition;
    }

    void Update()
    {
        mapTargetPosition = mapInitialPosition;

        mapTargetPosition.x -= GameManager.instance.mapPositionX * 10;
        mapTargetPosition.y -= GameManager.instance.mapPositionY * 6;

        mapRectTransform.anchoredPosition = mapTargetPosition;
    }
}
