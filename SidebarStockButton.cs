using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SidebarStockButton : MonoBehaviour
{
    public TMP_Text stockPrice;
    public string stockName;

    StockHistory stockHistory;

    void Start() {
        stockHistory = GameObject.Find(stockName).GetComponent<StockHistory>();
    }

    void Update()
    {
        double currentStockValue = stockHistory.getCurrentStockValue(); 

        if(currentStockValue >= 100000)
            stockPrice.text = "$" + (currentStockValue/1000000).ToString("f1") + "M";
        else
            stockPrice.text = "$" + currentStockValue.ToString("f2");
    }
}
