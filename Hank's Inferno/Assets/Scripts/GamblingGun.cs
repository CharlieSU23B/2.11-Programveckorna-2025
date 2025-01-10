using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamblingGun : MonoBehaviour
{
    public List<WeaponType> weapons = new List<WeaponType>();
    public int curWeapon = 0;
    [SerializeField] private float rerollDuration = 3f;

    float rerollTime = 0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
