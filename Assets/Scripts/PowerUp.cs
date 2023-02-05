using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    [Header("Power up settings")]
    [SerializeField] private TowerStat stat = TowerStat.Damage;
    [SerializeField] private float modifier = 0.4f;

    
    private void OnTriggerEnter(Collider other)
    {
        switch(other.gameObject.layer)
        {
            case 7: // Root
                GameManager.TowerData.Adjust(stat, modifier);
                Destroy(this.gameObject);
                break;
            default:
                break;
        }
    }
}
