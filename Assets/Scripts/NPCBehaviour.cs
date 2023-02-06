using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class NPCBehaviour : MonoBehaviour
{
    [SerializeField] public int _hp, _attPower, _defPower;
    [SerializeField] private float _moveSpeed;
    private Rigidbody2D _rb; //npc RB2D
    [SerializeField] GameObject _player;
    private Vector2 _direction2Player; //vector2 to have the direction towards the player
    private Transform _playerTransform;
    private float _distance;
    [SerializeField] private float _moveDistance;
    [SerializeField] public IntVariables _playerHealth; //player health scriptableobject to modify it's value on hit
    [SerializeField] private bool _isActive; //is the NPC active (move and attack the player) or inactive
    [SerializeField] private float _attackCD; //attack cooldown timer
    [SerializeField] private bool _onCombat; //is the NPC already fighting or not
    [SerializeField] private Animator _animator;

    [SerializeField] private float _nextAttack =0;

    private void Awake()
    {
        _rb= GetComponent<Rigidbody2D>();
        _playerTransform= _player.GetComponent<Transform>(); 
        GetComponentInChildren<CircleCollider2D>().enabled = false;
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
        Behaviour();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //if (collision.)
        //{
        //    DealDmg();
        //}
    }

    //NPC behaviour method
    private void Behaviour()
    {
        //float _distance = Vector2.Distance(transform.position, _playerTransform.position);
        //Debug.Log(_distance);
        //check si la distance entre l'ennemi et le joueur est suffisante pour le bouger ou le stopper
        if (Vector2.Distance(transform.position, _playerTransform.position) > _moveDistance) Move();
        else 
        { 
            Stop(); 
            if (_isActive && Time.timeSinceLevelLoad>_nextAttack)
            {
                Combat();
            }
            //if (_isActive && _onCombat==false)
            //{
            //    _onCombat = true;
            //    StartCoroutine(Combat()); 
            //}
        }

    }


    private void Move()
    {
        //deplacement vers le joueur, le sprite flip sur X en fonction de la direction du mouvement
        _direction2Player = _playerTransform.position - transform.position;
        _animator.SetFloat("DirX", _direction2Player.x);
        _animator.SetBool("Idle", false);
        if (_direction2Player.x >0) GetComponentInChildren<SpriteRenderer>().flipX= false;
        else if (_direction2Player.x <0) GetComponentInChildren<SpriteRenderer>().flipX= true;
        _rb.velocity = _direction2Player.normalized * _moveSpeed * Time.fixedDeltaTime;
    }

    private void Stop()
    {
        _animator.SetBool("Idle", true);
        _rb.velocity = Vector2.zero;
    }

    private void Combat()
    {
        _animator.SetBool("Attack",true);
        //spawn du collider d'attaque
        GetComponentInChildren<CircleCollider2D>().enabled = true;
        _nextAttack = Time.timeSinceLevelLoad + _attackCD;
        StartCoroutine(AttCD());
    }

    //Coroutine de combat
    //IEnumerator Combat()
    //{
    //    _animator.SetTrigger("Attack");
    //    Attack();

    //    yield return new WaitForSeconds(_attackCD);
    //    _onCombat= false;
    //}
    
    //public void Damage(int damage)
    //{
    //    _hp -= damage;
    //    if (_hp<=0) StartCoroutine(Death());
    //    GetComponentInChildren<CapsuleCollider2D>().enabled = false;
    //}

    private void DealDmg()
    {
        _playerHealth.value -= _attPower;
    }

    //l'ennemi va clignoter un peu avant de mourrir
    IEnumerator Death()
    {
        
        for (int i = 0; i<4;i++)
        {
            GetComponentInChildren<SpriteRenderer>().enabled= false;
            yield return new WaitForSeconds(0.1f);
            GetComponentInChildren<SpriteRenderer>().enabled= true;
        }
        Destroy(gameObject);
        yield return null;
    }

    IEnumerator AttCD()
    {
        yield return new WaitForSeconds(0.05f);
        GetComponentInChildren<CircleCollider2D>().enabled = false;
        _animator.SetBool("Attack", false);
    }
}
