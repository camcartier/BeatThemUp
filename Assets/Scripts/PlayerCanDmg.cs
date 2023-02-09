using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCanDmg : MonoBehaviour
{
    public bool _canDmg = false;
    [SerializeField] private float _nextAttack;
    [SerializeField] private float _AttackCD;
    private List<Collider2D> _colliders;

    private void Awake()
    {
        _colliders= new List<Collider2D>();
        _canDmg = false;
        _nextAttack= 0;
    }

    private void Update()
    {
        if (GetComponentInParent<PlayerMovement>()._isAttacking && Time.timeSinceLevelLoad > _nextAttack) { DealDmg(); }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("EnemyBody"))
        {
            _canDmg = true;
            _colliders.Add(collision);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("EnemyBody"))
        {
            _canDmg = false;
            _colliders.Remove(collision);
        }
    }


    private void DealDmg()
    {
        int Dmg = GetComponentInParent<PlayerMovement>().PlayerAttPower;
        foreach (var item in _colliders)
        {
            item.GetComponent<NPCBehaviour>()._hp -= Dmg;
        }
        _nextAttack = Time.timeSinceLevelLoad + _AttackCD;
        
    }
}