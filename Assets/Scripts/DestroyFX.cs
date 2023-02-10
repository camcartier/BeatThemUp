using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyFX : MonoBehaviour
{
    Animator _animator;
    AnimatorStateInfo _animStateInfo;
    private float NTime;
    bool _finished;
    float _timerCounter;
    float _duration;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _duration = 3f;
    }


    // Update is called once per frame
    void Update()
    {
        _timerCounter += Time.deltaTime;

        if (_timerCounter > _duration)
        {
            Destroy(gameObject);
        }

    }


    /*public Animator animator;
AnimatorStateInfo animStateInfo;
public float NTime;
 
bool animationFinished;
 
 
animStateInfo = animator.GetCurrentAnimatorStateInfo (0);
NTime = animStateInfo.normalizedTime;
 
if(NTime > 1.0f) animationFinished = true;*/

}
