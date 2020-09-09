using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardTurnInfo : Object
{
    public int i = 4;
    public BoardTurnInfo Clone()
    {
        return (BoardTurnInfo)this.MemberwiseClone();
    }
}
