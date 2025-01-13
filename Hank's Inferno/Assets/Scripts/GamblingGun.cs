using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.EventSystems.EventTrigger;
using static UnityEngine.GraphicsBuffer;

public class GamblingGun : MonoBehaviour
{
    public List<WeaponType> weapons = new List<WeaponType>();
    public int curWeapon = 0;
    [SerializeField] private float rerollDuration = 3f;
    [SerializeField] private TextMeshProUGUI slot1, slot2, slot3;
    [SerializeField] private Vector3 centerOffset;
    [SerializeField] private GameObject bullet;

    float rerollTime = 0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
            Roll();

        if (Input.GetMouseButtonDown(0))
            Shoot();

        if(rerollTime > 0f)
        {
            rerollTime -= Time.deltaTime;
            if(rerollTime <= 0f)
            {
                curWeapon = Random.Range(0, weapons.Count);
                slot1.text = curWeapon.ToString();
                slot2.text = curWeapon.ToString();
                slot3.text = curWeapon.ToString();
            }
        }
    }

    public void Roll()
    {
        rerollTime = rerollDuration;
        slot1.text = "";
        slot2.text = "";
        slot3.text = "";
    }

    public void Shoot()
    {
        /*public int magSize = 15;
	    public float reload = 8f;
	    public int bulletAmount = 1;
	    public float bulletOffset = 0f;
	    public float recoil = 5f;*/

        Quaternion rot = Quaternion.Euler(0, 0, (180 / Mathf.PI) * Mathf.Atan2(Camera.main.ScreenToWorldPoint(Input.mousePosition).y - (transform.position.y + centerOffset.y), Camera.main.ScreenToWorldPoint(Input.mousePosition).x - (transform.position.x + centerOffset.x)));
        //rot = Quaternion.EulerAngles(0, 0, rot.eulerAngles.z + Random.Range(-weapons[curWeapon].spread, weapons[curWeapon].spread));
        GameObject b = Instantiate(bullet, transform.position + centerOffset, rot);
        b.transform.localScale = new Vector3(weapons[curWeapon].size, weapons[curWeapon].size, 1);
        b.GetComponent<PlayerBullet>().rot = rot;

        PlayerBullet bul = b.GetComponent<PlayerBullet>();
        bul.speed = weapons[curWeapon].speed + Random.Range(-weapons[curWeapon].speedVariation, weapons[curWeapon].speedVariation);
        bul.drag = weapons[curWeapon].drag;
        bul.lifetime = weapons[curWeapon].lifetime;
        bul.knockback = weapons[curWeapon].knockback;
        bul.damage = weapons[curWeapon].damage;
        bul.hitscan = weapons[curWeapon].hitscan;
    }
}
