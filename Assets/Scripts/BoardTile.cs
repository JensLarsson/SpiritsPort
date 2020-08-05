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
    BoardUnit heldUnit;
    Vector2Int boardPosition;
    private void Awake()
    {
        outlineRenderer = GetComponent<SpriteRenderer>();
    }

    public void SetBoardPosition(Vector2Int pos) => boardPosition = pos;
    public void AddUnit(BoardUnit unit)
    {
        heldUnit = unit;
    }
    public void RemoveUnit(BoardUnit unit)
    {
        heldUnit = null;
    }
    public void SetState(TILE_MODE state)
    {
        this.state = state;
        backgroundRenderer.color = stateColour[(int)state];
    }
    public void Attack(int damage)
    {
        if (heldUnit != null)
        {
            heldUnit.DealDamage(damage);
        }
    }
    public bool Occupied()
    {
        if (heldUnit == null) return false;


        return true;
    }
    public Vector2Int BoardPosition => boardPosition;
    public Vector2 Position => transform.position;
    public BoardUnit GetUnit => heldUnit;
    public TILE_MODE State => state;
}
