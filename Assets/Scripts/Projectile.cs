using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Projectile : MonoBehaviour
{
    private NavMeshAgent _agent;
    private int _damage;
    private Mole _target;
    private Vector3 _targetPos;
    [SerializeField] private AudioEvent _attackSound;
    [SerializeField] private AudioEvent _hitSound;
    
    public void Init(int damage, Mole target)
    {
        _agent = GetComponent<NavMeshAgent>();
        _damage = damage;
        _target = target;

        _targetPos = _target.gameObject.transform.position;
        _agent.SetDestination(_targetPos);
        StartCoroutine(UpdateTarget());
        Tools.PlayAudio(this.gameObject, _attackSound);
    }

    IEnumerator UpdateTarget()
    {
        while(true)
        {
            yield return new WaitForSeconds(0.3f);
            if(_target == null) // Target dead, blowup on last known location
            {
                if(Vector3.Distance(this.transform.position, _targetPos) < 0.5f) BlowUp();
            }
            else    // Target alive, update targetpos
            {
                _targetPos = _target.gameObject.transform.position;
                _agent.SetDestination(_targetPos);
            }
        }
    }

    public int BlowUp()
    {
        StartCoroutine(Death());
        return _damage;
    }
    IEnumerator Death()
    {
        Tools.PlayAudio(this.gameObject, _hitSound);
        yield return new WaitForSeconds(0.2f);
        Destroy(this.gameObject);
    }
}
