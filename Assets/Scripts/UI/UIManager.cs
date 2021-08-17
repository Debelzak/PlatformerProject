using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    public Animator animator;

    void Awake()
    {
        if(instance == null) {
            instance = this;
        } else if(instance != this){
            Destroy(this.gameObject);
        }

        DontDestroyOnLoad(this.gameObject);

        animator = GetComponent<Animator>();
    }
}
