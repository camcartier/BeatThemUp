using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class NPCBehaviour : MonoBehaviour
{
    [SerializeField] public int _hp, _attPower, _attSpePower, _defPower;
    private int _lastHP;
    [SerializeField] private float _moveSpeed;
    private Rigidbody2D _rb; //npc RB2D
    private GameObject _player;
    private Vector2 _direction2Player; //vector2 to have the direction towards the player
    private Transform _playerTransform;
    private float _distance;
    [SerializeField] private float _moveDistance;
    [SerializeField] public IntVariables _playerHealth; //player health scriptableobject to modify it's value on hit
    [SerializeField] public bool _isActive; //is the NPC active (move and attack the player) or inactive
    [SerializeField] private float _attackCD; //attack cooldown timer
    [SerializeField] private bool _onCombat; //is the NPC already fighting or not
    [SerializeField] private Animator _animator;

    [SerializeField] private float _nextAttack =0;
    [SerializeField] private IntVariables _enemyCount;
    [SerializeField] private IntVariables _currentActiveGrunt; //quantit� de grunts active � l'instant T
    private bool _isDead;
    private bool _flipX;
    [SerializeField] GameObject _prefabVynil;
    [SerializeField] GameObject _prefabPoofingFX;
    private GameObject _gameManager;
    [SerializeField] public IntVariables _playerMana; 
    [SerializeField] private int _manaGainValue = 10;

    [SerializeField] private bool grunt, biggrunt, twin, robotnik;
    private int pattern;

    private void Awake()
    {
        _rb= GetComponent<Rigidbody2D>();
        _player = GameObject.FindGameObjectWithTag("PlayerBody");
        _playerTransform= _player.GetComponent<Transform>();
        _isActive= false;
        _isDead= false;
        _lastHP = _hp;
        _gameManager = GameObject.FindGameObjectWithTag("GameManager");
        pattern = 0;
    }
    // Start is called before the first frame update
    void Start()
    {
        _enemyCount.value++;
    }

    // Update is called once per frame
    void Update()
    {
        if (_hp <=0 && _isDead==false) { StartCoroutine(Death()); _isDead = true; }

        if (_lastHP != _hp && !_isDead)
        {
            _lastHP = _hp;
            KnockBack();
            if (_playerMana.value < 100) { _playerMana.value += _manaGainValue; } else { _playerMana.value = 100; }

            _nextAttack = Time.timeSinceLevelLoad + _attackCD;
        }
    }

    private void FixedUpdate()
    {
        if (_isActive && _isDead==false)
        {
            if (grunt) GruntBehaviour();
            else if (biggrunt) BigBehaviour();
            else if (twin) TwinBehaviour();
            else if (robotnik) RobotBehaviour();
        }
            
        else if (!_isActive && _isDead == false )
        {
            if (grunt) InactiveBehaviour();
            else if (biggrunt) InactiveBigBehaviour();

        }
    }

    private void InactiveBehaviour()
    {
        if (Vector2.Distance(transform.position, _playerTransform.position) > _moveDistance * 6) Move();
        else if(Vector2.Distance(transform.position, _playerTransform.position) < _moveDistance * 4) MoveAway();
        else Stop();
    }

    private void InactiveBigBehaviour()
    {
        if (Vector2.Distance(transform.position, _playerTransform.position) > _moveDistance * 3) Move();
        else if (Vector2.Distance(transform.position, _playerTransform.position) < _moveDistance * 2) MoveAway();
        else Stop();
    }

    //NPC behaviour method
    private void GruntBehaviour()
    {
        //check si la distance entre l'ennemi et le joueur est suffisante pour le bouger ou le stopper
        if (Vector2.Distance(transform.position, _playerTransform.position) > _moveDistance) Move();
        else 
        { 
            Stop(); 
            if (_isActive && Time.timeSinceLevelLoad>_nextAttack && _isDead == false)
            {
                Combat();
            }
        }

    }

    private void BigBehaviour()
    {
        if (Vector2.Distance(transform.position, _playerTransform.position) > _moveDistance) Move();
        else
        {
            Stop();
            if (_isActive && Time.timeSinceLevelLoad > _nextAttack && _isDead == false)
            {
                BigCombat();
            }
        }
    }

    private void TwinBehaviour()
    {
        
    }

    private void RobotBehaviour()
    {

    }


    private void Move()
    {
        //deplacement vers le joueur, le sprite flip sur X en fonction de la direction du mouvement
        _direction2Player = _playerTransform.position - transform.position;
        _animator.SetFloat("DirX", _direction2Player.x);
        _animator.SetBool("Idle", false);
        Vector2 _offset = GetComponentInChildren<CircleCollider2D>().offset;
        if (_direction2Player.x > 0)
        {
            GetComponentInChildren<SpriteRenderer>().flipX = false;
            if (_offset.x<0) GetComponentInChildren<CircleCollider2D>().offset = new Vector2(0.3f, _offset.y);
        }
        else if (_direction2Player.x < 0)
        {
            GetComponentInChildren<SpriteRenderer>().flipX = true;
            if (_offset.x > 0) GetComponentInChildren<CircleCollider2D>().offset = new Vector2(-0.3f, _offset.y);
        }
        _rb.velocity = _direction2Player.normalized * _moveSpeed * Time.fixedDeltaTime;
    }

    private void MoveAway()
    {
        if(!_flipX) { _flipX = true; }
        //deplacement vers le joueur, le sprite flip sur X en fonction de la direction du mouvement
        _direction2Player = _playerTransform.position - transform.position;
        _direction2Player.x = -_direction2Player.x;
        _animator.SetFloat("DirX", _direction2Player.x);
        _animator.SetBool("Idle", false);
        Vector2 _offset = GetComponentInChildren<CircleCollider2D>().offset;
        if (_direction2Player.x > 0)
        {
            GetComponentInChildren<SpriteRenderer>().flipX = false;
            if (_offset.x < 0) GetComponentInChildren<CircleCollider2D>().offset = new Vector2(0.3f, _offset.y);
        }
        else if (_direction2Player.x < 0)
        {
            GetComponentInChildren<SpriteRenderer>().flipX = true;
            if (_offset.x > 0) GetComponentInChildren<CircleCollider2D>().offset = new Vector2(-0.3f, _offset.y);
        }
        _rb.velocity = _direction2Player.normalized * _moveSpeed * Time.fixedDeltaTime;
    }

    private void Stop()
    {
        if(_flipX) { _flipX = false; GetComponentInChildren<SpriteRenderer>().flipX = !GetComponentInChildren<SpriteRenderer>().flipX; }
        //s'assurer que le NPC regarde le joueur (flip X une fois quand il move away)
        _animator.SetBool("Idle", true);
        _rb.velocity = UnityEngine.Vector2.zero;
    }

    private void Combat()
    {
        _animator.SetBool("Attack",true);
        //condition pour savoir si on est en range de toucher (le collider d'attaque qui touche le body du player)
        if (GetComponentInChildren<Damage>()._isOnRange==true) DealDmg();
        _nextAttack = Time.timeSinceLevelLoad + _attackCD;
        StartCoroutine(AttCD());
    }

    private void BigCombat()
    {
        //alterne attaque normale et attaque spéciale
        if (pattern % 2 ==0)
        {
            //attaque normale
            _animator.SetBool("Attack", true);
            //condition pour savoir si on est en range de toucher (le collider d'attaque qui touche le body du player)
            if (GetComponentInChildren<Damage>()._isOnRange == true) DealDmg();
            _nextAttack = Time.timeSinceLevelLoad + _attackCD;
            StartCoroutine(AttCD());
        }
        if (pattern % 2==1)
        {
            //attaque spéciale
            _animator.SetBool("AttackSpe", true);
            if (GetComponentInChildren<SmashDamage>()._isOnRange == true) DealDmgSpe();
            _nextAttack = Time.timeSinceLevelLoad + _attackCD;
            StartCoroutine(AttCD());
        }
        pattern++;

    }

    private void DealDmg()
    {
        _playerHealth.value -= _attPower;
    }

    private void DealDmgSpe()
    {
        _playerHealth.value -= _attSpePower;
    }

    //l'ennemi va clignoter un peu avant de mourrir
    IEnumerator Death()
    {
        _animator.SetBool("Death", true);
        _rb.velocity = UnityEngine.Vector2.zero;
        yield return new WaitForSeconds(1);
        for (int i = 0; i<8;i++)
        {
            yield return new WaitForSeconds(0.15f);
            GetComponentInChildren<SpriteRenderer>().enabled= false;
            yield return new WaitForSeconds(0.15f);
            GetComponentInChildren<SpriteRenderer>().enabled= true;
        }
        _enemyCount.value--;
        _currentActiveGrunt.value--;
        _gameManager.GetComponent<GameManager>()._activeGruntList.Remove(gameObject);
        Instantiate(_prefabPoofingFX, transform.position , Quaternion.identity);
        Instantiate(_prefabVynil, transform.position, Quaternion.identity);
        Destroy(gameObject);
        yield return null;
    }

    IEnumerator AttCD()
    {
        yield return new WaitForSeconds(0.05f);
        _animator.SetBool("Attack", false);
        _animator.SetBool("AttackSpe", false);
    }

    public void KnockBack()
    {
        _animator.SetTrigger("GetsHit");
    }


}
