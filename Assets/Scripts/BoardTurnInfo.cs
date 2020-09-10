using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardTurnInfo : GameBoard
{


}
public struct TileTurnInfo
{
    int health;
    int speed;
    List<Ability> abilities;
}