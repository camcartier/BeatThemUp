using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class NPCBehaviour : MonoBehaviour
{
    [SerializeField] private int _hp, _attPower, _defPower;
    [SerializeField] private float _moveSpeed;
    private Rigidbody2D _rb; //npc RB2D
    [SerializeField] GameObject _player;
    private Vector2 _direction2Player; //vector2 to have the direction towards the player
    private Transform _playerTransform;
    [SerializeField] private float _moveDistance;
    [SerializeField] private IntVariables _playerHealth; //player health scriptableobject to modify it's value on hit
    private bool _isActive; //is the NPC active (move and attack the player) or inactive
    [SerializeField] private float _attackCD; //attack cooldown timer
    private bool _onCombat; //is the NPC already fighting or not


    private void Awake()
    {
        _rb= GetComponent<Rigidbody2D>();
        _playerTransform= _player.GetComponent<Transform>();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        Move();
    }

    //NPC behaviour method
    private void Behaviour()
    {
        //check si la distance entre l'ennemi et le joueur est suffisante pour le bouger ou le stopper
        if (Vector2.Distance(transform.position, _playerTransform.position) > _moveDistance) Move();
        else { Stop(); if (_isActive && !_onCombat) StartCoroutine(Combat()); }

    }


    private void Move()
    {
        //deplacement vers le joueur
        _direction2Player = _playerTransform.position - transform.position;
        _rb.velocity = _direction2Player * _moveSpeed * Time.fixedDeltaTime;
    }

    private void Stop()
    {
        _rb.velocity = Vector2.zero;
    }

    //Coroutine de combat
    IEnumerator Combat()
    {
        _onCombat= true;
        for (int i=0; i<3;i++)
        {
            Attack();
            yield return new WaitForSeconds(_attackCD);
        }
        yield return new WaitForSeconds(_attackCD);
        _onCombat= false;
    }

    private void Attack()
    {
        _playerHealth.value -= _attPower;
    }


}
