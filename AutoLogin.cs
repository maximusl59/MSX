using PlayFab;
using PlayFab.ClientModels;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AutoLogin : MonoBehaviour
{
    string username;
    string password;

    void Start()
    {
        if (string.IsNullOrEmpty(PlayFabSettings.TitleId))
            PlayFabSettings.TitleId = "8E2BC";

        Init();
    }

    void Init() {
        LoginData loginData = SaveSystem.Load();
        if (loginData == null) {
            SceneManager.LoadScene(1);
            return;
        }

        username = loginData.username;
        password = loginData.password;

        Login();
    }

    void Login() {
        var request = new LoginWithPlayFabRequest { Username = username, Password = password };
        PlayFabClientAPI.LoginWithPlayFab(request, OnLoginSuccess, OnLoginFailed);
    }

    private void OnLoginSuccess(LoginResult result) {
        Debug.Log("Successful login");
        SceneManager.LoadScene(2);
    }

    private void OnLoginFailed(PlayFabError error) {
        SceneManager.LoadScene(1);
    }
}
