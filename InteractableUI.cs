using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableUI : MonoBehaviour
{
    public float normalSize;
    public float growthSize;

    public float textOffsetX;

    private double thisStockVal;
    private static string stockGraphName = "StockGraph";

    void OnMouseOver() {
        RectTransform rectTransform = this.GetComponentInParent<RectTransform>();
        rectTransform.localScale = new Vector2(growthSize, growthSize);
        GameObject.Find(stockGraphName).GetComponent<StockGraphing>().SetInteractableText(thisStockVal, rectTransform.anchoredPosition.x + textOffsetX);
    }

    void OnMouseExit() {
        RectTransform rectTransform = this.GetComponentInParent<RectTransform>();
        rectTransform.localScale = new Vector2(normalSize, normalSize);
        GameObject.Find(stockGraphName).GetComponent<StockGraphing>().DisableInteractableText();
    }

    public void setThisStockVal(double stockVal) {
        thisStockVal = stockVal;
    }
}
