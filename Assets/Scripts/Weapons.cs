using UnityEngine;
using System.Collections;

public class Weapons : MonoBehaviour {
    public enum WeaponType {
        SingleShot,
        SpreadWeapon
    }
    private bool[]    _weaponInventory;
    public WeaponType currentWeapon = WeaponType.SingleShot;
    private Player _player;

    private void  Start ()
    {
        // Create an inventory, and store the first weapon in there
        _weaponInventory = new bool[ System.Enum.GetValues(typeof (WeaponType)).Length];
        _player = gameObject.GetComponent<Player>();
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
         _player.ShootSingleShot();   
        }
        if (currentWeapon == WeaponType.SpreadWeapon)
        {
            _player.ShootSpreadWeapon();
        }
    }
}