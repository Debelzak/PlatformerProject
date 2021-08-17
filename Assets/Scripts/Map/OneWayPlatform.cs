using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneWayPlatform : MonoBehaviour
{   
    private PlatformEffector2D platformEffector;
    private float surfaceArc;
    private bool hasPlayerCollision;
    
    private float coolDown = .5f;
    private float coolDownTimer;

    void Start()
    {
        platformEffector = GetComponent<PlatformEffector2D>();
        surfaceArc = platformEffector.surfaceArc;
    }

    void Update()
    {
        if(coolDownTimer > 0) {
            coolDownTimer -= Time.deltaTime;
        } else {
            if(hasPlayerCollision && Input.GetAxisRaw("Vertical") <= -0.5 && Input.GetButtonDown("Jump") && platformEffector.surfaceArc == surfaceArc) {
                coolDownTimer = coolDown;
                StartCoroutine(ShufflePlatform());
            }
        }
    }

    IEnumerator ShufflePlatform() {
        platformEffector.surfaceArc = 0;
        yield return new WaitForSeconds(.3f);
        platformEffector.surfaceArc = surfaceArc;
    }

    void OnCollisionEnter2D(Collision2D collision) {
        if(collision.gameObject.tag == "Player") {
            hasPlayerCollision = true;
        }
    }

    void OnCollisionExit2D(Collision2D collision) {
        if(collision.gameObject.tag == "Player") {
            hasPlayerCollision = false;
        }
    }
}
