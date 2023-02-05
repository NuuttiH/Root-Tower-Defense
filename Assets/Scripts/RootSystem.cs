using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum BuildingType { None, Root, Defense, Treasure, Destroyed }

public class RootSystem : MonoBehaviour
{
    private static RootSystem _instance;
    
    [SerializeField] private  GridLayout _gridLayout;
    public static GridLayout GridLayout { get { return _instance._gridLayout; } }
    private Grid _grid;
    [SerializeField] private Tilemap _tilemap;
    [SerializeField] private TileBase _rootTile;
    [SerializeField] private TilemapRenderer _tileMapRenderer;

    [SerializeField] private GameObject _hiddenRootObject;
    [SerializeField] private GameObject _defenseObject;
    [SerializeField] private GameObject _treasureObject;
    [SerializeField] private GameObject _destroyedTreasureObject;
    [SerializeField] private GameObject _hoveringRoot;
    [SerializeField] private GameObject _hoveringDefense;
    [SerializeField] private GameObject _hoveringTreasure;
    [SerializeField] private LayerMask _backgroundLayer;

    [SerializeField] private Vector3Int[] _startingRoots;
    [SerializeField] private Vector3Int[] _startingDefenses;
    [SerializeField] private Vector3Int[] _startingTreasures;

    private GameObject _currentObject;
    private BuildingType _buildingType;
    private HashSet<Vector3Int> _buildingLocations;
    private bool _clickCooldown = false;
    private Vector3 _missVector = new Vector3(-100f, -100f, -100f);

    private void Awake()
    {
		if(_instance == null) _instance = this;
		else
		{
			Destroy(this);
			return;
        }
        _grid = GridLayout.gameObject.GetComponent<Grid>();
        _buildingLocations = new HashSet<Vector3Int>();
    }
    void Start()
    {
        foreach(Vector3Int position in _startingRoots)
        {
            BuildRoot(position, false);
        }
        foreach(Vector3Int position in _startingDefenses)
        {
            BuildBuilding(position, BuildingType.Defense, false);
        }
        foreach(Vector3Int position in _startingTreasures)
        {
            BuildBuilding(position, BuildingType.Treasure, false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Listen for input to enter building mode
        if(Input.GetKeyDown(KeyCode.R))
        {
            StartBuildingMode(BuildingType.Root);
        }
        else if(Input.GetKeyDown(KeyCode.D))
        {
            StartBuildingMode(BuildingType.Defense);
        }
        else if(Input.GetKeyDown(KeyCode.T))
        {
            StartBuildingMode(BuildingType.Treasure);
        }

        // Build
        if(!_clickCooldown) // Stop build from firing right after button click
        {
            if(_currentObject != null && Input.GetMouseButtonUp(0))
            {
                switch(_buildingType)
                {
                    case BuildingType.Root:
                        if(CanBePlaced())
                            BuildRoot(GridLayout.WorldToCell(_currentObject.transform.position));
                        break;
                    case BuildingType.Defense:
                    case BuildingType.Treasure:
                        if(CanBeBuild())
                            BuildBuilding(GridLayout.WorldToCell(_currentObject.transform.position), _buildingType);
                        break;
                }
                Destroy(_currentObject);
                _currentObject = null;
                _buildingType = BuildingType.None;
            }
            else if(Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonUp(1))
            {
                Destroy(_currentObject);
                _currentObject = null;
                _buildingType = BuildingType.None;
            }
        }
    }

    // Functions for grid based building system
    public static Vector3 GetMouseWorldPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //Debug.DrawRay(ray.origin, ray.direction *150, Color.green, 10f);
        if(Physics.Raycast(ray, out RaycastHit hit, 500f, _instance._backgroundLayer))
        {
            //Debug.Log(hit.collider.gameObject.name);
            return hit.point;
        }
        else
        {
            //Debug.Log("No raycast hit");
            return _instance._missVector;
        } 
    }
    public static Vector3 SnapCoordinateToGrid(Vector3 position)
    {
        Vector3Int cellPos = GridLayout.WorldToCell(position);
        position = _instance._grid.GetCellCenterWorld(cellPos);
        return position;
    }
    public static void StartBuildingMode(BuildingType buildingType)
    {
        GameObject obj;
        switch(buildingType)
        {
            case BuildingType.Root:
                obj = Instantiate(_instance._hoveringRoot);
                break;
            case BuildingType.Defense:
                obj = Instantiate(_instance._hoveringDefense);
                break;
            case BuildingType.Treasure:
                obj = Instantiate(_instance._hoveringTreasure);
                break;
            default:
                return;
        }

        Vector3 position = SnapCoordinateToGrid(_instance._missVector);
        obj.transform.position = position;
        if(_instance._currentObject != null) Destroy(_instance._currentObject);
        _instance._currentObject = obj;
        _instance._buildingType = buildingType;
    }

    private static bool CanBePlaced()
    {
        Vector3Int position = GridLayout.WorldToCell(_instance._currentObject.transform.position);

        // Check that the location is empty and next to a root
        if(_instance._tilemap.GetTile(position) == _instance._rootTile){
            //Debug.Log($"CanBePlaced: false, {position.x}, {position.y} is occupied.");
            return false;
        }
        if(_instance._tilemap.GetTile(position + new Vector3Int(1, 0, 0)) == _instance._rootTile){
            //Debug.Log($"CanBePlaced: True, adjacent root in {position.x + 1}, {position.y}.");
            return true;
        }
        if(_instance._tilemap.GetTile(position + new Vector3Int(0, 1, 0)) == _instance._rootTile){
            //Debug.Log($"CanBePlaced: true, adjacent root in {position.x}, {position.y + 1}.");
            return true;
        }
        if(_instance._tilemap.GetTile(position + new Vector3Int(-1, 0, 0)) == _instance._rootTile){
            //Debug.Log($"CanBePlaced: true, adjacent root in {position.x - 1}, {position.y}.");
            return true;
        }
        if(_instance._tilemap.GetTile(position + new Vector3Int(0, -1, 0)) == _instance._rootTile){
            //Debug.Log($"CanBePlaced: true, adjacent root in {position.x}, {position.y - 1}.");
            return true;
        }
        //Debug.Log($"CanBePlaced: false, no root is adjacent to {position.x}, {position.y}.");
        return false;
    }
    public static void BuildRoot(Vector3Int position, bool pay = true)
    {
        if(pay)
        {
            if(GameManager.TryUseMP(GameManager.RootCost))
            {
                //Debug.Log($"Building root in {position.x}, {position.y}.");
                _instance._tilemap.SetTile(position, _instance._rootTile);
                GameObject obj = Instantiate(_instance._hiddenRootObject, _instance.gameObject.transform);
                obj.transform.position = _instance._tilemap.GetCellCenterWorld(position);
                GameManager.IncreaseBuildCost(BuildingType.Root);
            }
            //else Debug.Log($"Building root failed, not enough money");
        }
        else
        {
            _instance._tilemap.SetTile(position, _instance._rootTile);
            GameObject obj = Instantiate(_instance._hiddenRootObject, _instance.gameObject.transform);
            obj.transform.position = _instance._tilemap.GetCellCenterWorld(position);
        }
    }

    private bool CanBeBuild()
    {
        Vector3Int position = GridLayout.WorldToCell(_instance._currentObject.transform.position);

        // Check that the location has a root, but no building
        if(_instance._tilemap.GetTile(position) != _rootTile){
            //Debug.Log($"CanBeBuild: false, {position.x}, {position.y} has no root.");
            return false;
        }
        if(_instance._buildingLocations.Contains(position))
        {
            //Debug.Log($"CanBeBuild: false, {position.x}, {position.y} already has a building.");
            return false;
        }
        
        //Debug.Log($"CanBeBuild: true, a free root is in {position.x}, {position.y}.");
        return true;
    }
    public static void BuildBuilding(Vector3Int position, BuildingType buildingType, bool pay = true)
    {
        GameObject obj = null;
        switch(buildingType)
        {
            case BuildingType.Defense:
                if(pay)
                {
                    if(GameManager.TryUseMP(GameManager.DefenseCost))
                    {
                        obj = Instantiate(_instance._defenseObject, _instance.gameObject.transform);
                        _instance._buildingLocations.Add(position);
                        GameManager.IncreaseBuildCost(BuildingType.Defense);
                    }
                    //else Debug.Log($"Building defense failed, not enough money");
                }
                else
                {
                    obj = Instantiate(_instance._defenseObject, _instance.gameObject.transform);
                    _instance._buildingLocations.Add(position);
                }
                break;
            case BuildingType.Treasure:
                if(pay)
                {
                    if(GameManager.TryUseMP(GameManager.TreasureCost))
                    {
                        obj = Instantiate(_instance._treasureObject, _instance.gameObject.transform);
                        _instance._buildingLocations.Add(position);
                        GameManager.IncreaseBuildCost(BuildingType.Treasure);
                    }
                    //else Debug.Log($"Building defense failed, not enough money");
                }
                else
                {
                    obj = Instantiate(_instance._treasureObject, _instance.gameObject.transform);
                    _instance._buildingLocations.Add(position);
                }
                break;
            case BuildingType.Destroyed:
                obj = Instantiate(_instance._destroyedTreasureObject, _instance.gameObject.transform);
                break;
            default:
                break;
        }
        if(obj != null) obj.transform.position = _instance._tilemap.GetCellCenterWorld(position) + new Vector3(0f, 0.1f, 0f);
    }

    public static void ClickCooldown()
    {
        _instance._clickCooldown = true;
        _instance.StartCoroutine(_instance.ClickCooldownOff());
    }
    IEnumerator ClickCooldownOff()
    {
        yield return new WaitForSeconds(0.2f);
        _instance._clickCooldown = false;
    }
}
