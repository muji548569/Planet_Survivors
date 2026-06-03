using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    private List<WeaponBase> weapons = new List<WeaponBase>();
    
    void Update()
    {
        foreach (var weapon in weapons)
        {
            weapon.Tick(Time.deltaTime);
        }
    }

    public void AddWeapon(WeaponBase weapon)
    {
        weapons.Add(weapon);
    }
}
