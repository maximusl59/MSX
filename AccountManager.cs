using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AccountManager : MonoBehaviour
{
    private static string[] stockNames = { "AmogusStock", "GameStopStock", "NvidiaStock" , "DaBabyStock", "BezosStock"};

    public int numOfStocks;
    public string[] stockHistoryNames;
    StockHistory[] stockHistories;
    double[] stockSharesByIndex;
    double[] avgPriceBuyByIndex;
    int[] sellButtonTimerByIndex;

    int currentStockIndex;

    double balance = 100;

    public StockGraphing stockGraphing;

    PlayFabPlayerStats playerStats;
    MongoDBAccount mongoDBAccount;

    void Start()
    {
        currentStockIndex = 0;
        numOfStocks = stockHistoryNames.Length;

        stockHistories = new StockHistory[stockHistoryNames.Length];
        stockSharesByIndex = new double[stockHistoryNames.Length];
        avgPriceBuyByIndex = new double[stockHistoryNames.Length];
        sellButtonTimerByIndex = new int[stockHistoryNames.Length];

        playerStats = GetComponent<PlayFabPlayerStats>();
        mongoDBAccount = GetComponent<MongoDBAccount>();

        for (int i = 0; i < stockHistoryNames.Length; i++) {
            stockHistories[i] = GameObject.Find(stockHistoryNames[i]).GetComponent<StockHistory>();
        }

        UpdateText();
    }

    public void CheckForDailyBonus() {
        playerStats.GetStatistics();
    }

    public void GiveDailyBonus() {
        balance += 100;
        UpdateText();
        UpdateBackend();
    }

    void CheckSellTimerState() {
        if (sellButtonTimerByIndex[currentStockIndex] > 0)
            stockGraphing.SetSellButton(false, sellButtonTimerByIndex[currentStockIndex].ToString());
        else
            stockGraphing.SetSellButton(true, "Sell");
    }

    void StartSellTimer(int index) {
        if(stockSharesByIndex[index] == 0)
            StartCoroutine(SellCountdown(index));
    }

    IEnumerator SellCountdown(int index) {
        sellButtonTimerByIndex[index] = 20;

        while (sellButtonTimerByIndex[index] > 0) {
            if(currentStockIndex == index)
                stockGraphing.SetSellButton(false, sellButtonTimerByIndex[index].ToString());
            yield return new WaitForSeconds(1);
            sellButtonTimerByIndex[index]--;
        }
        if (currentStockIndex == index)
            stockGraphing.SetSellButton(true, "Sell");
    }

    public void UpdateText() {
        stockGraphing.SetYourWallet(balance);
        stockGraphing.SetCurrentShares(stockSharesByIndex[currentStockIndex]);
        stockGraphing.SetAvgPrice(avgPriceBuyByIndex[currentStockIndex]);
    }

    void UpdateBackend() {
        //playerStats.SetStatistics(currentStockIndex);
        mongoDBAccount.UpdatePlayerData();
    }

    public void SetStockIndex(int index) {
        currentStockIndex = index;
        UpdateText();
        CheckSellTimerState();
    }

    public void BuyMax() {
        double currentSharePrice = stockHistories[currentStockIndex].getCurrentStockValue();
        if (currentSharePrice == 0 || balance == 0)
            return;

        StartSellTimer(currentStockIndex);
        double shares = balance / currentSharePrice;
        balance = 0;
        avgPriceBuyByIndex[currentStockIndex] = CalculateAvgPrice(currentSharePrice, shares);
        stockSharesByIndex[currentStockIndex] += shares;
        UpdateText();
        UpdateBackend();
    }

    public void BuyAmount() {
        double currentSharePrice = stockHistories[currentStockIndex].getCurrentStockValue();
        if (currentSharePrice == 0 || balance == 0) 
            return;

        float amount1 = 0;
        float amount2 = 0;

        float.TryParse(GameObject.Find("AmountInput").GetComponent<TMP_InputField>().text, out amount1);
        float.TryParse(GameObject.Find("DollarInput").GetComponent<TMP_InputField>().text, out amount2);

        if(isValidTransaction(true, true, amount1, currentSharePrice)) {

            StartSellTimer(currentStockIndex);
            double shares = amount1;
            balance -= shares * currentSharePrice;
            avgPriceBuyByIndex[currentStockIndex] = CalculateAvgPrice(currentSharePrice, shares);
            stockSharesByIndex[currentStockIndex] += shares;
        } else if(isValidTransaction(true, false, amount2, currentSharePrice)) {

            StartSellTimer(currentStockIndex);
            double shares = amount2 / currentSharePrice;
            balance -= shares * currentSharePrice;
            avgPriceBuyByIndex[currentStockIndex] = CalculateAvgPrice(currentSharePrice, shares);
            stockSharesByIndex[currentStockIndex] += shares;
        }
        UpdateText();
        UpdateBackend();
    }

    public void SellMax() {
        if (stockSharesByIndex[currentStockIndex] == 0)
            return;

        double balanceTemp = stockSharesByIndex[currentStockIndex] * stockHistories[currentStockIndex].getCurrentStockValue();
        stockSharesByIndex[currentStockIndex] = 0;
        balance += balanceTemp;
        avgPriceBuyByIndex[currentStockIndex] = 0;
        UpdateText();
        UpdateBackend();
    }

    public void SellAmount() {
        double currentSharePrice = stockHistories[currentStockIndex].getCurrentStockValue();

        float amount1 = 0;
        float amount2 = 0;

        float.TryParse(GameObject.Find("AmountInput").GetComponent<TMP_InputField>().text, out amount1);
        float.TryParse(GameObject.Find("DollarInput").GetComponent<TMP_InputField>().text, out amount2);

        if (isValidTransaction(false, true, amount1, currentSharePrice)) {
            double balanceTemp = amount1 * stockHistories[currentStockIndex].getCurrentStockValue();
            stockSharesByIndex[currentStockIndex] -= amount1;
            balance += balanceTemp;
            if (stockSharesByIndex[currentStockIndex] == 0)
                avgPriceBuyByIndex[currentStockIndex] = 0;
        } else if(isValidTransaction(false, false, amount2, currentSharePrice)) {
            double stockTemp = (amount2/currentSharePrice);
            stockSharesByIndex[currentStockIndex] -= stockTemp;
            balance += amount2;
            if (stockSharesByIndex[currentStockIndex] == 0)
                avgPriceBuyByIndex[currentStockIndex] = 0;
        }


        UpdateText();
        UpdateBackend();
    }

    private bool isValidTransaction(bool isBuying, bool isAmount, double value, double currentStockValue) {
        if(isBuying) {
            if(isAmount) {
                if (value <= 0 || value * currentStockValue > balance)
                    return false;
            } else {
                if (value <= 0 || value > balance)
                    return false;
            }
        } else {
            if (isAmount) {
                if (value <= 0 || value > stockSharesByIndex[currentStockIndex])
                    return false;
            } else {
                if (value <= 0 || value > currentStockValue * stockSharesByIndex[currentStockIndex])
                    return false;
            }
            
        }
        return true;
    }

    private double CalculateAvgPrice(double amount, double newShares) {
        double result = 0;

        if (avgPriceBuyByIndex[currentStockIndex] == 0)
            result = amount;
        else
            result = ((avgPriceBuyByIndex[currentStockIndex] * stockSharesByIndex[currentStockIndex]) 
                + (amount * newShares)) / (stockSharesByIndex[currentStockIndex] + newShares);

        return result;
    }

    public void StockCrash(int index) {
        stockSharesByIndex[index] = 0;
        avgPriceBuyByIndex[index] = 0;
        UpdateText();
        UpdateBackend();
    }

    public static float Round(float value, int digits) {
        float mult = Mathf.Pow(10.0f, (float)digits);
        return Mathf.Round(value * mult) / mult;
    }

    public double getBalance() {
        return balance;
    }

    public double getStockShares(int index) {
        return stockSharesByIndex[index];
    }

    public double getAvgPriceBuy(int index) {
        return avgPriceBuyByIndex[index];
    }

    public string getStockName(int index) {
        return stockNames[index];
    }

    public double getCurrentStockPrice(int index) {
        return stockHistories[index].getCurrentStockValue();
    }

    public void setBalance(double temp) {
        balance = temp;
    }

    public void setStockShares(int stockSharesIndex, double amount) {
        stockSharesByIndex[stockSharesIndex] = amount;
    }

    public void setAvgPriceBuy(int avgPriceBuyIndex, double amount) {
        avgPriceBuyByIndex[avgPriceBuyIndex] = amount;
    }
}
