using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class HealthCan : MonoBehaviour
{
    [SerializeField] private int _healthGiven =20;
    [SerializeField] private IntVariables _playerHealth;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerBody"))
        {

            if (_playerHealth.value < 100 && _playerHealth.value + _healthGiven <=100)
            {
                _playerHealth.value += _healthGiven;
            }
            else { _playerHealth.value = 100; }


            Destroy(this.gameObject);
        }

    }

}
