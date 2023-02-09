using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using Unity.VisualScripting;

public class PlayerMovement : MonoBehaviour
{
    private GameObject _player;
    private GameObject _throwPos;
    private Rigidbody2D _rb;
    private Animator _animator;
    [SerializeField] private InputActionReference movement, attack, jump, use, sprint;
    [SerializeField] public IntVariables _playerHealth;
    private float _storedHealth;
    public int PlayerAttPower;

    private Vector2 _move;
    private float _movespeed = 60;
    private float _runspeed = 140;
    private bool _isWalking;
    //valeur recuperee (0>>1) lorsque le bouton est active
    private bool _isJumping;
    //commence a true pour sauter x1
    private bool _canJump = true;
    //_jumpTime est le delai avant de ressauter
    [SerializeField] private float _jumpDuration = 1 ;
    private float _jumpTimerCounter;
    [SerializeField] AnimationCurve _jumpCurve;
    [SerializeField] private float _jumpHeight;
    private Vector2 _initialPos;

    //private Collider2D _fistCollider;

    //valeur recuperee (0>>1) lorsque le bouton est active
    public bool _isAttacking;
    private bool _isRunning;
    private bool _isUsing;
    private new float _useFloat;

    //private bool _canDmg; //booléen qui dit si le joueur peut faire des dégâts ou pas, conditionné par le collider des poings du joueur qui touche le body de l'ennemi


    [SerializeField] private GameObject _throwableCanPrefab;
    [SerializeField] private GameObject _thrownCanPrefab;
    private GameObject _objectThrown;
    private GameObject _readyToThrow;
    bool _canThrow;
    bool _hasCan;
    bool _canPickUp;

    [Header("HealthBar")]
    [SerializeField] Image _healthbarGreen;
    [SerializeField] Image _healthbarRed;
    private HealthBar _healthbar;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _animator = GetComponentInChildren<Animator>();
        _player = GameObject.Find("Player");
        _throwPos = GameObject.Find("throwPos");

        _jumpTimerCounter = 0;
        _storedHealth = _playerHealth.value;

        //_fistCollider = FindGameObjectWithTag("PlayerFist").Collider2D;
        //_fistCollider.enabled = false;

        _healthbar = GameObject.Find("HealthBar").GetComponent<HealthBar>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        GetInput();

        if (_storedHealth != _playerHealth.value)
        {
            _storedHealth = _playerHealth.value;
            _healthbar.SetHealth(_playerHealth.value);
            KnockBack();
        }


        if (_playerHealth.value <= 0)
        {
            Death();

        }
    }

    private void FixedUpdate()
    {
        Move();

    }


    private void GetInput()
    {
        _move = movement.action.ReadValue<Vector2>();

        _isWalking = movement.action.ReadValue<Vector2>() != new Vector2 (0,0);

        if (!_isJumping) _isJumping = jump.action.ReadValue<float>() > 0.1;

        _isAttacking = attack.action.ReadValue<float>() > 0.1;

        _useFloat = use.action.ReadValue<float>();

        _isRunning = sprint.action.ReadValue<float>() > 0.1;




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
        if (_move.x > 0)
        {
            GetComponentInChildren<SpriteRenderer>().flipX = false;
            if (_offset.x < 0) GetComponentInChildren<CircleCollider2D>().offset = new Vector2(0.3f, _offset.y);

        }

        if (_isWalking)
        {
            _animator.SetBool("Walking", true); 
        }
        else
        {
            _animator.SetBool("Walking", false);
        }

        if (_isJumping && _canJump)
        {

            _animator.SetBool("Jumping", true);
            _animator.SetBool("Walking", false);
            _animator.SetBool("Grounded", false);
            _jumpTimerCounter= 0;

            StartCoroutine(doJump2());
            //DoJump3();
        }

        if (_isAttacking)
        {
            //_fistCollider.enabled = true;
            _movespeed = 0f;
            _runspeed = 0f;
            _animator.SetBool("Attacking", true);

            if(_isUsing)
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
        }
        else
        {
            _rb.velocity = _move * _movespeed * Time.fixedDeltaTime;
            _animator.SetBool("Running", false);
        }



        if (_useFloat > 0)
        {
            _animator.SetBool("HasObject", true);
            PicksUp();
        }
        else
        {
            _animator.SetBool("HasObject", false);
            if (_canThrow)
            {
                Throw();
            }

        }



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
    /*
    void DoJump3()
    {

        _canJump = false;
        float y = _jumpCurve.Evaluate(_jumpDuration);

        transform.position = new Vector2(transform.position.x, transform.position.y * y);

        //verifier sii on est pas a la fin dusaut)
        //demarrer timer -(0 au debut du saut)
        //regarde la courbe en fonction du timer (evaluate parametrex temps )
        //augmente le timer 

        //
        _isJumping = false;
    }*/


    public void TakesHit()
    {
        _animator.SetTrigger("GetsHit");
    }

    public void Death()
    {
        _animator.SetBool("Dead", true);
    }


    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Throwable"))
        {
            _canPickUp = true;
            Debug.Log("canpickup");
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
            _hasCan = true;
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



}
