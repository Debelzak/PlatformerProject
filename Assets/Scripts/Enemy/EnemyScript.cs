using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EnemyAI))]
public class EnemyScript : MonoBehaviour
{
    public Enemy enemy;
    
    private EnemyAI enemyAI;

    private void Start()
    {
        enemyAI = GetComponent<EnemyAI>();
    }

    private void Update() 
    {
        enemyAI.ApplyAI(this);
    }
}
