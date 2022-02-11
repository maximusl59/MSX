using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MongoDB.Driver;
using MongoDB.Bson;
using System;

public class MongoDBManager : MonoBehaviour
{
    MongoClient mongoClient;
    IMongoDatabase mongoDatabase;
    IMongoCollection<BsonDocument> collectionStock;

    void Start()
    {
        mongoClient = new MongoClient("mongodb+srv://Everyone:everyone@msx.yauaa.mongodb.net/myFirstDatabase?retryWrites=true&w=majority");
        mongoDatabase = mongoClient.GetDatabase("MSX");
        collectionStock = mongoDatabase.GetCollection<BsonDocument>("stocksList");
    }

    public double GetCurrentStock(int index) {
        var filter = Builders<BsonDocument>.Filter.Eq("id", index);
        try {
            var document = collectionStock.Find(filter).FirstOrDefault();
            return double.Parse(document.GetElement("stockVal").Value.ToString());
        }
        catch (Exception e) {
            Debug.LogError("error " + e);
        }
        return -1;
    }
}
