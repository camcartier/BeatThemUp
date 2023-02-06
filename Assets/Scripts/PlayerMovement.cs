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
    [SerializeField] private InputActionReference movement, attack, jump;
    

    private Vector2 _move;
    private Vector2 _notmoving = new Vector2(0, 0);
    private float _movespeed = 60;
    private bool _isWalking;
    private float _lastXposition;
    


    //valeur recuperee (0>>1) lorsque le bouton est active
    private bool _isJumping;
    [SerializeField] private float _jumpForce = 2f;
    private Vector2 _initialPos;


    //valeur recuperee (0>>1) lorsque le bouton est active
    private bool _isAttacking;

    //gestion de l'animator
    //private PlayerStateMode _currentState;
    private Animator _animator;


    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _animator = GetComponentInChildren<Animator>();
       
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

    private void GetInput()
    {
        _move = movement.action.ReadValue<Vector2>();
        
        _isWalking = movement.action.ReadValue<Vector2>() != new Vector2 (0,0);
        
        
        //prbs de tp
        //_isJumping = jump.action.triggered;
        //_isAttacking= attack.action.triggered; 
        _isJumping = jump.action.ReadValue<float>() > 0.1;
        _isAttacking = attack.action.ReadValue<float>() > 0.1;
    }

    private void Move()
    {
        _lastXposition = 0;
        float _currentPosition = _rb.position.x;
        _rb.velocity = _move * _movespeed * Time.fixedDeltaTime ;


        if (_currentPosition <_lastXposition)
        {
            _lastXposition = _currentPosition;
            //transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
        }
        else
        {
            _lastXposition += _currentPosition;

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

        if (_isJumping)
        {
            /*
            _rb.AddForce(transform.up * _jumpForce, ForceMode2D.Impulse);
            _rb.AddForce(transform.up * -_jumpForce, ForceMode2D.Impulse);*/

            _animator.SetBool("Jumping", true);
            StartCoroutine(doJump());
        }
        if (_isAttacking)
        {
            //_animator.SetTrigger("AttackTrigger");
            _animator.SetBool("Attacking", true);
        }
        else
        {
            _animator.SetBool("Attacking", false);
        }

    }

    IEnumerator doJump()
    {
        _rb.AddForce(transform.up * _jumpForce, ForceMode2D.Impulse);
        yield return new WaitForSeconds(.1f);
        _animator.SetBool("Jumping", false);

        //Debug.Log("waiting");
        //ça marche pas lel
        //_rb.AddForce(transform.up * -_jumpForce*2, ForceMode2D.Impulse);
        //Debug.Log("goingdown");
        //yield return new WaitForSeconds(.1f);


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


}
