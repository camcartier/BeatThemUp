using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SmashDamage : MonoBehaviour
{
    public bool _isOnRange = false;

    private void Awake()
    {
        _isOnRange= false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerBody"))
        {
            _isOnRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerBody"))
        {
            _isOnRange = false;
        }
    }
}
