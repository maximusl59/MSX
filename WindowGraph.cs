using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CodeMonkey.Utils;

public class WindowGraph : MonoBehaviour
{
    [SerializeField]
    private GameObject dotPrefab;
    [SerializeField]
    private GameObject linePrefab;
    private RectTransform graphParent;
    private List<GameObject> refreshList = new List<GameObject>();

    void Awake() {
        graphParent = GameObject.Find("GraphParent").GetComponent<RectTransform>();
    }

    void Start() {
        graphWidth = graphParent.sizeDelta.x;
        graphHeight = graphParent.sizeDelta.y;
    }

    GameObject CreatePoint(Vector2 anchoredPos) {
        GameObject dotClone = Instantiate(dotPrefab, graphParent);
        refreshList.Add(dotClone);
        RectTransform rectTransform = dotClone.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = anchoredPos;
        return dotClone;
    }

    void CreateLine(Vector2 posA, Vector2 posB) {
        GameObject lineClone = Instantiate(linePrefab, graphParent);
        RectTransform rectTransform = lineClone.GetComponent<RectTransform>();
        Vector2 dir = (posB - posA).normalized;
        refreshList.Add(lineClone);
        float distance = Vector2.Distance(posA, posB);
        rectTransform.sizeDelta = new Vector2(distance, 3f);
        rectTransform.anchoredPosition = posA + dir * distance * 0.5f;
        rectTransform.eulerAngles = new Vector3(0, 0, UtilsClass.GetAngleFromVector(dir));
    }

    public string stockGraphName;

    void SetupTrigger(GameObject dotClone, double stockValue) {
        float graphHeight = graphParent.sizeDelta.y;
        float triggerPosY = (0.5f * graphHeight) - dotClone.GetComponent<RectTransform>().anchoredPosition.y;
        BoxCollider2D childTrigger = dotClone.GetComponentInChildren<BoxCollider2D>();
        childTrigger.size = new Vector2(childTrigger.size.x, graphHeight);
        childTrigger.offset = new Vector2(childTrigger.offset.x, triggerPosY);
        InteractableUI interactableUI = dotClone.GetComponentInChildren<InteractableUI>();
        interactableUI.setThisStockVal(stockValue);
    }

    double xMin;
    double yMin;

    public double xMax = 100f;
    public double yMax = 100f;
    public double xPadding;
    public double yPadding;

    double graphWidth;
    double graphHeight;

    public void ShowGraph(LinkedList<Tuple<float, double>> listOfPoints) {
        RefreshGraph();
        ResizeGraph(listOfPoints);

        ShowGraphHelper(listOfPoints.First, null);
    }

    private void ShowGraphHelper(LinkedListNode<Tuple<float, double>> graphPointNode, GameObject lastGO) {
        if (graphPointNode == null)
            return;

        double xPos = ((graphPointNode.Value.Item1 - xMin) / xMax) * graphWidth;
        double yPos = ((graphPointNode.Value.Item2 - yMin) / yMax) * graphHeight;
        if (lastGO != null) {
            CreateLine(lastGO.GetComponent<RectTransform>().anchoredPosition, new Vector2((float)xPos, (float)yPos));
        }
        lastGO = CreatePoint(new Vector2((float)xPos, (float)yPos));
        SetupTrigger(lastGO, graphPointNode.Value.Item2);

        ShowGraphHelper(graphPointNode.Next, lastGO);
    }

    public void RefreshGraph() {
        foreach(GameObject go in refreshList) {
            Destroy(go);
        }
    }

    void ResizeGraph(LinkedList<Tuple<float, double>> listOfPoints) {
        xMin = listOfPoints.First.Value.Item1;
        xMax = listOfPoints.Last.Value.Item1 - xMin + xPadding;

        if (listOfPoints.Last.Value.Item2 > yMax + yPadding)
            yMax = listOfPoints.Last.Value.Item2 + yPadding;
    }

    public void AxisRescaleY(LinkedList<Tuple<float, double>> listOfPoints) {
        double max = AxisRescaleYHelperMax(listOfPoints.First);
        yMax = max + yPadding;
    }

    private double AxisRescaleYHelperMax(LinkedListNode<Tuple<float, double>> graphPointNode) {
        if (graphPointNode == null)
            return 0;
        double result = graphPointNode.Value.Item2;
        double next = AxisRescaleYHelperMax(graphPointNode.Next);
        if (next > result)
            return next;
        return result;
    }

    private double AxisRescaleYHelperMin(LinkedListNode<Tuple<float, double>> graphPointNode) {
        if (graphPointNode == null)
            return Mathf.Infinity;
        double result = graphPointNode.Value.Item2;
        double next = AxisRescaleYHelperMin(graphPointNode.Next);
        if (next < result)
            return next;
        return result;
    }
}
