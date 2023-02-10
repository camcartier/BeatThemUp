using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowControl : MonoBehaviour
{
    private Animator _animator;
    private GameObject _player;
    private float _posYPlayer;
    private bool _hasPos;
    private SpriteRenderer _shadowSprite;

    private void Awake()
    {
        _animator= GetComponent<Animator>();
        _player= GameObject.Find("Player");
        _shadowSprite = GetComponent<SpriteRenderer>();
    }


    // Update is called once per frame
    void Update()
    {


        if (GameObject.Find("Player").GetComponent<PlayerMovement>()._isJumping == true)
        {
            _shadowSprite.enabled = false;
        }
        else
        {
            _shadowSprite.enabled = true;
        }

    }



}
