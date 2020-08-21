
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

class BoardState_ComputerPlaceUnits : BoardState
{
    Player selectedPlayer;
    bool placeOnLeftSide;
    BoardTile previousTile;
    List<BoardUnit> creatures = new List<BoardUnit>();
    BoardState upcommingState;
    Vector2Int lastBoardPos;

    BoardUnitBaseClass selectedUnit;
    public BoardState_ComputerPlaceUnits(Player player, bool leftSide, BoardState nextState)
    {
        selectedPlayer = player;
        placeOnLeftSide = leftSide;
        creatures = player.Creatures.ToList();
        upcommingState = nextState;
    }
    public override void EnterState(GameBoard gameBoard, (int x, int y)[] positions = null)
    {
        int x = placeOnLeftSide ? 0 : (int)gameBoard.boardWidth - (int)gameBoard.boardWidth / 2;
        for (int i = 0; i < gameBoard.boardWidth / 2; i++)
        {
            for (int y = 0; y < gameBoard.boardHeight; y++)
            {
                BoardTile tile = gameBoard.GetBoardTile(x + i, y);
                if (!tile.Occupied())
                {
                    tile.SetState(TILE_MODE.MoveAllowed);
                }
            }
        }
        foreach (BoardUnit unit in creatures)
        {
            int randomX;
            int randomY;
            BoardTile tile;
            do
            {
                randomX = Random.Range(x, x + (int)gameBoard.boardWidth / 2);
                randomY = Random.Range(0, (int)gameBoard.boardHeight);
                tile = gameBoard.GetBoardTile(randomX, randomY);
            } while (tile.Occupied());
            BoardUnit newUnit = gameBoard.CreateBoardUnit(unit, selectedPlayer) as BoardUnit;
            newUnit.Move(tile);
            UI_UnitBar.Instance.AddUnit(newUnit, placeOnLeftSide);
        }
        gameBoard.ChangeState(upcommingState);
    }

    public override void Interact(GameBoard gameBoard, Vector2Int position)
    {

    }
}
