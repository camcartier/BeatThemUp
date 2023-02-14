using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ElevatorSpawner : MonoBehaviour
{
    [SerializeField] private int _gruntQt, _bigGruntQt, _twinQt, _robotQt;
    [SerializeField] private GameObject _gruntPrefab, _bigGruntPrefab, _twinPrefab, _robotPrefab;
    [SerializeField] private BoolVariables _toFollow; //bool pour dire s'il faut déplacer la caméra pour la raccrocher au joueur
    [SerializeField] private BoolVariables _wave;
    private GameObject _gameManager;

    private void Awake()
    {
        _gameManager = GameObject.FindGameObjectWithTag("GameManager");
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("collision enter");
        if (collision.collider.CompareTag("PlayerBody"))
        {
            Spawn(_gruntQt,_gruntPrefab);
            if (_bigGruntQt>0) Spawn(_bigGruntQt,_bigGruntPrefab);
            if (_twinQt > 0) Spawn(_twinQt, _twinPrefab);
            if (_robotQt > 0) Spawn(_robotQt, _robotPrefab);
            Destroy(gameObject);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("collision");
        if (collision.CompareTag("PlayerBody"))
        {
            Spawn(_gruntQt, _gruntPrefab);
            if (_bigGruntQt > 0) Spawn(_bigGruntQt, _bigGruntPrefab);
            if (_twinQt > 0) Spawn(_twinQt, _twinPrefab);
            if (_robotQt > 0) Spawn(_robotQt, _robotPrefab);
            Destroy(gameObject);
        }
    }

    //supervision du spawn de mob avec un peu de random
    public void Spawn(int gruntQt, GameObject prefab)
    {
        //stop de la caméra
        _toFollow.value = false;
        if (gruntQt > 0)
        {
            int right = 0;
            int left = 0;
            if (gruntQt == 1) { right = 1; left = 0 ; }
            else if (gruntQt == 2)
            {
                right = left = 1;
                    Random.Range(0, gruntQt);
                left = gruntQt - right;
            }
            else if (gruntQt >= 3)
            {
                right = left = gruntQt / 3;
                int rand = gruntQt - right -left;
                int add = Random.Range(0, rand+1);
                right += add;
                left = left + rand - add;
            }
            if (right>0) SpawnRight(right,prefab);
            if (left> 0) SpawnLeft(left,prefab);
        }
        _wave.value = true;
    }

    //spawn de mob à droite de l'écran
    public void SpawnRight(int gruntQt, GameObject prefab)
    {
        for (int i = 0; i < gruntQt; i++)
        {
            //random du x et y
            float x = Random.Range(3, 5) + transform.position.x;
            float y = Random.Range(-1.35f, -1.25f);
            //Instantiate(_gruntPrefab, new Vector2(x, y), Quaternion.identity);
            GameObject grunt = Instantiate(prefab, new Vector2(x, y), Quaternion.identity);
            _gameManager.GetComponent<GameManager>()._inactiveGruntList.Add(grunt);
        }

    }

    //spawn de mob à gauche de l'écran
    public void SpawnLeft(int gruntQt, GameObject prefab)
    {
        for (int i = 0; i < gruntQt; i++)
        {
            //random du x et y
            float x = transform.position.x - Random.Range(3, 5);
            float y = Random.Range(-1.35f, -1.25f);
            //Instantiate(_gruntPrefab, new Vector2(x, y), Quaternion.identity);
            GameObject grunt = Instantiate(prefab, new Vector2(x, y), Quaternion.identity);
            _gameManager.GetComponent<GameManager>()._inactiveGruntList.Add(grunt);
        }
    }
}
