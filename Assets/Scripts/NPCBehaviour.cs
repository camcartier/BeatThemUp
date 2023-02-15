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
    private Vector2 _direction2Player, _direction2Target; //vector2 to have the direction towards the player
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
    [SerializeField] private Vector2 _targetPos;
    private Vector2 _previousPlayerPos;
    [SerializeField] private bool _toTarget;

    #region KB Controls
    private float _kbTimerCounter = 0;
    [SerializeField] private float _kbDuration = 0.01f;
    [SerializeField] AnimationCurve _kbCurveY;
    float tempposY;
    bool _hasPos, _isKB;
    #endregion

    private void Awake()
    {
        _rb= GetComponent<Rigidbody2D>();
        _player = GameObject.FindGameObjectWithTag("PlayerBody");
        _playerTransform= _player.GetComponent<Transform>();
        _isActive= false;
        _isDead = _isKB = false;
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
                    Instantiate(_prefabPopingFX, new Vector3(transform.position.x, transform.position.y + 0.4f, transform.position.z), Quaternion.identity);
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
        if (_playerTransform.position.x != _previousPlayerPos.x || _playerTransform.position.y != _previousPlayerPos.y) _toTarget = false;
        if (!_toTarget)
        {
            CheckPlayerPos();
        }
        if (Vector2.Distance(transform.position, _targetPos) > 0.05f) Move();
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
        //si le joueur a bougé, changer l'état de _totarget
        if (_playerTransform.position.x != _previousPlayerPos.x || _playerTransform.position.y != _previousPlayerPos.y) _toTarget = false;
        if (!_toTarget)
        {
            CheckPlayerPos();
        }
        //if (Vector2.Distance(transform.position, _playerTransform.position) > _moveDistance) Move();
        if (Vector2.Distance(transform.position, _targetPos) > 0.05f && _animator.GetBool("AttackSpe")==false) Move();
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
        if (_playerTransform.position.x != _previousPlayerPos.x || _playerTransform.position.y != _previousPlayerPos.y) _toTarget = false;
        if (!_toTarget)
        {
            CheckPlayerPos();
        }
        if (_jumpAtt) { JumpAttack(); }
        else if (Vector2.Distance(transform.position, _targetPos) > 0.05f) Move();
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
        if (_playerTransform.position.x != _previousPlayerPos.x || _playerTransform.position.y != _previousPlayerPos.y) _toTarget = false;
        if (!_toTarget)
        {
            CheckPlayerPos();
        }
        //if (Vector2.Distance(transform.position, _playerTransform.position) > _moveDistance) Move();
        if (Vector2.Distance(transform.position, _targetPos) > 0.05f) MoveRobot();
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
        _direction2Target = new Vector2(_targetPos.x - transform.position.x, _targetPos.y - transform.position.y);
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
        _rb.velocity = _direction2Target.normalized * _moveSpeed * Time.fixedDeltaTime;
    }

    private void MoveRobot()
    {
        //deplacement vers le joueur, le sprite flip sur X en fonction de la direction du mouvement
        _direction2Player = _playerTransform.position - transform.position;
        _direction2Target = new Vector2(_targetPos.x - transform.position.x, _targetPos.y - transform.position.y);
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
        _rb.velocity = _direction2Target.normalized * _moveSpeed * Time.fixedDeltaTime;
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
        if (pattern % 2 ==1)
        {
            Debug.Log("bourrepif");
            Debug.Log(Time.timeSinceLevelLoad);
            //attaque normale
            _animator.SetBool("Attack", true);
            //condition pour savoir si on est en range de toucher (le collider d'attaque qui touche le body du player)
            if (GetComponentInChildren<Damage>()._isOnRange == true) DealDmg();
            _nextAttack = Time.timeSinceLevelLoad + _attackCD;
            StartCoroutine(AttCD());
        }
        else
        {
            //Debug.Log("début smash");
            //Debug.Log(Time.timeSinceLevelLoad);
            //attaque spéciale
            _animator.SetBool("AttackSpe", true);
            _nextAttack = Time.timeSinceLevelLoad + _attackCD;
            //if (GetComponentInChildren<SmashDamage>()._isOnRange == true) DealDmgSpe();
            //_nextAttack = Time.timeSinceLevelLoad + _attackCD;
            //StartCoroutine(AttCD());
            //coroutine pour l'attaque spé qui découpe l'action en 2
            StartCoroutine(SmashAttack());
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
        Debug.Log("Bim dégâts appliqué");
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
        Instantiate(_prefabPoofingFX, transform.position, Quaternion.identity);
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
            if(transform.position.x -_player.transform.position.x > 0f)
            {
                transform.position = new Vector2(Mathf.Lerp(transform.position.x, transform.position.x + 0.05f, _kbDuration), tempposY + 2 * y);
            }
            else
            {
                transform.position = new Vector2(Mathf.Lerp(transform.position.x, transform.position.x - 0.05f, _kbDuration), tempposY + 2 * y);
            }

        }
        else
        {
            _kbTimerCounter = 0;
            _isKB = false;
            _hasPos = false;
            _popingFXexist = false;
        }
    }



    private void CheckPlayerPos()
    {
        if (transform.position.x < _playerTransform.position.x)
        {
            _targetPos = new Vector2(_playerTransform.position.x - _moveDistance, _playerTransform.position.y);
            _toTarget = true;
            _previousPlayerPos = _playerTransform.position;
        }
        else
        {
            _targetPos = new Vector2(_playerTransform.position.x + _moveDistance, _playerTransform.position.y);
            _toTarget = true;
            _previousPlayerPos = _playerTransform.position;
        }
    }

    IEnumerator SmashAttack()
    {
        //première partie de l'animation de lever des mains
        //Debug.Log("début coroutine");
        //Debug.Log(Time.timeSinceLevelLoad);
        yield return new WaitForSeconds(1.5f);
        //Debug.Log("activation du smash");
        //Debug.Log(Time.timeSinceLevelLoad);
        _nextAttack = Time.timeSinceLevelLoad + _attackCD;
        //deuxième partie de l'animation
        _animator.SetBool("Smash", true);
        yield return new WaitForSeconds(0.5f);
        //Debug.Log("dégâts du smash");
        //Debug.Log(Time.timeSinceLevelLoad);
        //check si le joueur est dans la zone de dégâts pour application des dégâts
        if (GetComponentInChildren<SmashDamage>()._isOnRange == true && _isDead==false) DealDmgSpe();
        _nextAttack = Time.timeSinceLevelLoad + _attackCD;
        _animator.SetBool("Smash", false);
        _animator.SetBool("AttackSpe", false);
        //Debug.Log("fin du smash");
        //Debug.Log(Time.timeSinceLevelLoad);
    }
}



