using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    [SerializeField] private WeaponData[] startWeapons;
    [SerializeField] private Transform planet;
    private List<WeaponBase> weapons = new List<WeaponBase>();

    private void Start()
    {
        foreach (WeaponData data in startWeapons)
        {
            if (data == null) continue; 
            AddWeapon(WeaponFactory.Create(data, transform, planet));
        }
    }

    void Update()
    {
        foreach (WeaponBase weapon in weapons)
        {
            weapon.Tick(Time.deltaTime);
        }
    }

    public void AddWeapon(WeaponBase weapon)
    {
        if (weapon == null) return;
        weapons.Add(weapon);
    }
}
