using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndTurnWindow : MonoBehaviour
{
    [SerializeField] GameObject EndTurnWindowObject;
    public static EndTurnWindow Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
    public void EndTurnPopup()
    {
        EndTurnWindowObject.SetActive(true);
        GameBoard.Instance.ChangeState(new BoardState_Awaiting());
    }

    public void ReturnToGame()
    {
        GameBoard.Instance.ChangeState(new BoardState_UnSelected(GameBoard.Instance.CurrentPlayer));
        EndTurnWindowObject.SetActive(false);
    }
    public void NextTurn()
    {
        GameBoard.Instance.EndTurn();
        EndTurnWindowObject.SetActive(false);
    }

}
