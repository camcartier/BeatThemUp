using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    private bool _isDead, _jumpAtt;
    private bool _flipX;
    [SerializeField] GameObject _prefabVynil;
    [SerializeField] GameObject _prefabPoofingFX;
    [SerializeField] private GameObject _prefabPopingFX;
    private bool _popingFXexist;
    private GameObject _gameManager;
    [SerializeField] public IntVariables _playerMana; 
    [SerializeField] private int _manaGainValue = 10;

    [SerializeField] private bool grunt, biggrunt, twin, robotnik;
    private int pattern;
    [SerializeField] AnimationCurve _jumpCurve;
    [SerializeField] private float _jumpDuration = 1;
    [SerializeField] private float _jumpTimerCounter;
    [SerializeField] private float _jumpHeight;
    [SerializeField] private float _jumpY;

    private CircleCollider2D[] _robotColliders;
    private Vector2 _targetPos;
    private bool _toTarget;

    #region KB Controls
    private float _kbTimerCounter = 0;
    [SerializeField] private float _kbDuration = 0.01f;
    [SerializeField] AnimationCurve _kbCurveY;
    float tempposY;
    bool _hasPos, _isKB;
    float tempTimer = 0f;
    #endregion

    private void Awake()
    {
        _rb= GetComponent<Rigidbody2D>();
        _player = GameObject.FindGameObjectWithTag("PlayerBody");
        _playerTransform= _player.GetComponent<Transform>();
        _isActive= false;
        _isDead = _isKB = false;
        _jumpAtt = false;

        _isDead= false;
        _jumpAtt = _toTarget = false;
        _lastHP = _hp;
        _gameManager = GameObject.FindGameObjectWithTag("GameManager");
        pattern = 1;
        _jumpTimerCounter = 0;
        if(robotnik) _robotColliders = GetComponentsInChildren<CircleCollider2D>();
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
            _animator.SetTrigger("GetsHit");

            _lastHP = _hp;
            if (!_hasPos)
            {
                tempposY = transform.position.y;
                _hasPos = true;
                _isKB = true;

                if (!_popingFXexist)
                {
                    Instantiate(_prefabPopingFX, new Vector3(transform.position.x, transform.position.y + 0.001f, transform.position.z), Quaternion.identity);
                    _popingFXexist = true;
                }
            }

            if (_playerMana.value < 100) { _playerMana.value += _manaGainValue; } else { _playerMana.value = 100; }

            _nextAttack = Time.timeSinceLevelLoad + _attackCD;
        }

        if (_isKB)
        {
            KnockBack();
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
            if (biggrunt) InactiveBigBehaviour();
            else InactiveBehaviour();
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
        if (!_toTarget)
        {
            if (_playerTransform.position.x > transform.position.x)
            {
                _targetPos = new Vector2(_playerTransform.position.x - _moveDistance, _playerTransform.position.y);
            }
            else
            {
                _targetPos = new Vector2 (_playerTransform.position.x + _moveDistance, _playerTransform.position.y);
            }
        }
        //if (Vector2.Distance(transform.position, _playerTransform.position) > _moveDistance) Move();
        if (Vector2.Distance(transform.position, _targetPos) > 0.05f) Move();
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
        if (_jumpAtt) { JumpAttack(); }
        else if (Vector2.Distance(transform.position, _playerTransform.position) > _moveDistance) Move();
        else
        {
            Stop();
            if (_isActive && Time.timeSinceLevelLoad > _nextAttack && _isDead == false)
            {
                TwinCombat();
            }
        }
    }

    private void RobotBehaviour()
    {
        if (Vector2.Distance(transform.position, _playerTransform.position) > _moveDistance) MoveRobot();
        else
        {
            Stop();
            if (_isActive && Time.timeSinceLevelLoad > _nextAttack && _isDead == false)
            {
                RobotCombat();
            }
        }
    }


    private void Move()
    {
        float x = 0;
        if (grunt || twin) x = 0.3f;
        else if (biggrunt) x = 0.45f;
        //deplacement vers le joueur, le sprite flip sur X en fonction de la direction du mouvement
        _direction2Player = _playerTransform.position - transform.position;
        _animator.SetFloat("DirX", _direction2Player.x);
        _animator.SetBool("Idle", false);
        Vector2 _offset = GetComponentInChildren<CircleCollider2D>().offset;
        if (_direction2Player.x > 0)
        {
            GetComponentInChildren<SpriteRenderer>().flipX = false;
            if (_offset.x<0) GetComponentInChildren<CircleCollider2D>().offset = new Vector2(x, _offset.y);
        }
        else if (_direction2Player.x < 0)
        {
            GetComponentInChildren<SpriteRenderer>().flipX = true;
            if (_offset.x > 0) GetComponentInChildren<CircleCollider2D>().offset = new Vector2(-x, _offset.y);
        }
        _rb.velocity = _direction2Player.normalized * _moveSpeed * Time.fixedDeltaTime;
    }

    private void MoveRobot()
    {
        //deplacement vers le joueur, le sprite flip sur X en fonction de la direction du mouvement
        _direction2Player = _playerTransform.position - transform.position;
        _animator.SetFloat("DirX", _direction2Player.x);
        _animator.SetBool("Idle", false);
        foreach (var item in _robotColliders)
        {
            Vector2 _offset = item.offset;
            if (_direction2Player.x > 0)
            {
                GetComponentInChildren<SpriteRenderer>().flipX = false;
                if (_offset.x > 0) item.offset = new Vector2(1.5f, _offset.y);
            }
            else if (_direction2Player.x < 0)
            {
                GetComponentInChildren<SpriteRenderer>().flipX = true;
                if (_offset.x < 0) item.offset = new Vector2(-1.5f, _offset.y);
            }
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
        _toTarget = false;
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

    private void TwinCombat()
    {
        //alterne 2 attaques normales et une attaque spéciale
        if (pattern % 3 == 0)
        {
            //attaque spéciale
            _jumpY = transform.position.y;
            _jumpAtt = true;
            _nextAttack = Time.timeSinceLevelLoad + _attackCD +1f;
        }
        else
        {
            //attaque normale
            _animator.SetBool("Attack", true);
            //condition pour savoir si on est en range de toucher (le collider d'attaque qui touche le body du player)
            if (GetComponentInChildren<Damage>()._isOnRange == true) DealDmg();
            _nextAttack = Time.timeSinceLevelLoad + _attackCD;
            StartCoroutine(AttCD());
        }
        pattern++;
    }


    private void RobotCombat()
    {
        //alterne 2 attaques normales et une attaque spéciale
        if (pattern % 3 == 0)
        {
            //attaque spéciale
            _animator.SetBool("AttackSpe", true);
            if (GetComponentInChildren<SmashDamage>()._isOnRange == true) DealDmgSpe();
            _nextAttack = Time.timeSinceLevelLoad + _attackCD;
            StartCoroutine(AttCD());
        }
        else
        {
            //attaque normale
            _animator.SetBool("Attack", true);
            //condition pour savoir si on est en range de toucher (le collider d'attaque qui touche le body du player)
            if (GetComponentInChildren<Damage>()._isOnRange == true) DealDmg();
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

    //attaqué sautée du twin
    private void JumpAttack()
    {
        //première partie du jump et condition pour activer la second partie avec l'animation du poing
        if (_jumpTimerCounter < _jumpDuration)
        {
            if (_animator.GetBool("Jump")==false) _animator.SetBool("Jump", true);
            _jumpTimerCounter += Time.fixedDeltaTime;
            float y = _jumpCurve.Evaluate(_jumpTimerCounter / _jumpDuration);
            transform.position = new Vector2(transform.position.x, _jumpY + 1f * y);
            if (_jumpTimerCounter > _jumpDuration * 0.66f && _animator.GetBool("AttackSpe")==false) {_animator.SetBool("AttackSpe", true); }
        }

        //application des dégâts à la retombée
        else
        {
            _jumpTimerCounter = 0;
            _jumpAtt = false;
            _animator.SetBool("Jump", false);
            if (GetComponentInChildren<SmashDamage>()._isOnRange == true) DealDmgSpe();
            _nextAttack = Time.timeSinceLevelLoad + _attackCD;
            StartCoroutine(AttCD());
        }
    }

    IEnumerator AttCD()
    {
        yield return new WaitForSeconds(0.05f);
        _animator.SetBool("Attack", false);
        _animator.SetBool("AttackSpe", false);
    }

    public void KnockBack()
    {
        if (_kbTimerCounter < _kbDuration)
        {
            _kbTimerCounter += Time.deltaTime;
            float y = _kbCurveY.Evaluate(_kbTimerCounter / _kbDuration);
            transform.position = new Vector2(Mathf.Lerp(transform.position.x, transform.position.x + 0.1f, _kbDuration), tempposY + 2 * y);

        }
        else
        {
            _kbTimerCounter = 0;
            _isKB = false;
            _hasPos = false;
            _popingFXexist = false;
        }
    }




   /*
        void DoJump3()
    {
        if (_jumpTimerCounter < _jumpDuration)
        {
            _jumpTimerCounter += Time.fixedDeltaTime;
            float y = _jumpCurve.Evaluate(_jumpTimerCounter / _jumpDuration);
            transform.position = new Vector2(transform.position.x, _jumpY + 1f * y);
            if (_jumpTimerCounter > _jumpDuration * 0.75f) _animator.SetBool("Jumping", false);
        }
        else
        {
            _jumpTimerCounter = 0;
            _isJumping = false;
            _jumpyjump = false;
            _animator.SetBool("Grounded", true);
            if (!_landingFXexist)
            {
                Instantiate(_prefabLandingFX, transform.position, Quaternion.identity);
                _landingFXexist = true;
            }

        }

        _landingFXexist = false;
        //verifier sii on est pas a la fin dusaut)
        //demarrer timer -(0 au debut du saut)
        //regarde la courbe en fonction du timer (evaluate parametrex temps )
        //augmente le timer 

        //
    }*/


}



