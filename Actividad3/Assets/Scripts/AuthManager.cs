using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Linq;

public class AuthManager : MonoBehaviour
{
    string url = "https://sid-restapi.onrender.com";

    string Token;
    string Username;


    [SerializeField] TMP_Text[] score = new TMP_Text[5];

    [SerializeField] GameObject game;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Login()
    {
        Credentials credentials = new Credentials();
        credentials.username = GameObject.Find("InputFieldUsername").
            GetComponent<TMP_InputField>().text;

        credentials.password = GameObject.Find("InputFieldPassword").
        GetComponent<TMP_InputField>().text;

        string postData = JsonUtility.ToJson(credentials);

        StartCoroutine(LoginPost(postData));
    }
    public void Register()
    {
        Credentials credentials = new Credentials();
        credentials.username = GameObject.Find("InputFieldUsername").
            GetComponent<TMP_InputField>().text;

        credentials.password = GameObject.Find("InputFieldPassword").
        GetComponent<TMP_InputField>().text;

        string postData = JsonUtility.ToJson(credentials);

        StartCoroutine(RegisterPost(postData));
    }
    public void GetScoreboard()
    {
        StartCoroutine("GetUsers");
    }

    IEnumerator RegisterPost(string postData)
    {
        string path = "/api/usuarios";
        UnityWebRequest www = UnityWebRequest.Put(url + path, postData);
        www.method = "POST";
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
                Debug.Log(www.downloadHandler.text);
                StartCoroutine(LoginPost(postData));
            }

            else
            {
                string mensaje = "status:" + www.responseCode;
                mensaje += "\nError: " + www.downloadHandler.text;
                Debug.Log(mensaje);
            }
        }
    }

    IEnumerator LoginPost(string postData)
    {
        string path = "/api/auth/login";
        UnityWebRequest www = UnityWebRequest.Put(url + path, postData);
        www.method = "POST";
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

                AuthResponse response = JsonUtility.FromJson<AuthResponse>(json);

                game.SetActive(true);
                GameObject.Find("PanelAuth").SetActive(false);

                PlayerPrefs.SetString("token", response.token);
                PlayerPrefs.SetString("username", response.usuario.username);


            }
            else
            {
                string mensaje = "status:" + www.responseCode;
                mensaje += "\nError: " + www.downloadHandler.text;
                Debug.Log(mensaje);
            }
        }
    }

    IEnumerator GetPerfil()
    {
        string path = "/api/usuarios/"+Username;
        UnityWebRequest www = UnityWebRequest.Get(url + path);
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
                AuthResponse response = JsonUtility.FromJson<AuthResponse>(json);
                GameObject.Find("PanelAuth").SetActive(false);
            }
            else
            {
                Debug.Log("Token Vencido... redirecionar a Login");
            }
        }
    }

    IEnumerator GetUsers()
    {
        Token = PlayerPrefs.GetString("token");
        Username = PlayerPrefs.GetString("username");


        string path = "/api/usuarios";
        UnityWebRequest www = UnityWebRequest.Get(url + path);
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
                UsersList response = JsonUtility.FromJson<UsersList>(json);

                UserModel[] leaderboard = response.usuarios
                                            .OrderByDescending(u => u.data.score)
                                            .Take(5).ToArray();

                int index = 0;
                foreach(var user in response.usuarios)
                {
                    Debug.Log(user.username + "|" + user.data.score);
                    score[index].text = user.username + "     " + user.data.score;
                    index++;
                }
            }
            else
            {
                Debug.Log("Token Vencido... redirecionar a Login");
            }
        }
    }
}
[System.Serializable]
public class Credentials
{
    public string username;
    public string password;
}
[System.Serializable]
public class AuthResponse
{
    public UserModel usuario;
    public string token;
}
[System.Serializable]
public class UserModel
{
    public string _id;
    public string username;
    public bool estado;
    public DataUser data;
}
[System.Serializable]
public class UsersList
{
    public UserModel[] usuarios;
}
[System.Serializable]
public class DataUser
{
    public int score;
    public int level;
}