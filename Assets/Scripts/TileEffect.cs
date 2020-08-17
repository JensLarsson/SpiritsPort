using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileEffect : MonoBehaviour
{
    [SerializeField] OverTimeEffect overTimeEffect;
    [SerializeField] int endOfTurnDamage = 0;
    [SerializeField] Sprite effectSprite;
    [SerializeField] Transform model;

    BoardTile occupiedTile;

    private void Start()
    {
        model.transform.Rotate(new Vector3(0, 0, Random.Range(0, 360)));
    }
    public void OnEndOfTurn(BoardUnit unit)
    {
        unit.DealDamage(new AbilityParameters { damage = endOfTurnDamage });
        unit.AddOverTimeEffect(overTimeEffect);
    }

    public void SetOccupiedTile(BoardTile tile)
    {
        occupiedTile = tile;
    }

    public void Destroy()
    {
        Destroy(this.gameObject);
    }

    public OverTimeEffect GetOverTimeEffect => overTimeEffect;
}
