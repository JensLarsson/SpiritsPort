using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Abilities : MonoBehaviour
{
    [SerializeField] BetterButton buttonPrefab;
    (Action action, KeyCode key)[] keyBindings = new (Action action, KeyCode key)[]
    {
        (null, KeyCode.Alpha1),
        (null, KeyCode.Alpha2),
        (null, KeyCode.Alpha3),
        (null, KeyCode.Alpha4),
        (null, KeyCode.Alpha5),
        (null, KeyCode.Alpha6),
        (null, KeyCode.Alpha7),
        (null, KeyCode.Alpha8),
        (null, KeyCode.Alpha9),
    };

    public static UI_Abilities instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public void AddAbilities(List<Ability> abilities, BoardTile tile)
    {
        int i = 0;
        foreach (Ability ability in abilities)
        {
            BetterButton button = Instantiate(buttonPrefab, this.transform);
            button.SetImage(ability.Icon);
            button.AddAction(() =>
            {
                GameBoard.Instance.ChangeState(new BoardState_AbilityPrepared(ability, tile));
            });
            keyBindings[i].action = () => { button.OnPointerDown(null); };
            button.SetText((i + 1).ToString());
            i++;
        }
    }

    public void ClearButtons()
    {
        for (int i = 0; i < keyBindings.Length; i++)
        {
            keyBindings[i].action = null;
        }
        foreach (BetterButton button in GetComponentsInChildren<BetterButton>())
        {
            Destroy(button.gameObject);
        }
    }

    private void Update()
    {
        if (Input.anyKeyDown)
        {
            for (int i = 0; i < keyBindings.Length; i++)
            {
                if (keyBindings[i].action == null)
                {
                    break;
                }
                if (Input.GetKeyDown(keyBindings[i].key))
                {
                    keyBindings[i].action.Invoke();
                }
            }
        }
    }
}
