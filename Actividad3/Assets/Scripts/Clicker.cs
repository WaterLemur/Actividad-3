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
    string Username;
    string url = "https://sid-restapi.onrender.com";

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Clicked()
    {
        score = score + level;
        textScore.text = "SCORE: " + score.ToString();

        AuthResponse credentials = new AuthResponse();
        //credentials.usuario.username = PlayerPrefs.GetString("username");
        //credentials.token = PlayerPrefs.GetString("token");
        credentials.usuario.data.score = score;
        string postData = JsonUtility.ToJson(credentials);
        StartCoroutine("PatchData", postData);
    }
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

    IEnumerator PatchData(string postData)
    {
        Token = PlayerPrefs.GetString("token");
        Username = PlayerPrefs.GetString("username");

        string path = "/api/usuarios";
        UnityWebRequest www = UnityWebRequest.Put(url + path, postData);
        www.method = "PATCH";
        www.SetRequestHeader("Content-Type", "application/json");
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
