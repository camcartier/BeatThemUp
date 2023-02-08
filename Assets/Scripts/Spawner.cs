using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] private int _gruntQt;
    [SerializeField] private GameObject _gruntPrefab;
    [SerializeField] private BoolVariables _toFollow; //bool pour dire s'il faut d�placer la cam�ra pour la raccrocher au joueur
    [SerializeField] private BoolVariables _toStop;
    [SerializeField] private BoolVariables _wave;

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
            Debug.Log("spawn enter");
            Spawn(_gruntQt);
            Destroy(gameObject);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("collision");
        if (collision.CompareTag("PlayerBody"))
        {
            Debug.Log("spawn");
            Spawn(_gruntQt);
            Destroy(gameObject);
        }
    }

    //supervision du spawn de mob avec un peu de random
    public void Spawn(int gruntQt)
    {
        //stop de la cam�ra
        _toStop.value = true;
        if (gruntQt > 0)
        {
            if (gruntQt == 1) { SpawnRight(gruntQt); }
            else if (gruntQt >= 2)
            {
                int right = Random.Range(0, gruntQt);
                int left = gruntQt - right;
                if (right > 0) SpawnRight(right);
                if (left > 0) SpawnLeft(left);
            }
        }
        _wave.value = true;
    }

    //spawn de mob � droite de l'�cran
    public void SpawnRight(int gruntQt)
    {
        for (int i = 0; i < gruntQt; i++)
        {
            //random du x et y
            float x = Random.Range(3, 5) + transform.position.x;
            float y = Random.Range(0, 1.7f);
            Instantiate(_gruntPrefab, new Vector2(x, y), Quaternion.identity);
        }

    }

    //spawn de mob � gauche de l'�cran
    public void SpawnLeft(int gruntQt)
    {
        for (int i = 0; i < gruntQt; i++)
        {
            //random du x et y
            float x = transform.position.x - Random.Range(3, 5);
            float y = Random.Range(0, 1.7f);
            Instantiate(_gruntPrefab, new Vector2(x, y), Quaternion.identity);
        }
    }
}