using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Logout : MonoBehaviour
{
    [SerializeField] GameObject panelAuth;
    [SerializeField] GameObject game;
    [SerializeField] GameObject scoreboard;

    public void Clicked()
    {
        panelAuth.SetActive(true);
        game.SetActive(false);
        scoreboard.SetActive(false);

        PlayerPrefs.SetString("token", null);
        PlayerPrefs.SetString("username", null);
    }
}
