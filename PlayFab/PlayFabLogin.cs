using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using PlayFab;
using PlayFab.ClientModels;
using TMPro;
using UnityEngine.SceneManagement;

public class PlayFabLogin : MonoBehaviour {

    private static PlayFabLogin playFabLogin;

    private string username;
    private string password;

    public TMP_Text errorText;

    void Start() {
        if (string.IsNullOrEmpty(PlayFabSettings.TitleId))
            PlayFabSettings.TitleId = "8E2BC";
    }

    public void Login() {
        var request = new LoginWithPlayFabRequest { Username = username, Password = password };
        PlayFabClientAPI.LoginWithPlayFab(request, OnLoginSuccess, OnLoginFailed);
    }

    public void CreateAccount() {
        var register = new RegisterPlayFabUserRequest { Username = username, Password = password, RequireBothUsernameAndEmail = false };
        PlayFabClientAPI.RegisterPlayFabUser(register, OnRegisterSuccess, OnRegisterFailed);
    }

    private void OnLoginSuccess(LoginResult result) {
        Debug.Log("Successful login");

        SaveSystem.Save(new LoginData(username, password));
        SceneManager.LoadScene(2);
    }

    private void OnLoginFailed(PlayFabError error) {
        errorText.text = "Username or password is invalid";
    }

    private void OnRegisterSuccess(RegisterPlayFabUserResult result) {
        Debug.Log("Successful register");

        var request0 = new UpdateUserTitleDisplayNameRequest { DisplayName = username };
        PlayFabClientAPI.UpdateUserTitleDisplayName(request0, OnUserTitleSuccess, OnUserTitleFailed);

        var request = new LoginWithPlayFabRequest { Username = username, Password = password };
        PlayFabClientAPI.LoginWithPlayFab(request, OnLoginSuccess, OnLoginFailed);
    }

    private void OnRegisterFailed(PlayFabError error) {
        errorText.text = "Registration error";
    }

    private void OnUserTitleSuccess(UpdateUserTitleDisplayNameResult result) {
        Debug.Log("Successful display name update");
    }

    private void OnUserTitleFailed(PlayFabError error) {
        Debug.Log("Failed to update display name");
    }

    public void SetUsername(string str) {
        username = str;
    }

    public void SetPassword(string str) {
        password = str;
    }

    public string GetUsername() {
        return username;
    }
}
