using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public enum Type
    {
        AI_NONE,
        AI_PATROL,
        AI_SKELETON,
    }

    public void ApplyAI(EnemyScript instance)
    {
        if(instance.enemy._ai == Type.AI_PATROL) {
            Patrol(instance);
            return;
        }
        else if(instance.enemy._ai == Type.AI_SKELETON) {
            return;
        }
        else {
            return;
        }
    }

    private void Patrol(EnemyScript instance) {
        Debug.Log("Patrulhando...");
        Debug.Log("Meu nome é" + instance.gameObject.name);
        return;
    }
}
