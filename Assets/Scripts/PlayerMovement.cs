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
    private float _movespeed = 60;
    private bool _isWalking;


    //valeur recuperee (0>>1) lorsque le bouton est active
    private float _isJumping;
    [SerializeField] private float _jumpForce = 2f;

    //valeur recuperee (0>>1) lorsque le bouton est active
    private float _isAttacking;

    //gestion de l'animator
    private PlayerStateMode _currentState;
    private Animator _animator;


    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
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
        if (_isJumping != 0)
        {
            Jump();
        }

    }

    private void GetInput()
    {
        _move = movement.action.ReadValue<Vector2>();
        _isJumping = jump.action.ReadValue<float>();
        _isAttacking= attack.action.ReadValue<float>(); 
    }

    private void Move()
    {
        _rb.velocity = _move * _movespeed * Time.fixedDeltaTime ;
        
    }

    private void Jump()
    {
        _rb.AddForce(transform.up *_jumpForce, ForceMode2D.Impulse);
    }

    private void Attack()
    {
        
    }

    #region Testing Debug
    /*
    void OnJump()
    {
        Debug.Log("Jump pressed");

    }

    void OnAttack()
    {
        Debug.Log("Attack");

    }*/
    #endregion

    void OnStateEnter()
    {
        switch (_currentState)
        {
            case PlayerStateMode.IDLE:
                break;

            case PlayerStateMode.WALK:

                _animator.SetBool("Walking", true);
                break;

            case PlayerStateMode.JUMP:

                _animator.SetBool("Jumping", true);
                break;

            case PlayerStateMode.ATTACK:

                _animator.SetBool("Attacking", true);
                break;
        }
    }

    void OnStateUpdate()
    {
        switch (_currentState)
        {
            case PlayerStateMode.IDLE:
                break;

            case PlayerStateMode.WALK:

                if (_isJumping>0)
                {
                    TransitionToState(PlayerStateMode.JUMP);
                }
                if (_isAttacking>0)
                {
                    TransitionToState(PlayerStateMode.ATTACK);
                }
                break;

            default:
                break;
        }
    }

    void OnStateExit()
    {

    }

    void TransitionToState(PlayerStateMode toState)
    {
        OnStateExit();

        _currentState = toState;

        OnStateEnter();
    }
}
