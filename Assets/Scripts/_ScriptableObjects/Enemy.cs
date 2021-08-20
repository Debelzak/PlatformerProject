using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Enemy : ScriptableObject
{
    public int _id;
    public string _name;
    public EnemyAI.Type _ai;
}
