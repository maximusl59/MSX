using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TotalRow : MonoBehaviour
{
    public DataRow[] dataRows;

    public TMP_Text balance;
    public TMP_Text total;

    AccountManager accountManager;

    void Start()
    {
        accountManager = GameObject.Find("AccountManager").GetComponent<AccountManager>();
    }

    void Update()
    {
        double balanceF = accountManager.getBalance();
        double totalF = balanceF;
        foreach(DataRow dR in dataRows) {
            totalF += dR.getTotal();
        }

        balance.text = balanceF.ToString("f2");
        total.text = totalF.ToString("f2");
    }
}
