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


            /*
            if (_canTakeDamage)
            {
                _canTakeDamage = false;
                Debug.Log("canbedamage");
                _animator.SetBool("punched", true);
                _destroyableHP -= _player.GetComponent<PlayerMovement>().PlayerAttPower;
            }*/



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
        gameObject.GetComponentInChildren<SpriteRenderer>().enabled = false;
        Destroy(this.gameObject);

    }

    void SpawnLoot()
    {
        Instantiate(cassette, new Vector2(transform.position.x + 0.5f, transform.position.y), Quaternion.identity);
    }

}
