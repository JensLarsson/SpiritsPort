using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_UnitBar : MonoBehaviour
{
    [SerializeField] BetterButton buttonPrefab;
    [SerializeField] GridLayoutGroup leftList;
    [SerializeField] GridLayoutGroup rightList;

    Dictionary<BoardUnit, BetterButton> buttons = new Dictionary<BoardUnit, BetterButton>();

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
    public void AddUnit(BoardUnit unit, bool left)
    {
        BetterButton button = Instantiate(buttonPrefab, left ? leftList.transform : rightList.transform);
        buttons.Add(unit, button);
        button.SetImage(unit.Icon, false);
        button.AddAction(() =>
        {
            GameBoard.Instance.InteractWithTile(unit.OccupiedTile.BoardPosition);
        });
        button.SetText("");
    }
    public void DestroyIcon(BoardUnit unit)
    {
        if (buttons.TryGetValue(unit, out var button))
        {
            Destroy(button.gameObject);
        }
    }
}