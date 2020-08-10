using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_UnitBar : MonoBehaviour
{
    [SerializeField] BetterButton buttonPrefab;
    [SerializeField] GridLayoutGroup leftList;
    [SerializeField] GridLayoutGroup rightList;


    public static UI_UnitBar Instance;

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


    public void AddUnits(List<BoardUnitBaseClass> teamA, List<BoardUnitBaseClass> teamB)
    {
        foreach (BoardUnitBaseClass unit in teamA)
        {
            BetterButton button = Instantiate(buttonPrefab, leftList.transform);
            button.SetImage(unit.Icon, false);
            button.AddAction(() =>
            {
                GameBoard.Instance.InteractWithTile(unit.OccupiedTile.BoardPosition);
            });
            button.SetText("");
        }
        foreach (BoardUnitBaseClass unit in teamB)
        {
            BetterButton button = Instantiate(buttonPrefab, rightList.transform);
            button.SetImage(unit.Icon, false);
            button.AddAction(() =>
            {
                GameBoard.Instance.InteractWithTile(unit.OccupiedTile.BoardPosition);
            });
            button.SetText("");
        }
    }
}