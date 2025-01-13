using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Weapon", menuName = "Weapon")]
public class WeaponType : ScriptableObject
{
    public string name = "Weapon";
	public float damage = 1f;
	public float speed = 16f;
	public bool hitscan = false;
	public float spread = 3f;
	public float speedVariation = 0f;
	public int magSize = 15;
	public float reload = 8f;
	public int bulletAmount = 1;
	public float bulletOffset = 0f;
	public float drag = 1f;
	public float lifetime = 60f;
	public float size = 4f;
	public float recoil = 5f;
	public float knockback = 7f;
}
