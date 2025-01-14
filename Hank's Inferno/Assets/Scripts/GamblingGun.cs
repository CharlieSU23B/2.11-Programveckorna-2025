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
    float time = 0;
    float slotTime1, slotTime2, slotTime3;
    int nextNum;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
            Roll();

        if (Input.GetMouseButton(0) && time <= 0)
        {
            Shoot();
            time = 2;
        }

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

        time -= 10f * Time.deltaTime;

        if (slotTime1 > 0f)
        {
            slotTime1 = Mathf.Max(slotTime1 - Time.deltaTime * 3f, 0f);
            if (slotTime1 < 1f)
            {
                slot1.transform.localScale = new Vector3(1f + slotTime1, 1f - (slotTime1 * 0.5f), 1f);
                slot1.rectTransform.localPosition = new Vector3(slot1.rectTransform.localPosition.x, -16f * slotTime1, 0f);
                slot1.text = nextNum.ToString();
            }
            else
                slot1.text = Random.Range(0, weapons.Count).ToString();
        }
        if (slotTime2 > 0f)
        {
            slotTime2 = Mathf.Max(slotTime2 - Time.deltaTime * 3f, 0f);
            if (slotTime2 < 1f)
            {
                slot2.transform.localScale = new Vector3(1f + slotTime2, 1f - (slotTime2 * 0.5f), 1f);
                slot2.rectTransform.localPosition = new Vector3(slot2.rectTransform.localPosition.x, -16f * slotTime2, 0f);
                slot2.text = nextNum.ToString();
            }
            else
                slot2.text = Random.Range(0, weapons.Count).ToString();
        }
        if (slotTime3 > 0f)
        {
            slotTime3 = Mathf.Max(slotTime3 - Time.deltaTime * 3f, 0f);
            if (slotTime3 < 1f)
            {
                slot3.transform.localScale = new Vector3(1f + slotTime3, 1f - (slotTime3 * 0.5f), 1f);
                slot3.rectTransform.localPosition = new Vector3(slot3.rectTransform.localPosition.x, -16f * slotTime3, 0f);
                slot3.text = nextNum.ToString();
            }
            else
                slot3.text = Random.Range(0, weapons.Count).ToString();
        }
    }

    public void Roll()
    {
        rerollTime = rerollDuration;
        /*slot1.text = "7";
        slot2.text = "7";
        slot3.text = "7";*/

        slotTime1 = rerollDuration * 3f - 0.5f;
        slotTime2 = rerollDuration * 3f + 0.25f;
        slotTime3 = rerollDuration * 3f + 1f;

        nextNum = Random.Range(0, weapons.Count);
    }

    public void Shoot()
    {
        /*public int magSize = 15;
	    public float reload = 8f;
	    public int bulletAmount = 1;
	    public float bulletOffset = 0f;
	    public float recoil = 5f;*/

        if(GameObject.Find("Player") != null)
        {
            float _h_recoil = (Camera.main.ScreenToWorldPoint(Input.mousePosition).x - GameObject.Find("Player").transform.position.x);

            if (_h_recoil >= 0) _h_recoil = 1;
            else _h_recoil = -1;

            float _v_recoil = (Camera.main.ScreenToWorldPoint(Input.mousePosition).y - GameObject.Find("Player").transform.position.y);

            if (_v_recoil >= 0) _v_recoil = 1;
            else _v_recoil = -1;

            GameObject.Find("Player").GetComponent<PlayerMovement>().h_speed += _h_recoil * -8f;
            GameObject.Find("Player").GetComponent<PlayerMovement>().v_speed += _v_recoil * -4f;
        }

        Quaternion rot = Quaternion.Euler(0, 0, (180 / Mathf.PI) * Mathf.Atan2(Camera.main.ScreenToWorldPoint(Input.mousePosition).y - (transform.position.y + centerOffset.y), Camera.main.ScreenToWorldPoint(Input.mousePosition).x - (transform.position.x + centerOffset.x)));
        //rot = Quaternion.EulerAngles(0, 0, rot.eulerAngles.z + Random.Range(-weapons[curWeapon].spread, weapons[curWeapon].spread));
        GameObject b = Instantiate(bullet, (transform.position + centerOffset) + transform.right * 2, rot);
        b.transform.localScale = new Vector3(weapons[curWeapon].size, weapons[curWeapon].size, 1);

        PlayerBullet bul = b.GetComponent<PlayerBullet>();
        bul.speed = weapons[curWeapon].speed + Random.Range(-weapons[curWeapon].speedVariation, weapons[curWeapon].speedVariation);
        bul.drag = weapons[curWeapon].drag;
        bul.lifetime = weapons[curWeapon].lifetime;
        bul.knockback = weapons[curWeapon].knockback;
        bul.damage = weapons[curWeapon].damage;
        bul.hitscan = weapons[curWeapon].hitscan;
    }
}
