using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] Transform _playerTransform;
    [SerializeField] private BoolVariables _toFollow; //bool pour dire s'il faut déplacer la caméra pour la raccrocher au joueur
    [SerializeField] private BoolVariables _wave;
    [SerializeField] private IntVariables _enemyCount;

    // Start is called before the first frame update
    void Start()
    {
        _toFollow.value = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (_toFollow.value == true) { CameraFollow(); }
        else if (_wave.value && _enemyCount.value == 0) { _toFollow.value = true; _wave.value = false; }
    }


    private void CameraFollow()
    {
        Vector2 cammove = transform.position;
        Vector2 pPos = _playerTransform.position;
        cammove.y = pPos.y = 1.5f;
         if (Vector2.Distance(cammove, pPos) > 0.2f) transform.position = Vector2.Lerp(cammove, pPos, 1);
        else 
        { 
            transform.position = new Vector3 (pPos.x,pPos.y,-10);
        }
    }
}
