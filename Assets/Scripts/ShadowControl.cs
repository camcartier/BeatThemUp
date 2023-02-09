using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowControl : MonoBehaviour
{
    private Animator _animator;
    private GameObject _player;
    private float _posYPlayer;
    private bool _hasPos;

    private void Awake()
    {
        _animator= GetComponent<Animator>();
        _player= GameObject.Find("Player");
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
