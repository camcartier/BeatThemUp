using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ManaBar : MonoBehaviour
{
    public Slider _slider;

    public void SetMaxMana(int maxmana, int mana)
    {
        _slider.maxValue = maxmana;
        _slider.value = mana;
    }

    public void SetMana(int mana)
    {
        _slider.value = mana;
    }


}
