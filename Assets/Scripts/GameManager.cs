using Cinemachine;
using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private IntVariables _playerHP;
    [SerializeField] private GameObject _camera;
    [SerializeField] private Transform _playerTransform;
    [SerializeField] private BoolVariables _toFollow; //bool pour dire s'il faut d�placer la cam�ra pour la raccrocher au joueur
    [SerializeField] private BoolVariables _wave;
    [SerializeField] private IntVariables _enemyCount;
    Vector2 _center;

    // Start is called before the first frame update
    void Start()
    {
        _playerHP.value = 100;
        _enemyCount.value = 0;
        _wave.value = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}
