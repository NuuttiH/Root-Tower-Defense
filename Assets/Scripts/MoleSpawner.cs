using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoleSpawner : MonoBehaviour
{
    private static MoleSpawner _instance;
    private List<Vector3> _spawnPoints;

    [SerializeField] private GameObject _molePrefab;
    private float _boostValue = 1f;
    [HideInInspector] public static float BoostValue { get { return _instance._boostValue; } }

    [Header("Spawner Settings")]
    [SerializeField] private float _spawnInterval = 12f;
    [SerializeField] private int _spawnPerWave = 4;
    [SerializeField] private float _boostInterval = 11f;
    [SerializeField] private float _boostIncrement = 0.05f;
    [SerializeField] private float _calmBeforeTheStorm = 15f;


    void Awake()
    {
		if(_instance == null) _instance = this;
		else
		{
			Destroy(this);
			return;
        }

        _spawnPoints = new List<Vector3>();
        foreach(Transform child in gameObject.transform)
        {
            _spawnPoints.Add(child.position);
        }

        StartCoroutine(SpawnMoles());
        StartCoroutine(Booster());
    }

    IEnumerator SpawnMoles()
    {
        while(true)
        {
            yield return new WaitForSeconds(_spawnInterval * (1f / _boostValue));
            
            for(int i=0; i<(int)(_spawnPerWave * _boostValue); i++)
            {
                Vector3 position = _spawnPoints[Random.Range(0, _spawnPoints.Count)];
                GameObject obj = Instantiate(_molePrefab);
                obj.transform.position = position;
            }
        }
    }
    IEnumerator Booster()
    {
        yield return new WaitForSeconds(_calmBeforeTheStorm);
        
        while(true)
        {
            yield return new WaitForSeconds(_boostInterval);
            
            _boostValue += _boostIncrement;
        }
    }
}
