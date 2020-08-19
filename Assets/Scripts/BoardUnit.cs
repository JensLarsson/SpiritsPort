using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardUnit : BoardUnitBaseClass
{
    #region Unit Variables
    // // Unit variables
    [SerializeField] int maxHealth;
    [SerializeField] int maxMovement;
    [SerializeField] List<Ability> abilities = new List<Ability>();
    [SerializeField] Sprite icon;
    [SerializeField] Renderer renderer;


    Player owningPlayer;
    BoardTile occupiedTile;
    bool snared;
    bool isDead;
    List<OverTimeEffect> overTimeEffects = new List<OverTimeEffect>();
    public override int CurrentHealth
    {
        get => health;
        set
        {
            health = value;
            if (health <= 0)
            {
                KillUnit();
            }
            //isDead = (health <= 0);
        }
    }
    int health = 3;
    int moves = 1;
    public override void UseAbility()
    {
        abilityUses--;
    }
    int abilityUses = 1;
    #endregion


    // // Editor Connections
    #region Editor Connections

    [SerializeField] HealthBar healthBar;
    [SerializeField] EffectDisplay effectDisplay;
    [SerializeField] SpriteRenderer playerMarker;
    [SerializeField] GameObject directionArrow;
    [SerializeField] bool pushable = true;

    Animator animator;
    List<Material> materials;
    #endregion



    private void Awake()
    {
        animator = GetComponent<Animator>();
        health = maxHealth;
        materials = renderer.materials.ToList();
    }

    public override void SetPlayer(Player player = null)
    {
        if (player != null)
        {
            SetFaction(player);
        }
    }

    public override void Move(BoardTile targetTile, bool useMoveAction = false)
    {
        if (occupiedTile != null && occupiedTile.GetUnit == this)
        {
            occupiedTile.RemoveUnit();
        }
        targetTile.AddUnit(this);
        occupiedTile = targetTile;
        transform.position = targetTile.transform.position;
        if (useMoveAction)
        {
            moves--;
        }
    }
    public override void DealDamage(AbilityParameters param)
    {
        CurrentHealth -= param.damage;
        ShowHealth(true);
        DamageIndicator(param.damage);
        ActionDelayer.RunAfterDelay(1, () =>
        {
            healthBar.HideHealth();
        });
    }
    void DamageIndicator(int damage)
    {
        DamageIndicatorController.Instance.ShowDamage(this.transform, damage);
    }

    void KillUnit()
    {
        OccupiedTile.RemoveUnit();
        Destroy(this.gameObject);
    }


    public override void ResetActions()
    {
        abilityUses = 1;
        moves = 1;
    }
    public override void OnStartOfTurn()
    {
        for (int i = overTimeEffects.Count - 1; i >= 0; i--)
        {
            overTimeEffects[i].OnStartOfturn(this);
        }
    }
    public override void OnEndofTurn()
    {
        for (int i = overTimeEffects.Count - 1; i >= 0; i--)
        {
            overTimeEffects[i].OnEndOfTurn(this);
            if (overTimeEffects[i].TurnsRemaining <= 0)
            {
                overTimeEffects.RemoveAt(i);
            }
        }
        if (IsDead)
        {
            KillUnit();
        }
    }
    public override void AddOverTimeEffect(OverTimeEffect effect)
    {
        if (effect != null)
        {
            for (int i = 0; i < overTimeEffects.Count; i++)
            {
                if (overTimeEffects[i].name == effect.name)
                {
                    overTimeEffects.RemoveAt(i);
                    break;
                }
            }
            overTimeEffects.Add(effect);
        }
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

    public override void ShowHealth(bool forceUpdate = false)
    {
        healthBar.ShowHealth(maxHealth, CurrentHealth, forceUpdate);
    }

    void SetFaction(Player player)
    {
        owningPlayer = player;
        playerMarker.color = player.PlayerColour;
    }

    public override void ShowDirectionArrow(BoardTile from, BoardTile to)
    {
        directionArrow.SetActive(true);
        directionArrow.transform.right = to.transform.position - from.transform.position;
    }
    public override void HideDirectionArrow()
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

    public override void SetSnared(bool value) => snared = value;



    public override bool Snared => snared;
    public override bool IsDead => isDead;
    public override Player OwningPlayer => owningPlayer;
    public override BoardTile OccupiedTile => occupiedTile;
    public override Vector2 Position => OccupiedTile.Position;
    public override List<Ability> Abilites => abilities;
    public override Sprite Icon => icon;
    public override int MaxHealth => maxHealth;
    public override int MaxMovement => maxMovement;
    public override bool MayAct => !IsDead && (MayMove || mayUseAbility);
    public override bool MayMove => (moves > 0) && !Snared;
    public override bool mayUseAbility => abilityUses > 0;
    public override bool Pushable => pushable;

}


