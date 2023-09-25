using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    //public
    public GameObject playerArea;
    public Vector2 playerLocation;
    public int mapPositionX = 0;
    public int mapPositionY = 0;

    //private
    private GameObject player;

    void Start()
    {
        if(instance == null) {
            instance = this;
        } else if(instance != this){
            Destroy(this.gameObject);
        }
        
        player = GameObject.FindGameObjectWithTag("Player");
        
        Setup();
    }

    void FixedUpdate() {
        playerLocation = player.transform.position;

        mapPositionX = (int)(player.transform.position.x / 20);
        mapPositionY = (int)(player.transform.position.y / 12);
    }

    void Setup()
    {
        Application.targetFrameRate = 60;
    }
}
