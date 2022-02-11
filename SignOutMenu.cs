using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SignOutMenu : MonoBehaviour
{
    public void SignOutApp() {
        SaveSystem.Delete();
        SceneManager.LoadScene(1);
    }

    public void QuitApp() {
        Application.Quit();
    }
}
