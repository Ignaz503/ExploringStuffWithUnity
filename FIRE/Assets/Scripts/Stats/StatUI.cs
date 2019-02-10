using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StatUI : MonoBehaviour
{
    public event Action<float> OnStatIncreaseRemainderReturn;

    Stat statDisplayed;
    Func<float> increaseAmountGetter;

    [SerializeField] Button increaseBtn = null;
    [SerializeField] TextMeshProUGUI nameTextField = null;
    [SerializeField] TextMeshProUGUI valueTextField = null;

    public void SetStatResponsible(Func<float> increaseAmountGetter, Stat st)
    {
        statDisplayed = st;

        if (increaseAmountGetter == null)
            throw new Exception("Increase Amount getter can't be null");

        this.increaseAmountGetter = increaseAmountGetter;

        Setup();
    }

    private void Setup()
    {
        SetUpTextFields();
        SetupButton();
    }

    private void SetupButton()
    {
        increaseBtn.onClick.AddListener(IncreaseStat);
    }

    void SetUpTextFields()
    {
        nameTextField.text = statDisplayed.Name;

        SetValueTextField();

        statDisplayed.OnStatChange += (s) => SetValueTextField();
    }

    void SetValueTextField()
    {
        //valueTextField.text = string.Format("{0:N2}", statDisplayed.Value);
        SetNumberTextField(valueTextField, statDisplayed.Value, 2);
    }

    void SetNumberTextField(TextMeshProUGUI textfield, float number, int decPoints)
    {
        textfield.text = string.Format($"{{0:N{decPoints}}}", number);
    }

    public void IncreaseStat()
    {
        float diff = statDisplayed.Change(increaseAmountGetter());
        OnStatIncreaseRemainderReturn?.Invoke(diff);
    }

}
