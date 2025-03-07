using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Clicker : MonoBehaviour
{
    [SerializeField] int score = 0;
    [SerializeField] int level = 1;

    [SerializeField] TMP_Text textScore;
    [SerializeField] TMP_Text textLevel;
    [SerializeField] TMP_Text textUpgrade;

    [SerializeField] GameObject game;
    [SerializeField] GameObject scoreboard;

    [SerializeField] AuthManager authManager;

    string Token;
    string url = "https://sid-restapi.onrender.com";


    public void Upgrade()
    {
        if (score >= level)
        {
            level += 1;
            textLevel.text = "LEVEL: " + level.ToString();
            textUpgrade.text = level.ToString();
        }
    }
    public void OpenScoreboard()
    {
        scoreboard.SetActive(true);
        game.SetActive(false);
        authManager.GetScoreboard();
    }
    public void Clicked()
    {
        score = score + level;
        textScore.text = "SCORE: " + score.ToString();

        UserModel credentials = new UserModel();
        credentials.username = PlayerPrefs.GetString("username");
        credentials.data.score = score;

        string postData = JsonUtility.ToJson(credentials);
        Debug.Log(postData);
        StartCoroutine("PatchData", postData);
    }
    IEnumerator PatchData(string postData)
    {
        Token = PlayerPrefs.GetString("token");

        string path = "/api/usuarios";
        UnityWebRequest www = UnityWebRequest.Put(url + path, postData);
        www.method = "PATCH";
        www.SetRequestHeader("Content-Type", "application/json");
        www.SetRequestHeader("x-token", Token);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log(www.error);
        }
        else
        {
            if (www.responseCode == 200)
            {
                string json = www.downloadHandler.text;

                UserModel response = JsonUtility.FromJson<UserModel>(json);
            }
            else
            {
                string mensaje = "status:" + www.responseCode;
                mensaje += "\nError: " + www.downloadHandler.text;
                Debug.Log(mensaje);
            }
        }
    }
}
