using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D _rb;
    [SerializeField] private InputActionReference movement, attack, jump;
    private Vector2 _move;
    private float _movespeed = 5;

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
    }

    private void GetInput()
    {
        _move = movement.action.ReadValue<Vector2>();
    }

    private void Move()
    {
        _rb.velocity = _move * _movespeed * Time.fixedDeltaTime ;
    }

    private void Jump()
    {

    }

    private void Attack()
    {

    }
}
