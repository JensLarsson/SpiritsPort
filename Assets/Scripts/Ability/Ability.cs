using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Ability", menuName = "Ability/Ability")]
public class Ability : ScriptableObject
{
    enum TARGETING_TYPE { Linear }
    [SerializeField] Sprite icon;
    [SerializeField] TARGETING_TYPE targeting;
    [SerializeField] bool movesThroughOccupied = false;
    [SerializeField] bool pushesTarget = false;
    [SerializeField] int minRange = 1;
    [SerializeField] int maxRange = 20;
    [SerializeField] int damage = 1;
    [SerializeField] OverTimeEffect overTimeEffect;
    [SerializeField] TileEffect tileEffect;
    [SerializeField] Projectile projectilePrefab;
    [SerializeField] GameObject onHitParticleEffect;
    [SerializeField] OnHitTargetEffect onHitTargetEffect;



    public void GetTargetableTiles(GameBoard board, BoardTile tile)
    {
        switch (targeting)
        {
            case TARGETING_TYPE.Linear:
                LinearTargeting(board, tile.BoardPosition, movesThroughOccupied);
                break;
        }
    }

    /// <summary>
    /// Constructs the ability based on exposed variables and invokes it
    /// </summary>
    /// <param name="casterTile"></param>
    /// <param name="targetTile"></param>
    /// <param name="onHit"></param>
    public void Invoke(BoardTile casterTile, BoardTile targetTile, Action onHit)
    {
        casterTile?.GetUnit?.UseAbility();
        onHit = () =>
        {
            targetTile?.Attack(
                new AbilityParameters
                {
                    damage = damage,
                    ability = this,
                    casterTile = casterTile,
                    caster = casterTile.GetUnit as BoardUnit
                });
            if (onHitParticleEffect != null)
            {
                ParticleEffectFactory.StartEffectOnScene(onHitParticleEffect, targetTile.Position);
            }
            if (overTimeEffect != null) //DoTs, Hots, Snares etc
            {
                targetTile?.GetUnit?.AddOverTimeEffect(Instantiate(overTimeEffect));
            }
            if (pushesTarget)           //Pushing
            {
                GameBoard.Instance.PushUnit(casterTile, targetTile);
            }
            if (onHitTargetEffect.material != null) //Shader shit
            {
                targetTile?.GetUnit?.AddTemporaryMaterialEffect(onHitTargetEffect.time, onHitTargetEffect.material);
            }
            if (tileEffect != null)
            {
                targetTile.AddTileEffect(tileEffect);
            }

        }
        + onHit; //Places the original events of onHit in the end of the process
        if (projectilePrefab != null && !movesThroughOccupied) //Move onHit to when projectile reaches target
        {
            Instantiate(projectilePrefab).LinearTravel(casterTile.Position, targetTile.Position, onHit);
        }
        else if (projectilePrefab != null && movesThroughOccupied)
        {
            Instantiate(projectilePrefab).ArchTravel(casterTile.Position, targetTile.Position, onHit);
        }
        else        //Instantly activate onHit
        {
            onHit.Invoke();
        }
    }

    public List<BoardTile> GetLinearTiles(GameBoard board, Vector2Int startPos, bool movesThrough = false)
    {
        List<BoardTile> tiles = new List<BoardTile>();
        for (int x = startPos.x - minRange; x >= 0; x--)
        {
            BoardTile tile = board.GetBoardTile(x, startPos.y);
            tiles.Add(tile);
            if (!movesThrough && tile.Occupied()
                || x <= startPos.x - maxRange)
            {
                break;
            }
        }
        for (int x = startPos.x + minRange; x < board.boardWidth; x++)
        {
            BoardTile tile = board.GetBoardTile(x, startPos.y);
            tiles.Add(tile);
            if (!movesThrough && tile.Occupied()
                || x >= startPos.x + maxRange)
            {
                break;
            }
        }
        for (int y = startPos.y - minRange; y >= 0; y--)
        {
            BoardTile tile = board.GetBoardTile(startPos.x, y);
            tiles.Add(tile);
            if (!movesThrough && tile.Occupied()
                || y <= startPos.y - maxRange)
            {
                break;
            }
        }
        for (int y = startPos.y + minRange; y < board.boardHeight; y++)
        {
            BoardTile tile = board.GetBoardTile(startPos.x, y);
            tiles.Add(tile);
            if (!movesThrough && tile.Occupied()
                || y >= startPos.y + maxRange)
            {
                break;
            }
        }
        return tiles;
    }

    public void LinearTargeting(GameBoard board, Vector2Int startPos, bool movesThrough = false)
    {
        List<BoardTile> tiles = GetLinearTiles(board, startPos, movesThrough);
        foreach (BoardTile tile in tiles)
        {
            tile.SetState(TILE_MODE.AttackAllowed);
        }
    }


    public Sprite Icon => icon;
    public bool PushesTarget => pushesTarget;
    public bool movesThroughUnits => movesThroughOccupied;

    [Serializable]
    class OnHitTargetEffect
    {
        public Material material;
        public float time = 1;
    }
}

public struct AbilityParameters
{
    public int damage;
    public Ability ability;
    public BoardTile casterTile;
    public BoardUnit caster;
    public Vector2Int direction;
}
