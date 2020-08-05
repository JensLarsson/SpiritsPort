using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_PlayerIcons : MonoBehaviour
{
    [SerializeField] Image IconA;
    Player playerA;
    [SerializeField] Image IconB;
    Player playerB;
    (Player player, Image icon)[] players;


    public static UI_PlayerIcons Instance;
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


    public void SetPlayers(Player playerA, Player playerB)
    {
        players = new (Player, Image)[]
        {
            (playerA, IconA),
            (playerB, IconB)
        };
        IconA.color = playerA.PlayerColour;
        IconB.color = playerB.PlayerColour;
    }

    public void SetActivePlayer(Player player)
    {
        for (int i = 0; i < players.Length; i++)
        {
            if (players[i].player == player)
            {
                players[i].icon.color = player.PlayerColour;
                players[((i + 1) % 2)].icon.color = Color.gray;
                return;
            }
        }
    }


}
