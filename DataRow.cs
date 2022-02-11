using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DataRow : MonoBehaviour
{
    public int stockIndex;
    public TMP_Text shares;
    public TMP_Text avg;
    public TMP_Text price;
    public TMP_Text total;

    double totalF;

    AccountManager accountManager;

    void Start()
    {
        accountManager = GameObject.Find("AccountManager").GetComponent<AccountManager>();
    }

    void Update()
    {
        double shareF = accountManager.getStockShares(stockIndex);
        double avgF = accountManager.getAvgPriceBuy(stockIndex);
        double priceF = accountManager.getCurrentStockPrice(stockIndex);
        totalF = shareF * priceF;

        shares.text = shareF.ToString("f2");
        avg.text = avgF.ToString("f2");
        price.text = priceF.ToString("f2");
        total.text = totalF.ToString("f2");

    }

    public double getTotal() {
        return totalF;
    }
}
