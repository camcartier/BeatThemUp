using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using Unity.VisualScripting;
using TMPro;

public class PlayerMovement : MonoBehaviour
{
    private GameObject _player;
    private GameObject _throwPos;
    private Rigidbody2D _rb;
    private Animator _animator;
    [SerializeField] GameObject _continueMenu;
    private GameObject _gameManager;

    #region sounds
    AudioSource _playerpunch;
    AudioSource _gruntpunch;
    AudioSource _playerdeath;
    AudioSource _jumpsound;

    #endregion

    [SerializeField] private InputActionReference movement, attack, jump, use, sprint, attackspe;
    [SerializeField] public IntVariables _playerHealth;
    [SerializeField] public IntVariables _playerMana;
    [SerializeField] private int _lives;
    private float _storedHealth;
    private float _storedMana;
    public int PlayerAttPower,PlayerSuperAttPower;

    private Vector2 _move;
    private float _movespeed = 60;
    private float _runspeed = 140;
    private bool _isWalking;
    //valeur recuperee (0>>1) lorsque le bouton est active
    public bool _isJumping;
    //_jumpTime est le delai avant de ressauter
    [SerializeField] private float _jumpDuration = 1 ;
    [SerializeField] private float _jumpTimerCounter;
    [SerializeField] AnimationCurve _jumpCurve;
    [SerializeField] private float _jumpHeight;
    private Vector2 _initialPos;

    //private Collider2D _fistCollider;

    //valeur recuperee (0>>1) lorsque le bouton est active
    public bool _isAttacking;
    private bool _isRunning;
    private bool _isUsing;
    private float _useFloat;
    private bool _AttackSpe;


    [SerializeField] private GameObject _throwableCanPrefab;
    [SerializeField] private GameObject _thrownCanPrefab;
    private GameObject _objectThrown;
    private GameObject _readyToThrow;
    bool _canThrow;
    bool _hasCan;
    bool _canPickUp;

    #region Health&Mana Bars
    [Header("HealthBar")]
    [SerializeField] Image _healthbarGreen;
    [SerializeField] Image _healthbarRed;
    private HealthBar _healthbar;
    [Header("ManaBar")]
    [SerializeField] Image _manabarBlue;
    [SerializeField] Image _manabarGrey;
    private ManaBar _manabar;
#endregion

    private bool _isDead;//bool qui s'active quand le joueur meurt la première fois
    [SerializeField] private float _jumpCD; //cooldown entre 2 jumps
    private float _nextJump; //timer avant le prochain saut
    private float _jumpY;
    [SerializeField] private bool _jumpyjump;
    private bool _isInvulnerable;

    #region VFX
    [SerializeField] private GameObject _prefabSprintingFX;
    private bool _sprintingFXexist;
    [SerializeField] private GameObject _prefabJumpingFX;
    private bool _jumpingFXexist;
    [SerializeField] private GameObject _prefabLandingFX;
    private bool _landingFXexist;
    #endregion


    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _animator = GetComponentInChildren<Animator>();
        _player = GameObject.Find("Player");
        _throwPos = GameObject.Find("throwPos");

        _playerpunch = GameObject.Find("PlayerPunch").GetComponent<AudioSource>();
        _gruntpunch = GameObject.Find("GruntPunch").GetComponent<AudioSource>();
        _playerdeath = GameObject.Find("DeathSound").GetComponent<AudioSource>();
        _jumpsound = GameObject.Find("JumpSound").GetComponent<AudioSource>();

        _jumpTimerCounter = 0;
        _storedHealth = 100;
        _storedMana = 0;
        _isDead = _isInvulnerable = false;
        //_fistCollider = FindGameObjectWithTag("PlayerFist").Collider2D;
        //_fistCollider.enabled = false;

        _healthbar = GameObject.Find("HealthBar").GetComponent<HealthBar>();
        _manabar = GameObject.Find("ManaBar").GetComponent<ManaBar>();
        _jumpyjump = false;
        _gameManager = GameObject.FindGameObjectWithTag("GameManager");
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        GetInput();

        if (_storedHealth != _playerHealth.value && !_isDead)
        {
            _storedHealth = _playerHealth.value;
            _healthbar.SetHealth(_playerHealth.value);
            KnockBack();
        }

        if (_storedMana != _playerMana.value && !_isDead)
        { 
            _manabar.SetMana(_playerMana.value);
            _storedMana = _playerMana.value;
        }

        if (_playerHealth.value <= 0 && _isDead == false)
        {
            if (_lives == 0)
            {
                _isDead = true;
                Death();
                //activer le menu gameover
            }
            else if (_lives > 0)
            {
                _isDead = true;
                Death();
                _continueMenu.SetActive(true);
            }
            
        }
    }

    private void FixedUpdate()
    {
        if (!_isDead) Move();

    }


    private void GetInput()
    {
        _move = movement.action.ReadValue<Vector2>();

        _isWalking = movement.action.ReadValue<Vector2>() != new Vector2 (0,0);

        if (!_isJumping) _isJumping = jump.action.ReadValue<float>() > 0.1;

        _isAttacking = attack.action.ReadValue<float>() > 0.1;

        _useFloat = use.action.ReadValue<float>();

        _isRunning = sprint.action.ReadValue<float>() > 0.1;

        _AttackSpe = attackspe.action.ReadValue<float>() > 0.1;


        //prbs de tp
        //_isJumping = jump.action.triggered;
        //if(!_isUsing) _isUsing = use.action.ReadValue<float>() > 0.1;
    }

    private void Move()
    {

        _rb.velocity = _move * _movespeed * Time.fixedDeltaTime ;
        Vector2 _offset = GetComponentInChildren<CircleCollider2D>().offset;
        _animator.SetBool("Grounded", true);

        if (_move.x < 0 )
        {
            GetComponentInChildren<SpriteRenderer>().flipX= true;
            if (_offset.x > 0) GetComponentInChildren<CircleCollider2D>().offset = new Vector2(-0.3f, _offset.y);


        }
        else if (_move.x > 0)
        {
            GetComponentInChildren<SpriteRenderer>().flipX = false;
            if (_offset.x < 0) GetComponentInChildren<CircleCollider2D>().offset = new Vector2(0.3f, _offset.y);

        }

        if (_isWalking) { _animator.SetBool("Walking", true); }
        else { _animator.SetBool("Walking", false); }


        if (_isJumping && !_jumpyjump)
        {
            Instantiate(_prefabJumpingFX, transform.position, Quaternion.identity);
            _animator.SetBool("Jumping", true);
            _animator.SetBool("Walking", false);
            _animator.SetBool("Grounded", false);
            _jumpyjump = true;
            _jumpY = transform.position.y;
            _jumpsound.Play();
        }
        if (_jumpyjump)
        {
            DoJump3();
        }


        if (_isAttacking && !_isJumping)
        {
            //_fistCollider.enabled = true;
            _movespeed = 0f;
            _runspeed = 0f;
            _animator.SetBool("Attacking", true);
            
            if (!_playerpunch.isPlaying)
            {
                _playerpunch.Play();
            }

            if (_isUsing)
            {
                _animator.SetTrigger("Throws");
            }
        }
        else
        {
            _movespeed = 60f;
            _runspeed = 140f;
            _animator.SetBool("Attacking", false);
            //_fistCollider.enabled = false;
        }

        if (_isRunning)
        {
            _rb.velocity = _move * _runspeed * Time.fixedDeltaTime;
            _animator.SetBool("Running", true);
            if (!_sprintingFXexist && _move.x !=0 && !_isJumping)
            {
                Instantiate(_prefabSprintingFX, transform.position, Quaternion.identity);
                _sprintingFXexist = true;
                if (_move.x < 0)
                {
                    _prefabSprintingFX.GetComponent<SpriteRenderer>().flipX =false;
                }
                else
                {
                    _prefabSprintingFX.GetComponent<SpriteRenderer>().flipX = true;
                }
            }
            if (!_isJumping && Mathf.Abs(_move.x) <0.1)
            {
                _animator.SetBool("Running", false);
            }
            
        }
        else
        {
            _rb.velocity = _move * _movespeed * Time.fixedDeltaTime;
            _animator.SetBool("Running", false);
            _sprintingFXexist = false;
        }


        if (_useFloat > 0 && _canPickUp)
        {
            _animator.SetBool("HasObject", true);
            if (!_hasCan) { PicksUp(); _hasCan = true; }
        }
        else
        {
            _animator.SetBool("HasObject", false);
            if (_canThrow)
            {
                Throw();
            }

        }

        if (_AttackSpe  && _playerMana.value >=100)
        {
            _isInvulnerable= true;
            _animator.SetBool("AttackSpe", true);
            _playerMana.value = 0;
            SpecialAttack();
            StartCoroutine(InvulnerabilityNoBlink());
        }
        else { _animator.SetBool("AttackSpe", false); }

    }




    IEnumerator doJump2()
    {
        transform.position = new Vector2(transform.position.x, Mathf.Lerp(transform.position.y, transform.position.y + 0.2f, 0.3f));
        yield return new WaitForSeconds(.3f);
        _animator.SetBool("Jumping", false);
        transform.position = new Vector2(transform.position.x, Mathf.Lerp(transform.position.y, transform.position.y - 0.2f, 0.3f));
        _animator.SetBool("Grounded", true);
        
        _isJumping= false;
    }
    
    void DoJump3()
    {
        if (_jumpTimerCounter<_jumpDuration)
        {
            _jumpTimerCounter += Time.fixedDeltaTime;
            float y = _jumpCurve.Evaluate(_jumpTimerCounter/_jumpDuration);
            transform.position = new Vector2(transform.position.x, _jumpY + 1f* y);
            if (_jumpTimerCounter>_jumpDuration*0.75f) _animator.SetBool("Jumping", false);
        }
        else
        {
            _jumpTimerCounter= 0;
            _isJumping = false;
            _jumpyjump= false;
            _animator.SetBool("Grounded", true);
            if (!_landingFXexist)
            {
                Instantiate(_prefabLandingFX, transform.position, Quaternion.identity);
                _landingFXexist= true;
            }

        }

        _landingFXexist = false;
        //verifier sii on est pas a la fin dusaut)
        //demarrer timer -(0 au debut du saut)
        //regarde la courbe en fonction du timer (evaluate parametrex temps )
        //augmente le timer 

        //
    }


    public void TakesHit()
    {
        _animator.SetTrigger("GetsHit");
        _gruntpunch.Play();
    }

    public void Death()
    {
        _animator.SetBool("Dead", true);
        _playerdeath.Play();
    }


    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Throwable"))
        {
            _canPickUp = true;
            //Debug.Log("canpickup");
        }
        if (_hasCan)
        {
            Destroy(collision.gameObject);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Throwable"))
        {
            _canPickUp = false;
        }
    }


    public void PicksUp()
    {

        GameObject _pickupPos = _player.transform.Find("pickupPos").gameObject;
        if (!_hasCan && _canPickUp)
        {
            _readyToThrow = Instantiate(_throwableCanPrefab, _pickupPos.transform) as GameObject;
            _readyToThrow.transform.parent = GameObject.Find("Player").transform;
            //_hasCan = true;
            _canPickUp= false;
            _canThrow = true;
            
        }

    }

    
    
    public void Throw()
    {
        Destroy(_readyToThrow);
        GameObject _pickupPos = _player.transform.Find("pickupPos").gameObject;
        Instantiate(_thrownCanPrefab, _pickupPos.transform);
        _hasCan= false;
        _canThrow= false;
    }


    public void KnockBack()
    {
        _animator.SetTrigger("GetsHit");
        /*
        float force = 1000;
        _rb.AddForce(transform.right *force);
        Debug.Log("aouch");*/
    }

    public void Respawn()
    {
        _isInvulnerable = true;
        _continueMenu.SetActive(false);
        _animator.SetBool("Dead", false);
        _animator.SetBool("Walking", true);
        _lives--;
        _playerHealth.value = 100;
        _isDead = false;
        StartCoroutine(Invulnerability());
    }

    IEnumerator Invulnerability()
    {
        for (int i = 0; i < 5; i++)
        {
            yield return new WaitForSeconds(0.1f);
            gameObject.GetComponentInChildren<SpriteRenderer>().enabled = false;
            yield return new WaitForSeconds(0.1f);
            gameObject.GetComponentInChildren<SpriteRenderer>().enabled = true;
        }
        _isInvulnerable= false;
    }

    IEnumerator InvulnerabilityNoBlink()
    {
        yield return new WaitForSeconds(3f);
        _isInvulnerable = false;
    }

    private void SpecialAttack()
    {
        foreach (var item in _gameManager.GetComponent<GameManager>()._inactiveGruntList)
        {
            item.GetComponent<NPCBehaviour>()._hp -= PlayerSuperAttPower;
        }
        foreach (var item in _gameManager.GetComponent<GameManager>()._inactiveGruntList)
        {
            item.GetComponent<NPCBehaviour>()._hp -= PlayerSuperAttPower;
        }
    }
}
