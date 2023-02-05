using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{
    [SerializeField] private GameObject _projectilePrefab;
    private Mole _target;
    private int _doubt;
    
    [SerializeField] private TowerData _data;

    void Start()
    {
        StartCoroutine(Operate());
    }

    IEnumerator Operate()
    {
        while(true)
        {
            if(_target == null) 
                _target = Tools.GetClosestFromSet(this.transform.position, GameManager.Moles);
            if(_target != null)
            {
                if(Vector3.Distance(this.transform.position, _target.gameObject.transform.position) < _data.range) 
                    Fire();
                else
                {
                    // Try switching targets if never close enough
                    _doubt++;
                    if(_doubt > 3) _target = null;
                }
            }
            yield return new WaitForSeconds(_data.attackCooldown);
        }
    }

    private void Fire()
    {
        GameObject obj = Instantiate(_projectilePrefab);
        obj.transform.position = this.gameObject.transform.position;
        obj.GetComponent<Projectile>().Init(_data.damage, _target);
    }
}
