using UnityEngine;
using System.Collections;

public class Weapons : MonoBehaviour {
    public enum WeaponType {
        SingleShot,
        SpreadWeapon
    }
    private bool[]    _weaponInventory;
    public WeaponType currentWeapon = WeaponType.SingleShot;
 

    private void  Start ()
    {
        // Create an inventory, and store the first weapon in there
        _weaponInventory = new bool[ System.Enum.GetValues(typeof (WeaponType)).Length];
    }
 
    public void  SwitchToWeapon ( int weapon  ){
        if (_weaponInventory[weapon]) 
        {
           // animate putting currentWeapon away
           currentWeapon = (WeaponType) weapon;
           // animate pulling out currentWeapon
        }
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
}