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
    private SpawnPool _pool             = null;
    private Vector3[] _path             = null;
    private Weapons   _weapons          = null;
    
    /// <summary>  The enemy being killed determines the type when killed </summary>
    public enum PickupType
    {
        FireRate,
        BulletVel,
        SpeedBoost,
        BulletDmg
    }
    public PickupType pickupType = PickupType.FireRate;

    void Start ()
	{
	    _player          = GameObject.Find("Player").GetComponent<Player>();
        _particleManager = GameObject.Find("ParticleManager").GetComponent<ParticleEffectsManager>();
	    _xForm           = transform;
        _pool            = GameObject.Find("GameManager").GetComponent<GameManager>().BulletPool;
	    _weapons         = _player.GetComponent<Weapons>();
        CreatePath();
        pickupType = PickupType.BulletVel;
        SetPickupType();
	}


    /// <summary>
    /// Set the pickup type during initialization. This is set by the enemy
    /// </summary>
    private void SetPickupType()
    {
        switch (pickupType)
        {
            case PickupType.FireRate:
                renderer.material.color = Color.grey;
                break;
            case PickupType.BulletVel:
                renderer.material.color = Color.blue;
                break;
            case PickupType.SpeedBoost:
                renderer.material.color = Color.green;
                break;
            case PickupType.BulletDmg:
                renderer.material.color = Color.white;
                break;
            default:
                DebugUtils.Assert(false);
                break;
        }
    }


    /// <summary>
    /// Draws and runs through a path of points
    /// Gradually bouncing up and down, as it moves towards the left side of screen
    /// </summary>
    private void CreatePath()
    {
        _path = new Vector3[30];
        _path[0]  = new Vector3(-1,   1,  0);
        _path[1]  = new Vector3(-2,   0,  0);
        _path[2]  = new Vector3(-3,  -1,  0);
        _path[3]  = new Vector3(-4,   0,  0);
        _path[4]  = new Vector3(-5,   1,  0);
        _path[5]  = new Vector3(-6,   0,  0);
        _path[6]  = new Vector3(-7,  -1,  0);
        _path[7]  = new Vector3(-8,   0,  0);
        _path[8]  = new Vector3(-9,   1,  0);
        _path[9]  = new Vector3(-10,  0,  0);
        _path[10] = new Vector3(-11, -1,  0);
        _path[11] = new Vector3(-12,  0,  0);
        _path[12] = new Vector3(-13,  1,  0);
        _path[13] = new Vector3(-14,  0,  0);
        _path[14] = new Vector3(-15, -1,  0);
        _path[15] = new Vector3(-16,  0,  0);
        _path[16] = new Vector3(-17,  1,  0);
        _path[17] = new Vector3(-18,  0,  0);
        _path[18] = new Vector3(-19, -1,  0);
        _path[19] = new Vector3(-20,  0,  0);
        _path[20] = new Vector3(-21,  1,  0); 
        _path[21] = new Vector3(-22,  0,  0);
        _path[22] = new Vector3(-23, -1,  0);
        _path[23] = new Vector3(-24,  0,  0);
        _path[24] = new Vector3(-25,  1,  0);
        _path[25] = new Vector3(-26,  0,  0);
        _path[26] = new Vector3(-27, -1,  0);
        _path[27] = new Vector3(-28,  0,  0);
        _path[28] = new Vector3(-29,  1,  0);
        _path[29] = new Vector3(-30,  0,  0);

        iTween.MoveTo(gameObject, iTween.Hash("path", _path, "time", 12, "easetype", "linear", "onComplete", "MoveTowardsLeftEdgeOfScreen"));
    }

    private void MoveTowardsLeftEdgeOfScreen()
    {
        rigidbody.velocity = new Vector3(-8, 0, 0);
    }

    /// <summary>
    /// Debug path drawing
    /// </summary>
    public virtual void OnDrawGizmos(){
        iTween.DrawPath(_path);
    }


    /// <summary>
    /// Check for pickup type and set powerup effect 
    /// </summary>
    /// <param name="other">What are we colliding with? Should only check for player</param>
    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.CompareTag("Player")) return;

        switch (pickupType)
        {
            case PickupType.FireRate:
                IncreaseFireRate();
                break;
            case PickupType.BulletVel:
                IncreaseBulletVelocity();
                break;
            case PickupType.SpeedBoost:
                IncreasePlayerSpeed();
                break;
            case PickupType.BulletDmg:
                IncreaseBulletDmg();
                break;
            default:
                DebugUtils.Assert(false);
                break;
        }
        _particleManager.CreatePowerupParticleEffects(_xForm.position);
        _pool.Despawn(transform);
    }



    //------------------------------------------------------------
    //  Power up effects

    private void IncreaseFireRate()
    {
        _weapons.SetFireRate(FIRE_RATE);
        print("setting fire rate to:" + "" + _weapons.GetFireRate());
    }

    private void IncreaseBulletVelocity()
    {
        _weapons.SetBulletVelocity(BULLET_VELOCITY);
        print("setting bullet velocity to:" + "" + _weapons.GetBulletVelocity());
    }

    private void IncreasePlayerSpeed()
    {
        _player.SetPlayerSpeed(PLAYER_SPEED);
        print("setting player speed to:" + "" + _player.GetPlayerSpeed());

    }

    private void IncreaseBulletDmg()
    {
        _weapons.SetBulletDmg(BULLET_DMG);
        print("setting bullet damage to" + "" + _weapons.GetBulletDmg());
    }



}
