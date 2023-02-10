using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ManaBar : MonoBehaviour
{
    public Slider _slider;

    public void SetMaxMana(int health)
    {
        _slider.maxValue = 100;
        _slider.value = 0;
    }

    /*
    public void SetMana(int health)
    {
        _slider.value = health;
    }*/
}
