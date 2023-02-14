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

    private void Awake()
    {
        _vmHP = 40;
        _player = GameObject.Find("Player");
        _animator = GetComponentInChildren<Animator>();   
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
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("collide");

        if (collision.CompareTag("PlayerFist") && _player.GetComponent<PlayerMovement>()._isAttacking == true)
        {
            Debug.Log("oui");
            _vmHP -= _player.GetComponent<PlayerMovement>().PlayerAttPower;
        }

    }

    void DestroyVM()
    {
        gameObject.GetComponent<SpriteRenderer>().enabled = false;
        Instantiate(healthCan, transform.position, Quaternion.identity);
        Destroy(this);

    }
}
