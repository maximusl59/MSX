using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MongoDB.Driver;
using MongoDB.Bson;
using PlayFab;
using PlayFab.ClientModels;
using System;

public class MongoDBAccount : MonoBehaviour
{
    AccountManager accountManager;

    MongoClient mongoClient;
    IMongoDatabase mongoDatabase;
    IMongoCollection<BsonDocument> collectionPlayerData;

    private string username;

    void Start() {
        mongoClient = new MongoClient("mongodb+srv://Everyone:everyone@msx.yauaa.mongodb.net/myFirstDatabase?retryWrites=true&w=majority");
        mongoDatabase = mongoClient.GetDatabase("MSX");
        collectionPlayerData = mongoDatabase.GetCollection<BsonDocument>("playerData");

        accountManager = GetComponent<AccountManager>();

        var request = new GetAccountInfoRequest();
        PlayFabClientAPI.GetAccountInfo(request, OnAccountInfoSuccess, OnAccountInfoFailed);
    }

    private void OnAccountInfoSuccess(GetAccountInfoResult result) {
        username = result.AccountInfo.TitleInfo.DisplayName;
        GetPlayerData();
    }

    private void OnAccountInfoFailed(PlayFabError error) {
        Debug.Log(error);
    }

    public void GetPlayerData() {
        var filter = Builders<BsonDocument>.Filter.Eq("Name", username);
        try {
            var document = collectionPlayerData.Find(filter).FirstOrDefault();

            for (int i = 0; i < document.ElementCount; i++) {
                string temp = document.GetElement(i).Name;
                string value = document.GetElement(i).Value.ToString();
                if (temp == "Balance")
                    accountManager.setBalance(double.Parse(document.GetElement(i).Value.ToString()));
                else
                    for (int j = 0; j < accountManager.numOfStocks; j++) {
                        if (temp== accountManager.getStockName(j)) {
                            accountManager.setStockShares(j, double.Parse(value));
                            break;
                        }
                        else if (temp == accountManager.getStockName(j) + "Avg") {
                            accountManager.setAvgPriceBuy(j, double.Parse(value));
                            break;
                        } 
                    }
            }
        }
        catch (Exception e) {
            InitPlayerData();
        }

        accountManager.CheckForDailyBonus();
        accountManager.UpdateText();
    }

    public void InitPlayerData() {
        BsonDocument document = new BsonDocument() {
            { "Name", username } ,
            { "Balance" , 0.00 } ,
            { "AmogusStock" , 0.00 } ,
            { "AmogusStockAvg" , 0.00 } ,
            { "GameStopStock" , 0.00 } ,
            { "GameStopStockAvg" , 0.00 } ,
            { "NvidiaStock" , 0.00 } ,
            { "NvidiaStockAvg" , 0.00 } ,
            { "DaBabyStock" , 0.00 } ,
            { "DaBabyStockAvg" , 0.00 } ,
            { "BezosStock" , 0.00 } ,
            { "BezosStockAvg" , 0.00 } ,
        };
        collectionPlayerData.InsertOne(document);
    }

    public void UpdatePlayerData() {
        BsonDocument document = new BsonDocument() {
            { "Name", username } ,
            { "Balance" , accountManager.getBalance() } ,
            { "AmogusStock" , accountManager.getStockShares(0) } ,
            { "AmogusStockAvg" , accountManager.getAvgPriceBuy(0) } ,
            { "GameStopStock" , accountManager.getStockShares(1) } ,
            { "GameStopStockAvg" , accountManager.getAvgPriceBuy(1) } ,
            { "NvidiaStock" , accountManager.getStockShares(2) } ,
            { "NvidiaStockAvg" , accountManager.getAvgPriceBuy(2) } ,
            { "DaBabyStock" , accountManager.getStockShares(3) } ,
            { "DaBabyStockAvg" , accountManager.getAvgPriceBuy(3) } ,
            { "BezosStock" , accountManager.getStockShares(4) } ,
            { "BezosStockAvg" , accountManager.getAvgPriceBuy(4) } ,
        };
        var filter = Builders<BsonDocument>.Filter.Eq("Name", username);
        collectionPlayerData.ReplaceOne(filter, document);
    }

    public List<BsonDocument> getLeaderboardData() {
        var documents = collectionPlayerData.Find(new BsonDocument()).ToList<BsonDocument>();
        return documents;
    }
}
