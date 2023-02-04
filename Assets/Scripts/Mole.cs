using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Mole : MonoBehaviour
{
    [SerializeField] private NavMeshAgent _agent;
    [SerializeField] private SpriteRenderer _renderer;
    private int _rootsCounter;
    private float _baseSpeed;
    private Treasure _target;
    [SerializeField] private int _health = 20;
    

    void Start()
    {
        float val = MoleSpawner.BoostValue;
        _baseSpeed = _agent.speed * val;
        _health =  (int)(_health * val);
        var scale = this.gameObject.transform.GetChild(0).localScale;
        scale.x *= val;
        scale.y *= val;
        this.gameObject.transform.GetChild(0).localScale = scale;
        float colorMod = 1 / val;
        _renderer.color = new Color(1, colorMod, colorMod, 1);


        SetNewTarget();
        StartCoroutine(UpdateSpeed());
        GameManager.RegisterMole(this);

        ;
    }

    void OnDestroy()
    {
        GameManager.UnregisterMole(this);
        if(_target != null) _target.UnregisterMole(this);
    }

    private void OnTriggerEnter(Collider other)
    {
        switch(other.gameObject.layer)
        {
            case 7: // Root
                _rootsCounter ++;
                break;
            case 8: // Treasure
                Destroy(other.gameObject);
                break;
            case 9: // Projectile
                TakeDamage(other.gameObject.GetComponent<Projectile>().BlowUp());
                break;
            default:
                break;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        switch(other.gameObject.layer)
        {
            case 7: // Root
                _rootsCounter --;
                break;
            default:
                break;
        }
    }

    public void SetNewTarget(Treasure target = null)
    {
        if(target == null) 
            target = Tools.GetClosestFromSet(this.transform.position, GameManager.Treasures);
        if(target == null)
        {
            StartCoroutine(Wait());
        }
        else
        {
            _agent.SetDestination(target.gameObject.transform.position);
            target.RegisterMole(this);
            _target = target;
        }
    }
    IEnumerator Wait()
    {
        yield return new WaitForSeconds(1.1f);
        SetNewTarget();
    }

    IEnumerator UpdateSpeed()
    {
        while(true)
        {
            yield return new WaitForSeconds(0.5f);
            if(_rootsCounter > 0) _agent.speed = 0.5f * _baseSpeed;
            else _agent.speed = _baseSpeed;
        }
    }

    public void TakeDamage(int damage)
    {
        _health -= damage;
        if(_health <= 0)
        {
            Destroy(this.gameObject);
        }
    }
}
