using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StockHistory : MonoBehaviour
{
    [SerializeField]
    private StockGraphing stockGraphing;
    public string stockName; //replace
    public int stockIndex;

    double currentStockValue;
    double lastStockValue;

    LinkedList<Tuple<float, double>> listOfGraphPoints;

    AccountManager accountManager;
    public MongoDBManager mongoManager;

    void Start()
    {
        lastStockValue = -1;
        listOfGraphPoints = new LinkedList<Tuple<float, double>>();

        accountManager = GameObject.Find("AccountManager").GetComponent<AccountManager>();
    }

    private float nextActionTime = 0.0f;
    private float updatePeriod = 0.75f;

    void Update()
    {
        if (Time.time > nextActionTime) {
            nextActionTime += updatePeriod;
            UpdateHistory();
        }

    }

    void UpdateHistory() {
        //currentStockValue = GameObject.Find(stockName).GetComponent<StockMarketSimulator>().getCurrentStockValue(); //backend REQUEST
        currentStockValue = mongoManager.GetCurrentStock(stockIndex);

        if (currentStockValue == 0)
            accountManager.StockCrash(stockIndex);

        if (lastStockValue != currentStockValue) {
            lastStockValue = currentStockValue;
            listOfGraphPoints.AddLast(new Tuple<float, double>(Time.timeSinceLevelLoad, currentStockValue));
            RemoveGraphPoint();
        }
    }

    private void RemoveGraphPoint() {
        if (listOfGraphPoints.Count > stockGraphing.maxNumOfGO)
            listOfGraphPoints.RemoveFirst();
    }

    public double getCurrentStockValue() {
        return currentStockValue;
    }

    public LinkedList<Tuple<float, double>> getListOfGraphPoints() {
        return listOfGraphPoints;
    }
}
