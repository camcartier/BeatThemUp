using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScoreControl : MonoBehaviour
{
    private TMP_Text _counterTxt;
    [SerializeField] private IntVariables _scoreCount;

    // Start is called before the first frame update
    private void Awake()
    {
        _counterTxt = GetComponent<TMP_Text>();
    }

    // Update is called once per frame
    void Update()
    {
        _counterTxt.text = ($"{_scoreCount.value}");
    }
}
