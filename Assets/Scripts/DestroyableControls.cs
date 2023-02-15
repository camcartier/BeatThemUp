using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyableControls : MonoBehaviour
{

    [SerializeField] private int _destroyableMaxHP = 40;
    [SerializeField] public int _destroyableHP;
    private int _currentHP;
    private GameObject _player;
    private Animator _animator;
    [SerializeField] GameObject cassette;
    //[SerializeField] GameObject poofingFX;
    private bool _canTakeDamage = true;
    private bool _canInstantiate = true;

    private float timer;
    private float timercounter;

    private void Awake()
    {
        _destroyableHP = _destroyableMaxHP;
        _currentHP = _destroyableHP;
        _player = GameObject.Find("Player");
        _animator = GetComponentInChildren<Animator>();
        timercounter = 0;
        timer = 20f * Time.deltaTime;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (_destroyableHP < _destroyableMaxHP)
        {
            _animator.SetBool("punched", true);
        }

        if(!_canTakeDamage)
        {
            if (timercounter < timer)
            {
                timercounter +=Time.deltaTime;
            }
            else
            {
                timercounter = 0;
                SpawnLoot();
                _canTakeDamage = true;
            }
        }

        /*
        if (_destroyableHP <= 0 && _canInstantiate)
        {
            SpawnLoot();
            _canInstantiate = false;
        }*/
        if (_destroyableHP <= 0 && gameObject.tag != "StreetLamp")
        {
            DestroyDestroyable();
        }

    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerFist") && _player.GetComponent<PlayerMovement>()._isAttacking == true && _canTakeDamage)
        {
            _canTakeDamage = false;
            Debug.Log("canbedamage");
            _animator.SetBool("punched", true);
            _destroyableHP -= _player.GetComponent<PlayerMovement>().PlayerAttPower;

            if (gameObject.tag == "StreetLamp")
            {
                Debug.Log("street");
                gameObject.GetComponentInChildren<BoxCollider2D>().enabled = false;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerFist"))
        {
            _canTakeDamage = true;
        }
    }

    void DestroyDestroyable()
    {
        Destroy(this.gameObject);
    }

    /*
    void SpawnLoot()
    {
        float randomNumberX = Random.Range(0.1f, 0.7f);
        float randomNumberY = Random.Range(-0.5f, 0.5f);
        Instantiate(cassette, new Vector2(transform.position.x + randomNumberX, transform.position.y + randomNumberY), Quaternion.identity);
    }*/

    void SpawnLoot()
    {
        float randomNumberX = Random.Range(0.1f, 0.7f);
        float randomNumberY = Random.Range(-0.5f, 0.5f);
        if (transform.position.x - _player.transform.position.x > 0f)
        {
            Instantiate(cassette, new Vector2(transform.position.x + randomNumberX, transform.position.y + randomNumberY), Quaternion.identity);
        }
        else
        {
            Instantiate(cassette, new Vector2(transform.position.x - randomNumberX, transform.position.y - randomNumberY), Quaternion.identity);
        }
    }
            

    #region test
    /*
    public float _kbTimerCounter;
    public float _kbDuration;
    [SerializeField] AnimationCurve _kbCurveY;
    float tempposY;


    public void KnockBack()
    {
        float randomNumberX = Random.Range(0.1f, 0.7f);
        float randomNumberY = Random.Range(-0.5f, 0.5f);
        if (_kbTimerCounter < _kbDuration)
        {
            _kbTimerCounter += Time.deltaTime;
            float y = _kbCurveY.Evaluate(_kbTimerCounter / _kbDuration);
            if (transform.position.x - _player.transform.position.x > 0f)
            {
                transform.position = new Vector2(Mathf.Lerp(transform.position.x, transform.position.x + randomNumberX, _kbDuration), tempposY + 2 * y);
            }
            else
            {
                transform.position = new Vector2(Mathf.Lerp(transform.position.x, transform.position.x - 0.05f, _kbDuration), tempposY + 2 * y);
            }

        }
        else
        {
            _kbTimerCounter = 0;
        }
    }
    */
    #endregion


}
