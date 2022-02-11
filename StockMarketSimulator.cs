using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class StockMarketSimulator : MonoBehaviour
{
    public float maxStockValueDelta;
    public float minTimer;
    public float maxTimerDeltaIncludesMultiplier;
    public int timerMultipler;

    public int crashOrSpikeChance;
    public float crashPercentage;
    public float spikePercentage;

    public float currentStockValue;
    private float lastStockValue;
    private float nextStockValue;

    void Start()
    {
        currentStockValue = 0;
        UpdateStock();
    }

    void UpdateStock() {
        float deltaCurrency = GenerateDelta("currency");
        //SetupInterpolation(deltaCurrency);
        if (!DidCrashOrSpike())
            UpdateValue(deltaCurrency);

        float deltaTimer = GenerateDelta("timer");
        //StartCoroutine(InterpolateRoutine(deltaTimer));
        StartCoroutine(WaitTimer(deltaTimer));
    }

    //void SetupInterpolation(float deltaCurrency) {
    //    lastStockValue = currentStockValue;
    //    if (currentStockValue + deltaCurrency < 0)
    //        nextStockValue = 0;
    //    else
    //        nextStockValue = currentStockValue + deltaCurrency;
    //}

    bool DidCrashOrSpike() {
        int rng = Random.Range(0, crashOrSpikeChance);
        if (rng == 0) {
            currentStockValue *= crashPercentage;
            return true;
        } else if (rng == 1) {
            currentStockValue *= spikePercentage;
            return true;
        }
        return false;
    }

    void UpdateValue(float delta) {
        if(currentStockValue + delta < 0) 
            currentStockValue = 0;
        else 
            currentStockValue += delta;
    }

    float GenerateDelta(string whichDelta) {
        float result = 0;

        if(whichDelta == "currency") {
            result = Random.Range(-maxStockValueDelta, maxStockValueDelta);
        } else if(whichDelta == "timer") {
            result = Random.Range(minTimer, maxTimerDeltaIncludesMultiplier);
        }

        return result;
    }

    IEnumerator WaitTimer(float timeToWait) {
        yield return new WaitForSeconds(timeToWait * timerMultipler);
        UpdateStock();
    }

    //private IEnumerator InterpolateRoutine(float duration) {
    //    float lerp = 0;
    //    while (duration > 0 && lerp < 1) {
    //        lerp = Mathf.MoveTowards(lerp, 1, Time.deltaTime / duration);
    //        currentStockValue = Mathf.Lerp(lastStockValue, nextStockValue, lerp);

    //        yield return null;
    //    }
    //    currentStockValue = nextStockValue;

    //    UpdateStock();
    //}

    public float getCurrentStockValue() {
        return currentStockValue;
    }
}
