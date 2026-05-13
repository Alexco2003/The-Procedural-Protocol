using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NPCData
{
    public int id;
    public string npcName;
    public NPCClass npcClass;

    public float hp;
    public float maxHp;
    public float damage;
    public float armor;
    public float moveSpeed;

    public bool isDead = false;

    public NPCData() { }
}