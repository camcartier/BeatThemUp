using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyableControls : MonoBehaviour
{

    [SerializeField] private int _destroyableMaxHP = 20;
    [SerializeField] public int _destroyableHP;
    private GameObject _player;
    private Animator _animator;
    //[SerializeField] GameObject cassette;
    //[SerializeField] GameObject poofingFX;
    private bool _canTakeDamage = true;

    private void Awake()
    {
        _destroyableHP = _destroyableMaxHP;
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
        if (_destroyableHP < _destroyableMaxHP)
        {
            _animator.SetBool("punched", true);
        }

        if (_destroyableHP <= 0)
        {
            DestroyDestroyable();
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerFist") && _player.GetComponent<PlayerMovement>()._isAttacking == true && _canTakeDamage)
        {
            Debug.Log("aaaa");
            _canTakeDamage = false;
            _animator.SetBool("punched", true);
            _destroyableHP -= _player.GetComponent<PlayerMovement>().PlayerAttPower;
            StartCoroutine(WaitForDestroy());
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

    IEnumerator WaitForDestroy()
    {
        yield return new WaitForSeconds(1f);
        gameObject.GetComponentInChildren<SpriteRenderer>().enabled = false;

        Destroy(this.gameObject);
    }
}
