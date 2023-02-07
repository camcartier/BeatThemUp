using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private IntVariables _playerHP;

    // Start is called before the first frame update
    void Start()
    {
        _playerHP.value = 100;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
