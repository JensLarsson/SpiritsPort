using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class EnviromentalObject : BoardUnitBaseClass
{
    [SerializeField] protected Renderer renderer;
    [SerializeField] protected Sprite icon;
    [SerializeField] protected List<UnityEvent> onHitEvents = new List<UnityEvent>();
    protected Queue<UnityEvent> OnHitQueue = new Queue<UnityEvent>();

    protected int currentHealth;
    protected BoardTile occupiedTile;
    protected List<Material> materials;


    private void Awake()
    {
        materials = renderer.materials.ToList();
        OnHitQueue = new Queue<UnityEvent>(onHitEvents);
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
    public void InstantiateObject(GameObject gObject)
    {
        Instantiate(gObject, transform.position, this.transform.rotation);
    }
    public override void DealDamage(AbilityParameters param)
    {
        if (param.damage > 0 && OnHitQueue.Count > 0)
        {
            OnHitQueue.Dequeue().Invoke();
        }
    }

    public void DestroyUnit()
    {
        occupiedTile.RemoveUnit();
        Destroy(this.gameObject);
    }

    public void FreeOccupiedTile() => occupiedTile.RemoveUnit();

    public override void Move(BoardTile targetTile, bool useMoveAction = false)
    {
        if (occupiedTile == null)
        {
            targetTile.AddUnit(this);
            occupiedTile = targetTile;
            transform.position = targetTile.transform.position;
        }
    }

    public void ChangeMaterial(Material material)
    {
        renderer.material = material;
    }

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

    public override List<Ability> Abilites => throw new System.NotImplementedException();

    public override Sprite Icon => icon;

    public override int MaxHealth => 0;

    public override bool MayAct => false;

    public override bool MayMove => false;

    public override bool mayUseAbility => false;

    public override bool Pushable => false;
}
