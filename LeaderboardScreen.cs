using MongoDB.Bson;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class LeaderboardScreen : MonoBehaviour
{
    MongoDBAccount dBAccount;
    AccountManager accountManager;

    private List<GameObject> leaderboardRowsGO = new List<GameObject>();
    private List<LeaderboardRowData> leaderboardRows = new List<LeaderboardRowData>();

    public GameObject rowPrefab;
    public RectTransform content;

    double[] currentStockByIndex;

    void Start()
    {
        accountManager = GameObject.Find("AccountManager").GetComponent<AccountManager>();
        dBAccount = GameObject.Find("AccountManager").GetComponent<MongoDBAccount>();
    }

    private float nextActionTime = 0.0f;
    public float updatePeriod;

    void Update() {
        if (Time.time > nextActionTime) {
            nextActionTime += updatePeriod;
            UpdateLeaderboard();
        }
    }

    void UpdateStocks() {
        currentStockByIndex = new double[accountManager.numOfStocks];
        for(int i = 0; i < accountManager.numOfStocks; i++) {
            currentStockByIndex[i] = accountManager.getCurrentStockPrice(i);
        }
    }

    void UpdateLeaderboard()
    {
        RefreshLeaderboard();
        var documents = dBAccount.getLeaderboardData();
        UpdateStocks();
        for (int i = 0; i < documents.Count; i++) {
            LeaderboardRowData row = new LeaderboardRowData {
                username = ParseName(documents[i]),
                total = ParseTotal(documents[i])
            };
            leaderboardRows.Add(row);
        }

        leaderboardRows = leaderboardRows.OrderByDescending(o => o.total).ToList();

        for (int i = 0; i < leaderboardRows.Count; i++) {
            GameObject clone = Instantiate(rowPrefab, content);
            LeaderboardRow row = clone.GetComponent<LeaderboardRow>();
            int temp = i;

            row.username = leaderboardRows[i].username;
            row.total = leaderboardRows[i].total;
            row.rank = temp + 1;
            row.Init();
            leaderboardRowsGO.Add(clone);
        }
    }

    void RefreshLeaderboard() {
        int count = leaderboardRowsGO.Count;
        for (int i = 0; i < count; i++) {
            Destroy(leaderboardRowsGO[0].gameObject);
            leaderboardRowsGO.RemoveAt(0);
            leaderboardRows.RemoveAt(0);
        }
    }

    double ParseTotal(BsonDocument document) {
        document.TryGetValue("Balance", out BsonValue balanceV);
        document.TryGetValue("AmogusStock", out BsonValue amogusStockV);
        document.TryGetValue("GameStopStock", out BsonValue gameStopStockV);
        document.TryGetValue("NvidiaStock", out BsonValue nvidiaStockV);
        document.TryGetValue("DaBabyStock", out BsonValue daBabyStockV);
        document.TryGetValue("BezosStock", out BsonValue bezosStockV);

        double balance = double.Parse(balanceV.ToString());
        double amogusStock = double.Parse(amogusStockV.ToString());
        double gameStopStock = double.Parse(gameStopStockV.ToString());
        double nvidiaStock = double.Parse(nvidiaStockV.ToString());
        double daBabyStock = double.Parse(daBabyStockV.ToString());
        double bezosStock = double.Parse(bezosStockV.ToString());


        double total = balance
            + (amogusStock * currentStockByIndex[0])
            + (gameStopStock * currentStockByIndex[1])
            + (nvidiaStock * currentStockByIndex[2])
            + (daBabyStock * currentStockByIndex[3])
            + (bezosStock * currentStockByIndex[4]);

        return total;
    }

    string ParseName(BsonDocument document) {
        document.TryGetValue("Name", out BsonValue nameV);
        return nameV.ToString();
    }

    class LeaderboardRowData {
        public string username;
        public double total;
    }
}
