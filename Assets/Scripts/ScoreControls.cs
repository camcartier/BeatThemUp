using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScoreControls : MonoBehaviour
{
    private TMP_Text _counterTxt;
    private int _changeTracker;
    [SerializeField] private IntVariables _scoreCount;

    // Start is called before the first frame update
    private void Awake()
    {
        _counterTxt = GetComponent<TMP_Text>();
        _changeTracker = 0;
    }

    void Start()
    {
        _counterTxt.text = ($"{_scoreCount.value}");
    }

    // Update is called once per frame
    void Update()
    {
        if (_scoreCount.value != _changeTracker)
        {
            _counterTxt.text = ($"{_scoreCount.value}");
            _changeTracker = _scoreCount.value;
        }


    }
}
