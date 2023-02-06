using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class LootControls : MonoBehaviour
{
    [SerializeField] float _objPV;
    private GameObject _objet;
    [SerializeField] float _timer;
    private float _timerCounter;


    // Start is called before the first frame update
    void Start()
    {
        _objet = GetComponent<GameObject>();  
        _timerCounter = _timer;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
    }

    void DestroyAfterTime()
    {

    }
}
