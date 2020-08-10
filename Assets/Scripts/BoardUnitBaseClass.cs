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
    public abstract int CurrentHealth { get; set; }
    public abstract int MaxMovement { get; }
    public abstract List<Ability> abilities { get; }
    public abstract Sprite Icon { get; }
    public abstract int maxHealth { get; }
    public abstract bool MayAct { get; }
    public abstract bool MayMove { get; }
    public abstract bool mayUseAbility { get; }
    public abstract bool Pushable { get; }

    public abstract void Move(BoardTile targetTile, bool useMoveAction = false);
    public abstract void ShowDirectionArrow(BoardTile from, BoardTile to);
    public abstract void HideDirectionArrow();
    public abstract void UseAbility();
    public abstract void UnitConstructor(Creature unitCreature, Player player = null);
    public abstract void DealDamage(int damage);
    public abstract void ResetActions();
    public abstract void OnStartOfTurn();
    public abstract void OnEndofTurn();
    public abstract void SetRenderer(Renderer meshRenderer);
    public abstract void AddTemporaryMaterialEffect(float time, Material material);
    public abstract void AddOverTimeEffect(OverTimeEffect effect);
    public abstract void ShowHealth(bool forceUpdate = false);
    public abstract void SetSnared(bool value);
}
