using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaManager : MonoBehaviour
{
    CameraFollow cameraFollow;
    GameObject player;
    Vector3 playerPosition;
    Bounds currentCameraBounds;
    Bounds cameraBounds;

    void Start()
    {
        player = GameObject.FindWithTag("Player");
        cameraBounds = GetComponent<BoxCollider2D>().bounds;
        cameraFollow = Camera.main.GetComponent<CameraFollow>();

        cameraFollow.SetBoundaries(cameraBounds);
    }

    void Update()
    {
        playerPosition = player.transform.position;

        if( playerPosition.x >= cameraBounds.min.x &&
            playerPosition.x <= cameraBounds.max.x &&
            playerPosition.y >= cameraBounds.min.y &&
            playerPosition.y <= cameraBounds.max.y)
        {
            if(GameManager.instance.playerArea != this.gameObject) {
                GameManager.instance.playerArea = this.gameObject;
                cameraFollow.SetBoundaries(cameraBounds);
            };
        }
    }

    void OnDrawGizmos() {
        //Horizontal lines
        Gizmos.DrawLine(new Vector2(cameraBounds.min.x, cameraBounds.max.y), new Vector2(cameraBounds.max.x, cameraBounds.max.y));
        Gizmos.DrawLine(new Vector2(cameraBounds.min.x, cameraBounds.min.y), new Vector2(cameraBounds.max.x, cameraBounds.min.y));
        //Vertical lines
        Gizmos.DrawLine(new Vector2(cameraBounds.min.x, cameraBounds.min.y), new Vector2(cameraBounds.min.x, cameraBounds.max.y));
        Gizmos.DrawLine(new Vector2(cameraBounds.max.x, cameraBounds.min.y), new Vector2(cameraBounds.max.x, cameraBounds.max.y));
    }
}