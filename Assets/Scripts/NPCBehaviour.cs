using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class NPCBehaviour : MonoBehaviour
{
    [SerializeField] public int _hp, _attPower, _defPower;
    [SerializeField] private float _moveSpeed;
    private Rigidbody2D _rb; //npc RB2D
    private GameObject _player;
    private Vector2 _direction2Player; //vector2 to have the direction towards the player
    private Transform _playerTransform;
    private float _distance;
    [SerializeField] private float _moveDistance;
    [SerializeField] public IntVariables _playerHealth; //player health scriptableobject to modify it's value on hit
    [SerializeField] public bool _isActive; //is the NPC active (move and attack the player) or inactive
    [SerializeField] private float _attackCD; //attack cooldown timer
    [SerializeField] private bool _onCombat; //is the NPC already fighting or not
    [SerializeField] private Animator _animator;

    [SerializeField] private float _nextAttack =0;
    [SerializeField] private IntVariables _enemyCount;
    [SerializeField] private IntVariables _currentActiveGrunt; //quantité de grunts active à l'instant T
    private bool _isDead;
    private bool _flipX;
    [SerializeField] GameObject _prefabVynil;
    [SerializeField] GameObject _prefabPoofingFX;

    private void Awake()
    {
        _rb= GetComponent<Rigidbody2D>();
        _player = GameObject.FindGameObjectWithTag("PlayerBody");
        _playerTransform= _player.GetComponent<Transform>();
        _isActive= false;
        _isDead= false;
    }
    // Start is called before the first frame update
    void Start()
    {
        _enemyCount.value++;
    }

    // Update is called once per frame
    void Update()
    {
        if (_hp <=0 && _isDead==false) { StartCoroutine(Death()); _isDead = true; }

        //fishtree
        //ajout pour test a supprimer
        //if (_hp <= 0)
        //{
        //    _isDead = true;
        //    _isActive= false;
        //    Death2();
        //}
        //Debug.Log($"{_hp}");
    }

    private void FixedUpdate()
    {
        if (_isActive && _isDead==false) Behaviour();
        else if (!_isActive && _isDead == false ) InactiveBehaviour();
    }

    private void InactiveBehaviour()
    {
        if (Vector2.Distance(transform.position, _playerTransform.position) > _moveDistance * 6) Move();
        else if(Vector2.Distance(transform.position, _playerTransform.position) < _moveDistance * 4) MoveAway();
        else Stop();
    }

    //NPC behaviour method
    private void Behaviour()
    {
        //float _distance = Vector2.Distance(transform.position, _playerTransform.position);
        //Debug.Log(_distance);
        //check si la distance entre l'ennemi et le joueur est suffisante pour le bouger ou le stopper
        if (Vector2.Distance(transform.position, _playerTransform.position) > _moveDistance) Move();
        else 
        { 
            Stop(); 
            if (_isActive && Time.timeSinceLevelLoad>_nextAttack && _isDead == false)
            {
                Combat();
            }
        }

    }


    private void Move()
    {
        //deplacement vers le joueur, le sprite flip sur X en fonction de la direction du mouvement
        _direction2Player = _playerTransform.position - transform.position;
        _animator.SetFloat("DirX", _direction2Player.x);
        _animator.SetBool("Idle", false);
        Vector2 _offset = GetComponentInChildren<CircleCollider2D>().offset;
        if (_direction2Player.x > 0)
        {
            GetComponentInChildren<SpriteRenderer>().flipX = false;
            if (_offset.x<0) GetComponentInChildren<CircleCollider2D>().offset = new Vector2(0.3f, _offset.y);
        }
        else if (_direction2Player.x < 0)
        {
            GetComponentInChildren<SpriteRenderer>().flipX = true;
            if (_offset.x > 0) GetComponentInChildren<CircleCollider2D>().offset = new Vector2(-0.3f, _offset.y);
        }
        _rb.velocity = _direction2Player.normalized * _moveSpeed * Time.fixedDeltaTime;
    }

    private void MoveAway()
    {
        if(!_flipX) { _flipX = true; }
        //deplacement vers le joueur, le sprite flip sur X en fonction de la direction du mouvement
        _direction2Player = _playerTransform.position - transform.position;
        _direction2Player.x = -_direction2Player.x;
        _animator.SetFloat("DirX", _direction2Player.x);
        _animator.SetBool("Idle", false);
        Vector2 _offset = GetComponentInChildren<CircleCollider2D>().offset;
        if (_direction2Player.x > 0)
        {
            GetComponentInChildren<SpriteRenderer>().flipX = false;
            if (_offset.x < 0) GetComponentInChildren<CircleCollider2D>().offset = new Vector2(0.3f, _offset.y);
        }
        else if (_direction2Player.x < 0)
        {
            GetComponentInChildren<SpriteRenderer>().flipX = true;
            if (_offset.x > 0) GetComponentInChildren<CircleCollider2D>().offset = new Vector2(-0.3f, _offset.y);
        }
        _rb.velocity = _direction2Player.normalized * _moveSpeed * Time.fixedDeltaTime;
    }

    private void Stop()
    {
        if(_flipX) { _flipX = false; GetComponentInChildren<SpriteRenderer>().flipX = !GetComponentInChildren<SpriteRenderer>().flipX; }
        //s'assurer que le NPC regarde le joueur (flip X une fois quand il move away)
        _animator.SetBool("Idle", true);
        _rb.velocity = UnityEngine.Vector2.zero;
    }

    private void Combat()
    {
        _animator.SetBool("Attack",true);
        //condition pour savoir si on est en range de toucher (le collider d'attaque qui touche le body du player)
        if (GetComponentInChildren<Damage>()._isOnRange==true) DealDmg();
        _nextAttack = Time.timeSinceLevelLoad + _attackCD;
        StartCoroutine(AttCD());
    }

    private void DealDmg()
    {
        _playerHealth.value -= _attPower;
    }

    //l'ennemi va clignoter un peu avant de mourrir
    IEnumerator Death()
    {
        _animator.SetBool("Death", true);
        _rb.velocity = UnityEngine.Vector2.zero;
        yield return new WaitForSeconds(1);
        for (int i = 0; i<8;i++)
        {
            yield return new WaitForSeconds(0.15f);
            GetComponentInChildren<SpriteRenderer>().enabled= false;
            yield return new WaitForSeconds(0.15f);
            GetComponentInChildren<SpriteRenderer>().enabled= true;
        }
        _enemyCount.value--;
        _currentActiveGrunt.value--;
        Instantiate(_prefabPoofingFX, transform.position , Quaternion.identity);
        Instantiate(_prefabVynil, transform.position, Quaternion.identity);
        Destroy(gameObject);
        yield return null;
    }

    IEnumerator AttCD()
    {
        yield return new WaitForSeconds(0.05f);
        _animator.SetBool("Attack", false);
    }



    //fishtree
    private void TakeDamage()
    {
        _hp -= _player.GetComponent<PlayerMovement>().PlayerAttPower;
    }
    //fishtree
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerFist"))
        {
            //TakeDamage();
        }
    }
    //je pige pas tout le fonctionnement du Death() de plus haut
    //je met ici bas un scotch provisoire
    //fishtree
    void Death2()
    {
        _animator.SetBool("Death", true);
        _rb.velocity = UnityEngine.Vector2.zero;
        StartCoroutine(Death());
    }


}
