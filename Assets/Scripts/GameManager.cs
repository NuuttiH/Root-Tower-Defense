using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;

    private float _startTime;
    public static float StartTime { get { return _instance._startTime; } }
    private int _totalScore;
    public static int Score { get { return _instance._totalScore; } }
    private int _mp;
    public static int Mp { get { return _instance._mp; } }

    private HashSet<Treasure> _treasures;
    [HideInInspector] public static HashSet<Treasure> Treasures { 
        get { return _instance._treasures; } 
    }
    private HashSet<Mole> _moles;
    [HideInInspector] public static HashSet<Mole> Moles { 
        get { return _instance._moles; } 
    }
    
    [SerializeField] private GameObject _screenCover;
    [SerializeField] private Button _startButton;
    [SerializeField] private GameObject _endgameUI;

    public static Action<int> onMpChange = delegate {};
    public static Action<int> onRootPriceChange = delegate {};
    public static Action<int> onDefensePriceChange = delegate {};
    public static Action<int> onTreasurePriceChange = delegate {};

    [Header("Game Settings")]
    [SerializeField] private int _baseIncome = 5;
    [SerializeField] private int _rootCost = 25;
    public static int RootCost { get { return _instance._rootCost; } }
    [SerializeField] private int _defenseCost = 75;
    public static int DefenseCost { get { return _instance._defenseCost; } }
    [SerializeField] private int _treasureCost = 125;
    public static int TreasureCost { get { return _instance._treasureCost; } }
    [SerializeField] private float _buildCostIncrement = 1.04f;
    [SerializeField] private float _treasureExtraCostIncrement = 1.15f;
    

    void Awake()
    {
		if(_instance == null) _instance = this;
		else
		{
			Destroy(this);
			return;
        }
        _treasures = new HashSet<Treasure>();
        _moles = new HashSet<Mole>();
        onMpChange = delegate {};
        onTreasurePriceChange = delegate {};


        StartCoroutine(Income());
        Time.timeScale = 0f;
        _startButton.onClick.AddListener( delegate{ StartGame(); } );
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator Income()
    {
        while(true)
        {
            yield return new WaitForSeconds(1f);
            _totalScore += _baseIncome;
            _mp += _baseIncome;
            onMpChange(_mp);
        }
    }

    public static void StartGame()
    {
        Time.timeScale = 1f;
        _instance._startTime = Time.time;
        Destroy(_instance._screenCover);
    }
    IEnumerator EndGame()
    {
        yield return new WaitForSeconds(1f);
        if(_instance._treasures.Count == 0)
        {
            Time.timeScale = 0f;
            Instantiate(_instance._endgameUI);
        }
    }

    public static void RegisterTreasure(Treasure t)
    {
        _instance._treasures.Add(t);
        _instance._treasureCost = (int) (_instance._treasureCost * _instance._treasureExtraCostIncrement);
        //onTreasurePriceChange(_instance._treasureCost);
        IncreaseBuildCost();
        _instance._baseIncome += t.Income;
    }
    public static void UnregisterTreasure(Treasure t)
    {
        _instance._treasures.Remove(t);
        _instance._baseIncome -= t.Income;
        if(_instance._treasures.Count == 0) _instance.StartCoroutine(_instance.EndGame());
    }
    public static void RegisterMole(Mole mole)
    {
        _instance._moles.Add(mole);
    }
    public static void UnregisterMole(Mole mole)
    {
        _instance._moles.Remove(mole);
    }

    public static bool TryUseMP(int amount)
    {
        if(_instance._mp >= amount)
        {
            _instance._mp = _instance._mp - amount;
            onMpChange(_instance._mp);
            return true;
        }
        else return false;
    }

    public static void IncreaseBuildCost()
    {
        int increasedAmount = (int) (_instance._rootCost * _instance._buildCostIncrement);
        if(increasedAmount == _instance._rootCost) _instance._rootCost++;
        else _instance._rootCost = increasedAmount;
        
        _instance._defenseCost = (int) (_instance._defenseCost * _instance._buildCostIncrement);
        _instance._treasureCost = (int) (_instance._treasureCost * _instance._buildCostIncrement);

        onRootPriceChange(_instance._rootCost);
        onDefensePriceChange(_instance._defenseCost);
        onTreasurePriceChange(_instance._treasureCost);
    }
}
