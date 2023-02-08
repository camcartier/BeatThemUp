using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class LootControls : MonoBehaviour
{
    private GameObject _objet;
    public SpriteRenderer _myObjectSprite;


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
            Destroy(gameObject);
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
            Destroy(gameObject);

        }
    }

    /*
    void Clignote()
    {
        float timeOn = 1f;
        float timeOnCounter = 0;
        float timeOff = 0.25f;
        float timeOffCounter = 0;

        timeOnCounter += Time.deltaTime;

        if (gameObject.GetComponentInChildren<SpriteRenderer>().enabled == true)
        {
            
            if (timeOn - timeOnCounter < 0)
            {
                gameObject.GetComponentInChildren<SpriteRenderer>().enabled = false;
                timeOnCounter = 0f;
            }

        }
        if (gameObject.GetComponentInChildren<SpriteRenderer>().enabled == false)
        {
            timeOffCounter += Time.deltaTime;
            if (timeOff - timeOffCounter < 0)
            {
                gameObject.GetComponentInChildren<SpriteRenderer>().enabled = true;
                timeOffCounter = 0f;
            }
        }
    }*/

    //jpigepas pk ça marche pas
    //en fait ça marche
    //enfin presque
    //NON
    /*
    IEnumerator Clignote()
    {
        float timer = .5f;
        float timerCounter = 0;

        for (int i = 0; i<10; i++)
        {
            _myObjectSprite.enabled = false;
            yield return new WaitForSeconds(.5f);
            _myObjectSprite.enabled = true;
            yield return new WaitForSeconds(.85f);
            timerCounter += Time.deltaTime;
            if (timer - timerCounter  < 0)
            {
                i++;
                timerCounter = 0;
                Debug.Log("looped");
            }


        }

    }*/



    /*
    void Clignote2()
    {
        _myObjectSprite.enabled = true;



    }*/

}
