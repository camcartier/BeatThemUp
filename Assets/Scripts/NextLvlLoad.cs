using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NextLvlLoad : MonoBehaviour
{
    private bool _nextlvl;

    private void Start()
    {
        _nextlvl = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerBody") && _nextlvl == false) { _nextlvl = true; StartCoroutine(NextLvl()); }
    }

    IEnumerator NextLvl()
    {
        yield return new WaitForSeconds(2);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
