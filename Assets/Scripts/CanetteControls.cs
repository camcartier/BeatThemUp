using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CanetteControls : MonoBehaviour
{
    private Rigidbody2D _rbCanette;
    [SerializeField] private float _throwSpeed;
    private Vector2 _direction;

    // Start is called before the first frame update
    void Start()
    {
        _rbCanette = GetComponent<Rigidbody2D>();

        
        
        Move();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Move()
    {
        if (GameObject.Find("Player").GetComponentInChildren<SpriteRenderer>().flipX == true)
        {
            _direction = new Vector2(-1, 0);
        }
        else
        {
            _direction = new Vector2(1, 0);
        }
        _rbCanette.velocity = _direction * _throwSpeed * Time.deltaTime;   
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Destroy(this.gameObject);
    }
}
