using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Tree : EnviromentalObject
{
    [SerializeField] float animationLength = 1;
    [SerializeField] GameObject treeAxis;
    bool standing = true;
    public override void DealDamage(AbilityParameters param)
    {
        if (param.damage > 0 && standing)
        {
            standing = false;
            Vector2Int fallDirection = occupiedTile.BoardPosition - param.casterTile.BoardPosition;
            fallDirection.Clamp(new Vector2Int(-1, -1), new Vector2Int(1, 1));
            StartCoroutine(FallAnimation(fallDirection));
        }
    }

    IEnumerator FallAnimation(Vector2Int direction)
    {
        GameBoard.Instance.ChangeState(new BoardState_Awaiting());
        Vector3 startPos = treeAxis.transform.localPosition;
        Vector3 startRot = Vector3.zero;

        var endVar = Direction(direction);

        float time = 0;
        while (time < animationLength)
        {
            treeAxis.transform.localPosition =
                Vector3.Lerp(startPos, endVar.position, time / animationLength);
            treeAxis.transform.localEulerAngles =
                Vector3.Lerp(startRot, endVar.rotation, time / animationLength);

            time += Time.deltaTime;
            yield return null;
        }
        BoardTile tile = GameBoard.Instance.GetBoardTile(occupiedTile.BoardPosition + direction);
        GameBoard.Instance.PushUnit(occupiedTile, tile, true);
        tile.AddUnit(this);
        GameBoard.Instance.ChangeState(
            new BoardState_UnSelected(GameBoard.Instance.CurrentPlayer));

    }


    (Vector3 position, Vector3 rotation) Direction(Vector2 direction)
    {
        if (direction == new Vector2Int(0, -1))
            return (new Vector3(0, -0.24f, 0), new Vector3(0, 0, -90));
        else if (direction == new Vector2Int(0, 1))
            return (new Vector3(0, -0.24f, 0), new Vector3(0, 0, 90));
        else if (direction == new Vector2Int(-1, 0))
            return (new Vector3(0, -0.24f, 0), new Vector3(-90, 0, 0));
        else
            return (new Vector3(0, -0.24f, 0), new Vector3(90, 0, 0));

        ////Rotation Matrix
        //float sin = Mathf.Sin(rotation);
        //float cos = Mathf.Cos(rotation);
        //float2x2 m = float2x2(cos, -sin, sin, cos);
        //v.vertex.xz = mul(m, v.vertex.xz);
    }
}
