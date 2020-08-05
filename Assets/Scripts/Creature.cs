using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewCreature", menuName = "Creature")]
public class Creature : ScriptableObject
{
    [SerializeField] List<Ability> abilities = new List<Ability>();
    [SerializeField] int maxHealth = 3;
    [SerializeField] int moveDistance = 3;
    [SerializeField] GameObject unitModel;
    //[SerializeField] Sprite sprite;
    [SerializeField] Sprite icon;
    // [SerializeField] RuntimeAnimatorController animationController;









    public List<Ability> Abilities => abilities;
    public int MaxHealth => maxHealth;
    public int MoveDistance => moveDistance;
    // public Sprite GetSprite => sprite;
    public GameObject GetModelPrefab => unitModel;
    public Sprite GetIcon => icon;
    //public RuntimeAnimatorController AnimationController => animationController;
}
