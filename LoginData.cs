using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LoginData
{
    public string username;
    public string password;

    public LoginData(string user, string pass) {
        username = user;
        password = pass;
    }
}
