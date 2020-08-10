using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "NewCreature", menuName = "Creature")]
public class Creature : ScriptableObject
{
    [SerializeField] List<Ability> abilities = new List<Ability>();
    [SerializeField] int maxHealth = 3;
    [SerializeField] int moveDistance = 3;
    [SerializeField] GameObject unitModel;
    [SerializeField] Sprite icon;









    public List<Ability> Abilities => abilities;
    public int MaxHealth => maxHealth;
    public int MoveDistance => moveDistance;
    public GameObject GetModelPrefab => unitModel;
    public Sprite GetIcon => icon;
}
