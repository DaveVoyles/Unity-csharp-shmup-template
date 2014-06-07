using PathologicalGames;
using UnityEngine;
using System.Collections;

public class Weapons : MonoBehaviour {
    public enum WeaponType {
        SingleShot,
        SpreadWeapon
    }
    private bool[]     _weaponInventory;
    private Player     _player;
    private SpawnPool  _pool                  = null;
    private float      _bulletVelX            = 40f;
    private int        _bulletDmg             = 1;
    private const int  SPREAD_WEAPON_OFFSET_Y = 10;
    private const float DEFAULT_BULLET_VEL_X  = 40f;
    private const int   DEFAULT_BULLET_DMG    = 1;
    private const float DEFAULT_FIRE_RATE     = 0.035f;
    private float       _fireRate             = 0.035f;     // time between shots

    public Transform playerBulletPrefab;
    public Transform playerMissilePrefab;
    public AudioClip sfxShoot;
    public WeaponType currentWeapon = WeaponType.SingleShot;


    private void  Start ()
    {
        // Create an inventory, and store the first weapon in there
        _weaponInventory = new bool[ System.Enum.GetValues(typeof (WeaponType)).Length];
        _player          = gameObject.GetComponent<Player>();
        _pool            = GameObject.Find("GameManager").GetComponent<GameManager>().BulletPool;
    }
 
    public void  SwitchToWeapon ( int weapon  ){
        if (_weaponInventory[weapon]) 
        {
           // animate putting currentWeapon away
           currentWeapon = (WeaponType) weapon;
           // animate pulling out currentWeapon
        }
    }

    public void SwitchToNextWeapon()
    {
        currentWeapon++;
     
        //Going past the length of the array, so return to 0
        if (currentWeapon == (WeaponType) System.Enum.GetValues(typeof (WeaponType)).Length)
        {
            currentWeapon = 0;
        }
        print(currentWeapon);
    }

    public void  PickupWeapon ( int weapon  )
    {
        _weaponInventory[weapon] = true;
        SwitchToWeapon(weapon); 
    }
 

    public void DropWeapon ( WeaponType weapon  )
    {
        if (currentWeapon == weapon) {
           var nextWeaponIndex = (int) (weapon + 1);
            //Going past the length of the array, so return to 0
           if (nextWeaponIndex >= System.Enum.GetValues(typeof (WeaponType)).Length) {
             nextWeaponIndex = 0;
           }
           SwitchToWeapon(nextWeaponIndex);
        }
        _weaponInventory[(int) weapon] = false;
    }

    public void ShootWeapon()
    {
        if (currentWeapon == WeaponType.SingleShot)
        {
            ShootSingleShot();   
        }
        if (currentWeapon == WeaponType.SpreadWeapon)
        {
            ShootSpreadWeapon();
        }
    }

    /// <summary>
    /// Shoots one row of bullets in a straight line
    /// Grabs current instance of bullet, by retrieving bullet prefab from spawn pool
    /// </summary>
    public void ShootSingleShot()
    {
        var bulletInst = _pool.Spawn(playerBulletPrefab, _player.xform.position, Quaternion.identity);
        bulletInst.rigidbody.velocity = new Vector3(_bulletVelX, 0, _player.xform.position.z);

        // _soundManager.PlayClip(sfxShoot, false);                      // play shooting SFX
    }

    /// <summary>
    /// Shoots three bullets at once, like the spread weapon in Contra.
    /// Grabs current instance of bullet, by retrieving bullet prefab from spawn pool
    /// </summary>
    public void ShootSpreadWeapon()
    {
        var bulletInst = _pool.Spawn(playerBulletPrefab, _player.xform.position, Quaternion.identity);
        bulletInst.rigidbody.velocity = new Vector3(_bulletVelX, 0 - SPREAD_WEAPON_OFFSET_Y, _player.xform.position.z);

        var bulletInst2 = _pool.Spawn(playerBulletPrefab, _player.xform.position, Quaternion.identity);
        bulletInst2.rigidbody.velocity = new Vector3(_bulletVelX, 0, _player.xform.position.z);

        var bulletInst3 = _pool.Spawn(playerBulletPrefab, _player.xform.position, Quaternion.identity);
        bulletInst3.rigidbody.velocity = new Vector3(_bulletVelX, 0 + SPREAD_WEAPON_OFFSET_Y, _player.xform.position.z);

        // _soundManager.PlayClip(sfxShoot, false);                      // play shooting SFX
    }


    //-------------------------------------------------
    // POWER-UPS    
    //------------------------------------------------
    public float GetFireRate()
    {
        return _fireRate;
    }

    public void SetFireRate(float fireRate)
    {
        _fireRate = fireRate;
    }

    public void SetDefaultFireRate()
    {
        _fireRate = DEFAULT_FIRE_RATE;
    }

    public float GetBulletVelocity()
    {
        return _bulletVelX;
    }

    public void SetBulletVelocity(float bulletVelocity)
    {
        _bulletVelX = bulletVelocity;
    }

    public void SetDefaultBulletVel()
    {
        _bulletVelX = DEFAULT_BULLET_VEL_X;
    }

    public int GetBulletDmg()
    {
        return _bulletDmg;
    }

    public void SetBulletDmg(int bulletDmg)
    {
        _bulletDmg = bulletDmg;
    }

    public void SetDefaultBulletDmg()
    {
        _bulletDmg = DEFAULT_BULLET_DMG;
    }



    
}