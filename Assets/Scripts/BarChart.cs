using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System;

public class BarChart : MonoBehaviour
{
    [SerializeField] Text currentDay_text;
    [SerializeField] Text currentCourse_text;
    [SerializeField] Text pastCourse_text;
    [SerializeField] Text currentMoney_text;
    [SerializeField] Text amountCurrency_text;

    float currentCourse;
    float pastCourse;
    int day = 0;
    float k = 0.02f;
    public int interval;
    public float startPrice;
    public float money;
    float amountCurrency;
    public Bar barPrefab;
    List<Bar> bars = new List<Bar>();

    public float[] days;
    public List<float> inputValues;
    float chartHeight;
    System.Random rnd = new System.Random();

    private void Update()
    {
        currentCourse_text.text = currentCourse.ToString("0.0");
        currentDay_text.text = days[days.Length-2].ToString("0");
        pastCourse_text.text = pastCourse.ToString("0.0");
        amountCurrency_text.text = amountCurrency.ToString("0");
        currentMoney_text.text = money.ToString("0.00");


        currentCourse = inputValues[inputValues.Count - 1];
        pastCourse = inputValues[inputValues.Count - 2];
    }

    void Start()
    {
        InvokeRepeating("AddAndDisplay", interval, interval);
        inputValues[0] = startPrice;
        days[0] = 0;

        for (int i = 1; i < inputValues.Count; i++)
        {
            days[i] = i;
            //Было:
            //inputValues[i] += inputValues[i-1] * (1 + k * (Random.RandomRange(0f,1f) - 0.5f));

            //Стало:
            /////////////////////////////////////////////////
            float v1 = 0.04f;
            float v2 = 0.2f;
            float t = 0.1f;
            float c = (float)Math.Exp(t * (v1 - Math.Pow(v2, 2) * 0.5f));
            float b = v2 * (float)Math.Sqrt(t);
            inputValues[i] += inputValues[i-1] * c * (float)Math.Exp(b * (rnd.NextDouble() - 0.5));
            /////////////////////////////////////////////////

        }

        DisplayGraph();
    }

    void DisplayGraph()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Destroy(transform.GetChild(i).gameObject);
        }

        float maxValue = inputValues.Max();
        float minValue = inputValues.Min();

        for (int i = 0; i < inputValues.Count; i++)
        {
            Bar newBar = Instantiate(barPrefab) as Bar;
            newBar.transform.SetParent(transform);
            newBar.slider.maxValue = maxValue;
            newBar.slider.minValue = minValue - 1;
            newBar.slider.value = inputValues[i];
            newBar.barValue.text = inputValues[i].ToString("0.0");
            newBar.barDay.text = days[i].ToString("0");

        }
    }

    public void AddAndDisplay()
    {
        AddNewValue();
        DisplayGraph();

        for (int i = 0; i < inputValues.Count; i++)
        {
            days[i]++;

        }

    }

    void AddNewValue()
    {
        inputValues.Add(inputValues[inputValues.Count-1] * (1 + k * (UnityEngine.Random.RandomRange(0f, 1f) - 0.5f)));
        inputValues.RemoveAt(0);
    }


    public void Sell()
    {
        if (amountCurrency > 0)
        {
            money += currentCourse;
            amountCurrency--;
        }
    }

    public void Buy()
    {
        if (money >= currentCourse)
        {
            money -= currentCourse;
            amountCurrency++;
        }
    }
}
