using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class LootControls : MonoBehaviour
{
    private GameObject _objet;
    public SpriteRenderer _myObjectSprite;
    [SerializeField] private IntVariables _score;
    private int _vynilScoreValue = 100;
    private int _cassettelScoreValue = 200;

    [SerializeField] float _timer;
    private float _timerCounter;
    private float _fadeTimer = 4f;
    

    // Start is called before the first frame update
    void Start()
    {
        _objet = GetComponent<GameObject>();
        _myObjectSprite = GetComponentInChildren<SpriteRenderer>();

        _timerCounter = 0;
        
    }

    // Update is called once per frame
    void Update()
    {
        _timerCounter += Time.deltaTime;

        if (_fadeTimer - _timerCounter < 0)
        {
            //StartCoroutine(Clignote());
            //Clignote();
            StartCoroutine(Clignote());
        }

        if (_timer - _timerCounter < 0)
        {
            Destroy(gameObject);
        }

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerBody"))
        {
            _score.value += _vynilScoreValue;
            Destroy(gameObject);

        }
    }


    IEnumerator Clignote()
    {
        for (int i = 0; i < 20; i++)
        {
            yield return new WaitForSeconds(0.05f);
            GetComponentInChildren<SpriteRenderer>().enabled = false;
            yield return new WaitForSeconds(0.05f);
            GetComponentInChildren<SpriteRenderer>().enabled = true;
        }
    }

}
