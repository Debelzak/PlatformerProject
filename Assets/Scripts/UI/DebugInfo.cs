using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugInfo : MonoBehaviour
{
    private Text debugText;

    void Start() {
        debugText = GetComponent<Text>();
    }

    void LateUpdate()
    {
        debugText.text = GameManager.instance.playerArea.scene.name.ToString() + ": " + GameManager.instance.playerArea.name.ToString() + "\n" +
                        "X: " + GameManager.instance.playerLocation.x.ToString("F1") + "    " +
                        "Y: " + GameManager.instance.playerLocation.y.ToString("F1") + "\n";
    }
}
