using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    public GameObject uiCanvas;

    [HideInInspector]
    public Animator animator;
    
    public Text debugText;

    void Awake()
    {
        if(instance == null) {
            instance = this;
        } else if(instance != this){
            Destroy(this.gameObject);
        }

        uiCanvas.GetComponent<Animator>();
    }

    void LateUpdate()
    {
        debugText.text = GameManager.instance.playerArea.scene.name.ToString() + ": " + GameManager.instance.playerArea.name.ToString() + "\n" +
                        "X: " + GameManager.instance.playerLocation.x.ToString("F1") + "    " +
                        "Y: " + GameManager.instance.playerLocation.y.ToString("F1") + "\n";
    }
}
