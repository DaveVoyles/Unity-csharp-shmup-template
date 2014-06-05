using System;
using PathologicalGames;
using UnityEngine;
using System.Collections;

public class PowerUp : MonoBehaviour {

    private Player _player = null;
    private const float BULLET_VELOCITY = 55f;
    private const float PLAYER_SPEED    = 26f;
    private const float FIRE_RATE       = 0.018f;
    private const int   BULLET_DMG      = 3;
    private ParticleEffectsManager _particleManager = null;
    private Transform _xForm;
    private SpawnPool _pool = null;


	void Start ()
	{
	    _player          = GameObject.Find("Player").GetComponent<Player>();
        _particleManager = GameObject.Find("ParticleManager").GetComponent<ParticleEffectsManager>();
	    _xForm           = transform;
        _pool            = GameObject.Find("GameManager").GetComponent<GameManager>().BulletPool;

	}
	
	// Update is called once per frame
	void Update () {
	
	}

    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.CompareTag("Player")) return;
        IncreaseFireRate();
        IncreasePlayerSpeed();
        IncreaseBulletVelocity();
        IncreaseBulletDmg();
        _particleManager.CreatePowerupParticleEffects(_xForm.position);
        _pool.Despawn(transform);
    }

    private void IncreaseFireRate()
    {
        _player.SetFireRate(FIRE_RATE);
        print("setting fire rate to:" + "" + _player.GetFireRate());
    }

    private void IncreaseBulletVelocity()
    {
        _player.SetBulletVelocity(BULLET_VELOCITY);
        print("setting bullet velocity to:" + "" + _player.GetBulletVelocity());
    }

    private void IncreasePlayerSpeed()
    {
        _player.SetPlayerSpeed(PLAYER_SPEED);
        print("setting player speed to:" + "" + _player.GetPlayerSpeed());

    }

    private void IncreaseBulletDmg()
    {
        _player.SetBulletDmg(BULLET_DMG);
        print("setting bullet damage to" + "" + _player.GetBulletDmg());
    }



}
