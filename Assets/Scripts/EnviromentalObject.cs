using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnviromentalObject : BoardUnitBaseClass
{
    Creature creature;
    int currentHealth;
    Sprite icon;
    BoardTile occupiedTile;
    List<Material> materials;

    Renderer renderer;

    public override void UnitConstructor(Creature unitCreature, Player player = null)
    {
        creature = unitCreature;
        currentHealth = unitCreature.MaxHealth;
    }
    public override void SetRenderer(Renderer meshRenderer)
    {
        renderer = meshRenderer;
        materials = renderer.materials.ToList();
    }
    public override void AddTemporaryMaterialEffect(float time, Material material)
    {
        materials.Add(material);
        renderer.materials = materials.ToArray();
        ActionDelayer.RunAfterDelay(time, () =>
        {
            materials.Remove(material);
            renderer.materials = materials.ToArray();
        });
    }

    public override void DealDamage(int damage)
    {
        currentHealth -= damage > 0 ? 1 : 0;
    }

    public override void Move(BoardTile targetTile, bool useMoveAction = false)
    {
        if (occupiedTile == null)
        {
            targetTile.AddUnit(this);
            occupiedTile = targetTile;
            transform.position = targetTile.transform.position;
        }
    }

    public override void AddOverTimeEffect(OverTimeEffect effect) { }
    public override void HideDirectionArrow() { }
    public override void OnEndofTurn() { }
    public override void OnStartOfTurn() { }
    public override void ResetActions() { }
    public override void SetSnared(bool value) { }
    public override void ShowDirectionArrow(BoardTile from, BoardTile to) { }
    public override void ShowHealth(bool forceUpdate = false) { }
    public override void UseAbility() { }



    public override Vector2 Position => occupiedTile.Position;

    public override Player OwningPlayer => null;

    public override BoardTile OccupiedTile => occupiedTile;

    public override bool Snared => false;

    public override bool IsDead => false;

    public override int CurrentHealth
    {
        get => currentHealth;
        set => currentHealth = value;
    }

    public override int MaxMovement => 0;

    public override List<Ability> abilities => throw new System.NotImplementedException();

    public override Sprite Icon => icon;

    public override int maxHealth => creature.MaxHealth;

    public override bool MayAct => false;

    public override bool MayMove => false;

    public override bool mayUseAbility => false;

    public override bool Pushable => false;
}
