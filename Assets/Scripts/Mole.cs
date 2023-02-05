using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Mole : MonoBehaviour
{
    [SerializeField] private NavMeshAgent _agent;
    [SerializeField] private SpriteRenderer _renderer;
    [SerializeField] private int _health = 20;
    [SerializeField] private float _damageSlowdown = 0.75f;
    private int _rootsCounter;
    private bool _mutant = false;
    private float _baseSpeed;
    private Treasure _target;
    private AudioEvent _moleSounds;
    private AudioEvent _moleDeath;
    private float _lastHit = 0f;
    

    void Start()
    {
        float val = MoleSpawner.BoostValue;

        // Chance for mutant mole if BoostValue < 3.5, otherwise +1 boost
        if(val > 3.5f) val += 1f;
        else if(val > 1.5f && val > Random.Range(1f, 50f)){
            _health = (int)(_health * val);
            val = 6;
            _mutant = true;
            _agent.speed = _agent.speed * val * 0.14f;
        } 

        _baseSpeed = _agent.speed * val;
        _health =  (int)(_health * val);
        float colorMod = 1 / val;
        _renderer.color = new Color(1, colorMod, colorMod, 1);

        if(!_mutant && val > 5f) val = 5f;  // Max size for scale to stop giant moles

        var scale = this.gameObject.transform.GetChild(0).localScale;
        scale.x *= val;
        scale.y *= val;
        this.gameObject.transform.GetChild(0).localScale = scale;

        _moleSounds = MoleSpawner.GetSounds(_mutant);
        _moleDeath = MoleSpawner.GetDeathSounds(_mutant);

        SetNewTarget();
        StartCoroutine(UpdateSpeed());
        StartCoroutine(MakeSomeNoise());
        GameManager.RegisterMole(this);
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

    IEnumerator MakeSomeNoise()
    {
        yield return new WaitForSeconds(Random.Range(0.2f, 12f));
        var audioPlayer = new GameObject("Collapse audio", typeof (AudioSource)).GetComponent<AudioSource>();
        audioPlayer.transform.position = this.gameObject.transform.position;
        _moleSounds.Play(audioPlayer);
        Destroy(audioPlayer.gameObject, audioPlayer.clip.length*audioPlayer.pitch);
    }
    IEnumerator UpdateSpeed()
    {
        while(!_mutant) // Mutants don't get slowed down
        {
            yield return new WaitForSeconds(0.5f);
            if(_rootsCounter > 0) _agent.speed = 0.5f * _baseSpeed;
            else _agent.speed = _baseSpeed;

            if(Time.time - _lastHit < 1.5f) _agent.speed *= _damageSlowdown;
        }
    }

    public void TakeDamage(int damage)
    {
        _health -= damage;
        if(_health <= 0)
        {
            var audioPlayer = new GameObject("Collapse audio", typeof (AudioSource)).GetComponent<AudioSource>();
			audioPlayer.transform.position = this.gameObject.transform.position;
			_moleDeath.Play(audioPlayer);
			Destroy(audioPlayer.gameObject, audioPlayer.clip.length*audioPlayer.pitch);
            Destroy(this.gameObject, 0.1f);
        }
        else if(!_mutant)
        {
            // Slowdown from hit
            _agent.speed *= _damageSlowdown;
            _lastHit = Time.time;
        }
    }
}
