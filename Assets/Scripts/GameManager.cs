using Cinemachine;
using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private IntVariables _playerHP;
    [SerializeField] private IntVariables _playerMana;
    [SerializeField] private BoolVariables _wave;
    [SerializeField] private IntVariables _enemyCount;
    [SerializeField] public List<GameObject> _inactiveGruntList, _activeGruntList;
    [SerializeField] private int _activeGruntAmount; //combien on autorise de grunt à être actif en même temps
    [SerializeField] private IntVariables _currentActiveGrunt; //quantité de grunts active à l'instant T
    [SerializeField] private IntVariables _scoreCounter;

    private void Awake()
    {
        _scoreCounter.value = 0;
        DontDestroyOnLoad(this.gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        _playerHP.value = 100;
        _playerMana.value = 0;
        _enemyCount.value = 0;
        _wave.value = false;
        _inactiveGruntList= new List<GameObject>();
        _activeGruntList= new List<GameObject>();
        _currentActiveGrunt.value = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (_enemyCount.value == 0 && _inactiveGruntList.Count>0) { _inactiveGruntList.Clear(); }
        if (_enemyCount.value > 0 && _inactiveGruntList.Count>0)
        {
            //si on a une quantité d'ennemis inférieur ou égale au max d'actifs et que le nombre de grunt actifs est inférieur à la quantité d'ennemis, on active ce qu'il reste dans la liste
            if (_enemyCount.value <= _activeGruntAmount && _currentActiveGrunt.value < _enemyCount.value)
            {
                ActivateAllGrunt();
            }
            //si on a une quantité d'ennemis supérieure au max d'actifs, on regarde si on a le max de grunt active, sinon on active ce qu'il manque
            else if (_enemyCount.value > _activeGruntAmount && _currentActiveGrunt.value < _activeGruntAmount)
            {
                int a = _activeGruntAmount - _currentActiveGrunt.value;
                ActivateGrunt(a);
            }
        }
    }

    private void ActivateAllGrunt()
    {
        for (int i = 0; i < _inactiveGruntList.Count; i++)
        {
            _inactiveGruntList[0].GetComponent<NPCBehaviour>()._isActive = true;
            _activeGruntList.Add(_inactiveGruntList[0]);
            _inactiveGruntList.Remove(_inactiveGruntList[0]);
            _currentActiveGrunt.value++;
        }
        //foreach (var item in _inactiveGruntList)
        //{
        //    item.GetComponent<NPCBehaviour>()._isActive = true;
        //    _inactiveGruntList.Remove(item);
        //    _currentActiveGrunt.value++;
        //}
    }

    private void ActivateGrunt(int qt)
    {
        for (int i = 0; i<qt; i++)
        {
            _inactiveGruntList[0].GetComponent<NPCBehaviour>()._isActive = true;
            _activeGruntList.Add(_inactiveGruntList[0]);
            _inactiveGruntList.Remove(_inactiveGruntList[0]);
            _currentActiveGrunt.value++;
        }
    }

    public void GameOver()
    {
        SceneManager.LoadScene(3);
        //faire un loadscene du gameover
    }

    public void Quit()
    {
        Application.Quit();        
    }

    public void LoadLvl1()
    {
        SceneManager.LoadScene(1);
    }

    public void Victory()
    {
        SceneManager.LoadScene(2);
    }
}
