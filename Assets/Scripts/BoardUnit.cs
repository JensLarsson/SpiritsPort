using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardUnit : MonoBehaviour
{
    // // Editor Connections
    #region Editor Connections

    [SerializeField] Creature creature;
    [SerializeField] HealthBar healthBar;
    [SerializeField] EffectDisplay effectDisplay;
    [SerializeField] SpriteRenderer playerMarker;
    [SerializeField] GameObject directionArrow;

    Animator animator;
    SpriteRenderer spriteRenderer;
    List<Material> materials;
    #endregion

    #region Unit Variables
    // // Unit variables
    public Player OwningPlayer { get; private set; }
    public BoardTile OccupiedTile { get; private set; }
    public bool Snared { get; private set; }
    public bool isDead { get; private set; }
    List<OverTimeEffect> overTimeEffects = new List<OverTimeEffect>();
    public int CurrentHealth
    {
        get => health;
        private set
        {
            health = value;
            isDead = (health <= 0);
        }
    }

    int health = 3;
    public int MaxMovement { get; private set; }
    int moves = 1;
    public void UseAbility()
    {
        abilityUses--;
    }
    int abilityUses = 1;
    #endregion

    private void Awake()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        materials = spriteRenderer.materials.ToList();
    }

    public void UnitConstructor(Creature unitCreature, BoardTile targetTile)
    {
        creature = unitCreature;
        health = unitCreature.MaxHealth;
        MaxMovement = unitCreature.MoveDistance;
        //animator.runtimeAnimatorController = unitCreature.AnimationController;
        Move(targetTile);
    }

    public void Move(BoardTile targetTile, bool useMoveAction = false)
    {
        if (OccupiedTile != null)
        {
            OccupiedTile.RemoveUnit(this);
        }
        targetTile.AddUnit(this);
        OccupiedTile = targetTile;
        transform.position = targetTile.transform.position;
        if (useMoveAction)
        {
            moves--;
        }
    }
    public virtual void DealDamage(int damage)
    {
        CurrentHealth -= damage;
        ShowHealth(true);
        DamageIndicator(damage);
        ActionDelayer.RunAfterDelay(1, () =>
        {
            healthBar.HideHealth();
        });
    }
    protected void DamageIndicator(int damage)
    {
        DamageIndicatorController.Instance.ShowDamage(this.transform, damage);
    }

    public virtual void KillUnit()
    {
        OccupiedTile.RemoveUnit(this);
        Destroy(this.gameObject);
    }


    public void ResetActions()
    {
        abilityUses = 1;
        moves = 1;
    }
    public void OnStartOfTurn()
    {
        for (int i = overTimeEffects.Count - 1; i >= 0; i--)
        {
            overTimeEffects[i].OnStartOfturn(this);
        }
    }
    public void OnEndofTurn()
    {
        for (int i = overTimeEffects.Count - 1; i >= 0; i--)
        {
            overTimeEffects[i].OnEndOfTurn(this);
            if (overTimeEffects[i].TurnsRemaining <= 0)
            {
                overTimeEffects.RemoveAt(i);
            }
        }
        if (isDead)
        {
            KillUnit();
        }
    }
    public void AddTemporaryMaterialEffect(float time, Material material)
    {
        materials.Add(material);
        spriteRenderer.materials = materials.ToArray();
        ActionDelayer.RunAfterDelay(time, () =>
         {
             materials.Remove(material);
             spriteRenderer.materials = materials.ToArray();
         });
    }

    public void AddOverTimeEffect(OverTimeEffect effect)
    {
        if (overTimeEffects.Contains(effect)) overTimeEffects.Remove(effect);
        overTimeEffects.Add(effect);
    }

    public void ShowHealth(bool forceUpdate = false)
    {
        healthBar.ShowHealth(maxHealth, CurrentHealth, forceUpdate);
    }

    public void SetFaction(Player player)
    {
        this.OwningPlayer = player;
        playerMarker.color = player.PlayerColour;
    }

    public void ShowDirectionArrow(BoardTile from, BoardTile to)
    {
        directionArrow.SetActive(true);
        directionArrow.transform.right = to.transform.position - from.transform.position;
    }
    public void HideDirectionArrow()
    {
        directionArrow.SetActive(false);
    }


    #region OnMouseHandlers
    private void OnMouseEnter()
    {
        ShowHealth();
        effectDisplay.ShowEffectIcons(overTimeEffects);
    }
    private void OnMouseExit()
    {
        healthBar.HideHealth();
        effectDisplay.HideEffectIcons();
    }
    #endregion


    public void SetSnared(bool value) => Snared = value;
    public Vector2 Position => OccupiedTile.Position;
    public List<Ability> abilities => creature.Abilities;
    public Sprite Icon => creature.GetIcon;
    public int maxHealth => creature.MaxHealth;
    public bool MayAct => !isDead && (MayMove || mayUseAbility);
    public bool MayMove => (moves > 0) && !Snared;
    public bool mayUseAbility => abilityUses > 0;

}


