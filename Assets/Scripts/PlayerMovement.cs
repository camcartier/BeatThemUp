using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

enum PlayerStateMode
{
    IDLE,
    WALK,
    JUMP,
    ATTACK
}

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D _rb;
    [SerializeField] private InputActionReference movement, attack, jump, use, sprint;
    private GameObject _player;
    [SerializeField] private GameObject _throwableCanPrefab;

    private Vector2 _move;
    private Vector2 _notmoving = new Vector2(0, 0);
    private float _movespeed = 60;
    private bool _isWalking;
    private float _runspeed = 120;
    

    private float _lastXposition;
    private Vector2 _currentPosition;
    private Vector2 _direction;


    //valeur recuperee (0>>1) lorsque le bouton est active
    private bool _isJumping;
    //commence a true pour sauter x1
    private bool _canJump = true;
    //_jumpTime est le delai avant de ressauter
    [SerializeField] private float _jumpTime;
    private float _jumpTimerCounter;
    private Vector2 _initialPos;


    //valeur recuperee (0>>1) lorsque le bouton est active
    private bool _isAttacking;
    private bool _isRunning;
    private bool _isUsing;
    private float _useFloat;

    private bool _readyToThrow = true;
    bool _hasCan;
    bool _canPickUp;


    //gestion de l'animator
    private Animator _animator;




    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _animator = GetComponentInChildren<Animator>();
        _player = GameObject.Find("Player");

        _jumpTimerCounter = _jumpTime;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        GetInput();

    }

    private void FixedUpdate()
    {
        Move();


    }

    //prbs de tp
    //_isJumping = jump.action.triggered;
    //_isAttacking= attack.action.triggered;

    private void GetInput()
    {
        _move = movement.action.ReadValue<Vector2>();
        
        _isWalking = movement.action.ReadValue<Vector2>() != new Vector2 (0,0);
        

        if (!_isJumping) _isJumping = jump.action.ReadValue<float>() > 0.1;
        
        _isAttacking = attack.action.ReadValue<float>() > 0.1;

        //if(!_isUsing) _isUsing = use.action.ReadValue<float>() > 0.1;
        _useFloat = use.action.ReadValue<float>();
        //Debug.Log($"{_useFloat}");

        _isRunning = sprint.action.ReadValue<float>() > 0.1;
        
    }

    private void Move()
    {

        _rb.velocity = _move * _movespeed * Time.fixedDeltaTime ;
        _animator.SetBool("Grounded", true);

        #region toupie
        //toupie
        /*
        if (_lastXposition - _currentPosition > 0)
        {
            transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
            _lastXposition = _currentPosition;
        }
        
        else
        {
            _lastXposition = _currentPosition;
        }*/

        #endregion


        if (_move.x < 0 )
        {
            GetComponentInChildren<SpriteRenderer>().flipX= true;
        }
        if (_move.x > 0)
        {
            GetComponentInChildren<SpriteRenderer>().flipX = false;
        }

        if (_isWalking)
        {
            //vers l'infini et au dela
            _animator.SetBool("Walking", true); 
        }
        else
        {
            _animator.SetBool("Walking", false);
        }

        if (_isJumping && _canJump)
        {
            /* on ne l'utilse pas car on doit tricher le mvt vers le bas
            _rb.AddForce(transform.up * _jumpForce, ForceMode2D.Impulse);
            _rb.AddForce(transform.up * -_jumpForce, ForceMode2D.Impulse);*/

            _animator.SetBool("Jumping", true);
            _animator.SetBool("Walking", false);
            _animator.SetBool("Grounded", false);

            StartCoroutine(doJump2());

        }

        if (_isAttacking)
        {
            //_animator.SetTrigger("AttackTrigger");
            _animator.SetBool("Attacking", true);
            if(_isUsing)
            {
                _animator.SetTrigger("Throws");
            }
        }
        else
        {
            _animator.SetBool("Attacking", false);
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
            //Debug.Log("gets");
            _animator.SetBool("HasObject", true);
            PicksUp();
        }
        else
        {
            //Debug.Log("getsnot");
            _animator.SetBool("HasObject", false);
            Throw ();
        }


        /*
        if (_isUsing && _isAttacking)
        {
            Throw();
        }*/

    }

    /*
    IEnumerator doJump()
    {
        _rb.AddForce(transform.up * _jumpForce, ForceMode2D.Impulse);
        yield return new WaitForSeconds(.1f);
        _animator.SetBool("Jumping", false);
    }*/


    //prbs avec la coroutine
    IEnumerator doJump2()
    {
        //transform.position = new Vector2(transform.position.x, Mathf.Lerp(transform.position.y, transform.position.y + 0.1f, 0.2f));
        //transform.position = new Vector2(transform.position.x, Mathf.Lerp(transform.position.y, transform.position.y + 0.1f, 0.5f));

        transform.position = new Vector2(transform.position.x, Mathf.Lerp(transform.position.y, transform.position.y + 0.2f, 0.3f));
        yield return new WaitForSeconds(.3f);
        _animator.SetBool("Jumping", false);
        transform.position = new Vector2(transform.position.x, Mathf.Lerp(transform.position.y, transform.position.y - 0.2f, 0.3f));
        _animator.SetBool("Grounded", true);
        
        _isJumping= false;
        //yield return new WaitForSeconds(5);
    }




    #region Testing Debug

    void OnJump()
    {
        //Debug.Log("Jump pressed");

    }

    void OnAttack()
    {
        //Debug.Log("Attack");
    }
    #endregion

    public void Death()
    {
        _animator.SetBool("Dead", true);
    }

    public void TakesHit()
    {
        _animator.SetTrigger("GetsHit");
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
            //Debug.Log("entredansleif");
            GameObject readyToThrow = Instantiate(_throwableCanPrefab, _pickupPos.transform) as GameObject;
            //Debug.Log("instantiated");
            readyToThrow.transform.parent = GameObject.Find("Player").transform;
            _hasCan = true;
            _canPickUp= false;
            
        }

    }

    
    public void DestroyPickup()
    {

    }

    
    public void Throw()
    {
        _readyToThrow = false;
        /*
        if (_hasCan)
        {
            GameObject childThrowable = _player.transform.Find("ThrowableBlueCan(Clone)").gameObject;

            Rigidbody2D _rbchild = childThrowable.GetComponent<Rigidbody2D>();
            _rbchild.velocity = new Vector2(100, 0);
        }*/

    }

}
