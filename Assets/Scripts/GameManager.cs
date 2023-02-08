using Cinemachine;
using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private IntVariables _playerHP;
    [SerializeField] private BoolVariables _wave;
    [SerializeField] private IntVariables _enemyCount;
    [SerializeField] public List<GameObject> _inactiveGruntList;
    [SerializeField] private int _activeGruntAmount; //combien on autorise de grunt à être actif en même temps
    [SerializeField] private IntVariables _currentActiveGrunt; //quantité de grunts active à l'instant T


    // Start is called before the first frame update
    void Start()
    {
        _playerHP.value = 100;
        _enemyCount.value = 0;
        _wave.value = false;
        _inactiveGruntList= new List<GameObject>();
        _currentActiveGrunt.value = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (_enemyCount.value == 0 && _inactiveGruntList.Count>0) { _inactiveGruntList.Clear(); }
        if (_enemyCount.value > 0)
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
        foreach (var item in _inactiveGruntList)
        {
            item.GetComponent<NPCBehaviour>()._isActive = true;
            _inactiveGruntList.Remove(item);
            _currentActiveGrunt.value++;
        }
    }

    private void ActivateGrunt(int qt)
    {
        for (int i = 0; i<qt; i++)
        {
            _inactiveGruntList[0].GetComponent<NPCBehaviour>()._isActive = true;
            _inactiveGruntList.Remove(_inactiveGruntList[0]);
            _currentActiveGrunt.value++;
        }
    }

    //faire une liste de gameobject grunt inactif (au spawn)
    //enemy count compte combien y'a de grunt
    //activer jusqu'à 3 grunts (le grunt est inactif au spawn) et les retirer de la liste
    // quand l'enemycount change voir combien sont actifs, si <3
    //activer le complément
}
