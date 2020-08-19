using System;
using System.Collections.Generic;
using UnityEngine;

public enum TILE_MODE { Unselected, Selected, MoveAllowed, AttackAllowed }
public class BoardTile : MonoBehaviour
{
    Color[] stateColour = {
            new Color(0,0,0,0),      //Unselected
           new Color(0,0,1,0.2f),  //Selected
           new Color(0,1,0,0.2f),  //MoveAllowed
           new Color(0.9f,0.2f,0,0.2f) //AttackAllowed
        };
    TILE_MODE state = TILE_MODE.Unselected;
    SpriteRenderer outlineRenderer;
    [SerializeField] SpriteRenderer backgroundRenderer;
    BoardUnitBaseClass heldUnit;
    TileEffect tileEffect;

    Vector2Int boardPosition;
    private void Awake()
    {
        outlineRenderer = GetComponent<SpriteRenderer>();
    }
    internal void OnEndOfTurn(Player currentPlayer)
    {
        if (heldUnit?.OwningPlayer == currentPlayer)
        {
            heldUnit?.AddOverTimeEffect(tileEffect?.GetOverTimeEffect);
            heldUnit?.OnEndofTurn();
        }
    }
    public void SetBoardPosition(Vector2Int pos) => boardPosition = pos;
    public void AddUnit(BoardUnitBaseClass unit)
    {
        heldUnit = unit;
    }
    public void RemoveUnit()
    {
        heldUnit = null;
    }
    public void SetState(TILE_MODE state)
    {
        this.state = state;
        backgroundRenderer.color = stateColour[(int)state];
    }
    public void Attack(AbilityParameters param)
    {
        if (heldUnit != null)
        {
            heldUnit.DealDamage(param);
        }
    }
    public void Attack(int damage)
    {
        if (heldUnit != null)
        {
            heldUnit.DealDamage(new AbilityParameters { damage = 1 });
        }
    }

    public void RemoveTileEffect()
    {
        tileEffect.Destroy();
        tileEffect = null;
    }
    public void AddTileEffect(TileEffect effect)
    {
        TileEffect newEffect = Instantiate(effect, transform.position, Quaternion.identity);
        tileEffect = effect;
        heldUnit?.AddOverTimeEffect(newEffect.GetOverTimeEffect);
    }




    public bool Occupied()
    {
        if (heldUnit == null) return false;


        return true;
    }
    public Vector2Int BoardPosition => boardPosition;
    public Vector2 Position => transform.position;
    public BoardUnitBaseClass GetUnit
    {
        get
        {
            return heldUnit;
        }
    }
    public TILE_MODE State => state;


}
