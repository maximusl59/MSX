using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LeaderboardRow : MonoBehaviour
{
    public int rank;
    public string username;
    public double total;

    public TMP_Text rankText;
    public TMP_Text usernameText;
    public TMP_Text totalText;


    public void Init() {
        rankText.text = rank.ToString();
        usernameText.text = username;
        totalText.text = total.ToString("f2");
    }
}
