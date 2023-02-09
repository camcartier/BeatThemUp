using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowControl : MonoBehaviour
{
    private Animator _animator;

    private void Awake()
    {
        _animator= GetComponent<Animator>();
    }


    // Update is called once per frame
    void Update()
    {
        if (GameObject.Find("Player").GetComponent<PlayerMovement>()._isJumping == true)
        {
            _animator.SetBool("IsJumping", true);
        }
        else
        {
            _animator.SetBool("IsJumping", false);
        }
    }
}
