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
    [SerializeField] private BoolVariables _toFollow; //bool pour dire s'il faut déplacer la caméra pour la raccrocher au joueur
    [SerializeField] private BoolVariables _toStop;
    [SerializeField] private BoolVariables _wave;
    [SerializeField] private IntVariables _enemyCount;
    Vector2 _center;

    // Start is called before the first frame update
    void Start()
    {
        _playerHP.value = 100;
        _enemyCount.value = 0;
        _toFollow.value = true;
        _wave.value = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (_toStop.value) { CameraStop(); }
        else if (_toFollow.value==true) { CameraFollow(); }
        else if (_wave.value && _enemyCount.value == 0) { _toFollow.value = true; _wave.value = false; }
    }

    private void CameraStop()
    {
        _camera.GetComponent<CinemachineBrain>().enabled = false;
        _toStop.value = false;
    }

    private void CameraFollow()
    {
        if (Vector2.Distance(transform.position, _playerTransform.position) > 0.2f) transform.position = Vector2.Lerp(transform.position, _playerTransform.position, 1);
        else { _camera.GetComponent<CinemachineBrain>().enabled = true; _toFollow.value = false; }
    }
}
