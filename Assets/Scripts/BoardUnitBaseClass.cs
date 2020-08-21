using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BoardUnitBaseClass : MonoBehaviour
{
    public abstract Vector2 Position { get; }
    public abstract Player OwningPlayer { get; }
    public abstract BoardTile OccupiedTile { get; }
    public abstract bool Snared { get; }
    public abstract bool IsDead { get; }
    public virtual bool Targetable => true;
    public abstract int CurrentHealth { get; set; }
    public abstract int MaxMovement { get; }
    public abstract List<Ability> Abilites { get; }
    public abstract Sprite Icon { get; }
    public abstract int MaxHealth { get; }
    public abstract bool MayAct { get; }
    public abstract bool MayMove { get; }
    public abstract bool mayUseAbility { get; }
    public abstract bool Pushable { get; }

    public abstract void Move(BoardTile targetTile, bool useMoveAction = false);
    public abstract void DealDamage(AbilityParameters param);
    public virtual void ShowDirectionArrow(BoardTile from, BoardTile to) { }
    public virtual void HideDirectionArrow() { }
    public virtual void UseAbility() { }
    public virtual void SetPlayer(Player player = null) { }
    public virtual void ResetActions() { }
    public virtual void OnStartOfTurn() { }
    public virtual void OnEndofTurn() { }
    public virtual void AddTemporaryMaterialEffect(float time, Material material) { }
    public virtual void AddOverTimeEffect(OverTimeEffect effect) { }
    public virtual void ShowHealth(bool forceUpdate = false) { }
    public virtual void SetSnared(bool value) { }
}
