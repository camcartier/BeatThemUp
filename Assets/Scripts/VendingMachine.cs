using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VendingMachine : MonoBehaviour
{
    private int _vmMaxHP = 40;
    private int _vmHP;
    private GameObject _player;
    private Animator _animator;
    [SerializeField] GameObject healthCan;

    private bool _canTakeDamage = true;
    private bool _canInstantiate = true;

    private float timer;
    private float timercounter;

    private void Awake()
    {
        _vmHP = 40;
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
        if (_vmHP < _vmMaxHP)
        {
            _animator.SetBool("punched", true);
        }

        if (!_canTakeDamage)
        {
            if (timercounter < timer)
            {
                timercounter += Time.deltaTime;
            }
            else
            {
                timercounter = 0;
                SpawnLoot();
                _canTakeDamage = true;
            }
        }

        if (_vmHP <= 0 && _canInstantiate)
        {
            SpawnLoot();
            _canInstantiate = false;
        }
        if (_vmHP <= 0)
        {
            Destroy(this.gameObject);
        }

    }

    /*
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("collide");

        if (collision.CompareTag("PlayerFist") && _player.GetComponent<PlayerMovement>()._isAttacking == true)
        {
            Debug.Log("oui");
            _vmHP -= _player.GetComponent<PlayerMovement>().PlayerAttPower;
        }

    }*/


    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerFist") && _player.GetComponent<PlayerMovement>()._isAttacking == true && _canTakeDamage)
        {
            _canTakeDamage = false;
            _animator.SetBool("punched", true);
            _vmHP -= _player.GetComponent<PlayerMovement>().PlayerAttPower;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerFist"))
        {
            _canTakeDamage = true;
        }
    }

    void DestroyVM()
    {
        gameObject.GetComponent<SpriteRenderer>().enabled = false;
        Instantiate(healthCan, transform.position, Quaternion.identity);
        Destroy(this);

    }

    void SpawnLoot()
    {
        float randomNumberX = Random.Range(0.1f, 0.7f);
        float randomNumberY = Random.Range(-0.5f, 0.5f);
        if (transform.position.x - _player.transform.position.x > 0f)
        {
            Instantiate(healthCan, new Vector2(transform.position.x + randomNumberX, transform.position.y + randomNumberY), Quaternion.identity);
        }
        else
        {
            Instantiate(healthCan, new Vector2(transform.position.x - randomNumberX, transform.position.y - randomNumberY), Quaternion.identity);
        }
    }

}
