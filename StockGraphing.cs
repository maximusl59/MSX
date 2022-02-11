using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StockGraphing : MonoBehaviour {
    public TMP_Text stockPrice;
    public TMP_Text stockName;

    public TMP_Text yourWallet;
    public TMP_Text currentShares;
    public TMP_Text avgPrice;

    public string[] stockNames;
    public string[] stockHistoryNames;
    StockHistory currentStockHistory;

    public GameObject interactableStockText;
    public GameObject container;
    public GameObject sellButton;

    double lastStockValue;
    double currentStockValue;

    public int maxNumOfGO;

    public AccountManager accountManager;

    void Start() {
        lastStockValue = -1;
        currentStockHistory = GameObject.Find(stockHistoryNames[0]).GetComponent<StockHistory>();
    }

    void Update() {
        if (!container.activeSelf)
            return;

        currentStockValue = currentStockHistory.getCurrentStockValue(); //backend REQUEST
        stockPrice.text = "$" + currentStockValue.ToString("f2");
        if (lastStockValue != currentStockValue) {
            lastStockValue = currentStockValue;

            GetComponentInChildren<WindowGraph>().ShowGraph(currentStockHistory.getListOfGraphPoints());
        }
    }

    public void SetInteractableText(double stockValue, float xPos) {
        TMP_Text stockText = interactableStockText.GetComponent<TMP_Text>();
        stockText.text = "$" + stockValue.ToString("f2");
        RectTransform rectTransform = interactableStockText.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = new Vector2(xPos, rectTransform.anchoredPosition.y);
    }

    public void DisableInteractableText() {
        TMP_Text stockText = interactableStockText.GetComponent<TMP_Text>();
        stockText.text = "";
    }

    public void SetYourWallet(double amount) {
        yourWallet.text = "Your Wallet: $" + amount.ToString("f2");
    }

    public void SetCurrentShares(double amount) {
        currentShares.text = "Current Shares: " + amount.ToString("f2");
    }

    public void SetAvgPrice(double amount) {
        avgPrice.text = "Average Price: " + amount.ToString("f2");
    }

    public void SetCurrentStock(int index) {
        currentStockHistory = GameObject.Find(stockHistoryNames[index]).GetComponent<StockHistory>();
        stockName.text = stockNames[index];
        accountManager.SetStockIndex(index);
    }

    public void AxisRescaleY() {
        GetComponentInChildren<WindowGraph>().AxisRescaleY(currentStockHistory.getListOfGraphPoints());
    }

    public void SetSellButton(bool interactable, string text) {
        sellButton.GetComponent<Button>().interactable = interactable;
        sellButton.GetComponentInChildren<TMP_Text>().text = text;
    }
}
