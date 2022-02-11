using System.Collections;
using System.Collections.Generic;
using MongoDB.Bson;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;

public class PlayFabPlayerStats : MonoBehaviour
{
    AccountManager accountManager;

    void Start()
    {
        accountManager = this.gameObject.GetComponent<AccountManager>();
        //GetStatistics();
    }

    public void SetStatistics() {
        PlayFabClientAPI.UpdatePlayerStatistics(new UpdatePlayerStatisticsRequest {
            // request.Statistics is a list, so multiple StatisticUpdate objects can be defined if required.
            Statistics = new List<StatisticUpdate> {
                new StatisticUpdate { StatisticName = "DailyBonus", Value = 0 }
            //new StatisticUpdate { StatisticName = "Balance", Value = (int)(accountManager.getBalance() * 100) },
            //new StatisticUpdate { StatisticName = accountManager.getStockName(index), Value = (int)(accountManager.getStockShares(index) * 100) },
            //new StatisticUpdate { StatisticName = accountManager.getStockName(index) + "AvgPrice", Value = (int)(accountManager.getAvgPriceBuy(index) * 100) },
            }
        },
        result => { Debug.Log("User statistics updated"); },
        error => { Debug.LogError(error.GenerateErrorReport()); });
    }
    
    public void GetStatistics() {
        PlayFabClientAPI.GetPlayerStatistics(
            new GetPlayerStatisticsRequest(),
            OnGetStatistics,
            error => Debug.LogError(error.GenerateErrorReport())
        );
    }

    void OnGetStatistics(GetPlayerStatisticsResult result) {
        Debug.Log("Received statistics");
        if (result.Statistics.Count == 0) {
            GenerateStats();
            return;
        }
        //foreach (var eachStat in result.Statistics) {
            //if (eachStat.StatisticName == "Balance")
            //    accountManager.setBalance(eachStat.Value / 100);
            //else
            //    for (int i = 0; i < accountManager.numOfStocks; i++) {
            //        if (eachStat.StatisticName == accountManager.getStockName(i)) {
            //            accountManager.setStockShares(i, (float)eachStat.Value / 100);
            //        }
            //        else if (eachStat.StatisticName == accountManager.getStockName(i) + "AvgPrice") {
            //            accountManager.setAvgPriceBuy(i, (float)eachStat.Value / 100);
            //        }
            //    }
        //}
        //accountManager.UpdateText();
    }

    void GenerateStats() {
        PlayFabClientAPI.UpdatePlayerStatistics(new UpdatePlayerStatisticsRequest {
            // request.Statistics is a list, so multiple StatisticUpdate objects can be defined if required.
            Statistics = new List<StatisticUpdate> {
            new StatisticUpdate { StatisticName = "DailyBonus", Value = 0 },
            }
        },
        result => { Debug.Log("Daily bonus updated"); },
        error => { Debug.LogError(error.GenerateErrorReport()); });
        accountManager.GiveDailyBonus();
        //InitPlayerData();

        //for(int i = 0; i < accountManager.numOfStocks; i++) {
        //    PlayFabClientAPI.UpdatePlayerStatistics(new UpdatePlayerStatisticsRequest {
        //        // request.Statistics is a list, so multiple StatisticUpdate objects can be defined if required.
        //        Statistics = new List<StatisticUpdate> {
        //        new StatisticUpdate { StatisticName = accountManager.getStockName(i), Value = 0 },
        //        new StatisticUpdate { StatisticName = accountManager.getStockName(i) + "AvgPrice", Value = 0 },
        //        }
        //    },
        //    result => { Debug.Log("User statistics updated"); },
        //    error => { Debug.LogError(error.GenerateErrorReport()); });
        //}
    }

    // Build the request object and access the API
    private void InitPlayerData() {

        PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest() {
            FunctionName = "initPlayerData", // Arbitrary function name (must exist in your uploaded cloud.js file)
            FunctionParameter = new {}, // The parameter provided to your function
            GeneratePlayStreamEvent = true, // Optional - Shows this event in PlayStream
        }, OnSuccess, OnErrorShared);
    }

    private static void OnSuccess(ExecuteCloudScriptResult result) {
        // CloudScript returns arbitrary results, so you have to evaluate them one step and one parameter at a time
        Debug.Log("Successful API Call");
    }

    private static void OnErrorShared(PlayFabError error) {
        Debug.Log(error.GenerateErrorReport());
    }
}
