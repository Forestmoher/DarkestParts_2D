using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/ActorStats")]
public class ActorStats : ScriptableObject
{
    public string actorName;
    public int maxHealth;
    public int strength;
    public int speed;
}
