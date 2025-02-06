using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.EventSystems.EventTrigger;
using static UnityEngine.GraphicsBuffer;

public class GamblingGun : MonoBehaviour
{
    // Detta var huvudsakligen skrivet av Dennis med några saker tillagda av Eskil, men jag tror att man kan se vem som skrev vad. Ett ganska enkelt sätt är att se om 'camelCasing' eller 'snake_casing' används.
    // Jag (Dennis) kommer dock bara att kommentera min egen kod.

    // System för olika vapen typer...
    [SerializeField] private List<WeaponType> weapons = new List<WeaponType>();
    [SerializeField] private int curWeapon = 0;

    // ...plus system för att byta vapen (som en slotmachine) samt skottet man skjuter.
    [SerializeField] private float rerollDuration = 3f;
    [SerializeField] public TextMeshProUGUI slot1, slot2, slot3, coin;
    [SerializeField] private Vector3 centerOffset;
    [SerializeField] private GameObject bullet;

    public AudioSource sound;
    public AudioSource start_sound;
    public AudioSource roll_sound;
    public AudioSource end_sound;
    private bool roll_audio = false;
    public int coins_amount = 1;

    // Gömda variablar som man inte ställer in i editorn.
    float rerollTime = 0f;
    float time = 0;
    float slotTime1, slotTime2, slotTime3;
    int nextNum = 0;

    // Detta tillåter ett vapen att skjuta flera skott varje gång man skjuter.
    int queuedBullets = 0;
    float queuedBulletsTimer = 0f;

    // Start is called before the first frame update
    void Start()
    {
        coins_amount = 999;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q) && coins_amount > 0)
        {
            if(GameObject.Find("Player").GetComponent<PlayerMovement>().healing <= 0) GameObject.Find("Main Camera").GetComponent<CameraController>().flash_alpha = 0.2f;

            GameObject.Find("Player").GetComponent<PlayerMovement>().healing = 1;

            coin.transform.localScale = new Vector3(0, 0, 0);
            coins_amount--;

            roll_audio = false;
            Roll();
            start_sound.Play();
        }

        queuedBulletsTimer -= Time.deltaTime;
        if(queuedBulletsTimer <= 0f)
        {
            if (Input.GetMouseButton(0) && queuedBullets <= 0)
            {
                // Om det inte finns några fler skott att skjuta så kan man skjuta nya själv.
                sound.Play();

                Shoot();
            }
            else if(queuedBullets > 0)
            {
                // Annars så skjuts skot automatiskt.
                FireBullet();
            }
        }

        if(rerollTime > 0f)
        {
            if(rerollTime <= 2.35f && roll_audio == false)
            {
                roll_sound.Play();
                roll_audio = true;
            }

            rerollTime -= Time.deltaTime;
            if(rerollTime <= 0f)
            {
                end_sound.Play();

                // Efter ett litet tag av snurrande så används vapnet man rullat fram.
                curWeapon = nextNum;
                slot1.text = curWeapon.ToString();
                slot2.text = curWeapon.ToString();
                slot3.text = curWeapon.ToString();
            }
        }

        coin.transform.localScale += (new Vector3(1, 1, 1) - coin.transform.localScale) * 1f * Time.deltaTime;
        coin.text = coins_amount.ToString();

        time -= 10f * Time.deltaTime;

        // Detta kan göras med en for loop, men gjort är gjort.
        // Allt som händer är att ett nummer visas tillsammans med en animation där nummret pressas ner, som om det försöker bromsa hårt.
        if(slotTime1 > 0f)
        {
            slotTime1 = Mathf.Max(slotTime1 - Time.deltaTime * 3f, 0f);
            if(slotTime1 < 1f)
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

        // De olika slotsen stannar vid olika tillfällen för att göra det mer realistiskt.
        slotTime1 = rerollDuration * 3f - 0.5f;
        slotTime2 = rerollDuration * 3f + 0.25f;
        slotTime3 = rerollDuration * 3f + 1f;

        // Och vapen numret väljs i förtid så att alla slots är korrekta.
        nextNum = Random.Range(0, weapons.Count);
    }

    public void Shoot()
    {
        /*public int magSize = 15;
	    public float reload = 8f;
	    public int bulletAmount = 1;
	    public float bulletOffset = 0f;*/

        // Alla skott som ett vapen kan skjuta per gång läggs till.
        // Om det inte är något tidsintervall mellan skotten så skjuts alla samtidigt.
        queuedBullets = weapons[curWeapon].bulletAmount;
        if(weapons[curWeapon].bulletOffset <= 0f)
        {
            for(int i = 0; i < queuedBullets; i++)
            {
                FireBullet();
            }
        }
        else
            FireBullet();
    }

    void FireBullet()
    {
        queuedBullets--;
        // Reload tiden ändras beroende på om det är det sista skottet eller inte.
        if(queuedBullets > 0)
            queuedBulletsTimer = weapons[curWeapon].bulletOffset;
        else
            queuedBulletsTimer = weapons[curWeapon].reload;

        // Eskils kod
        if (GameObject.Find("Player") != null)
        {
            float _h_recoil = (Camera.main.ScreenToWorldPoint(Input.mousePosition).x - GameObject.Find("Player").transform.position.x);

            if (_h_recoil >= 0) _h_recoil = 1;
            else _h_recoil = -1;

            float _v_recoil = (Camera.main.ScreenToWorldPoint(Input.mousePosition).y - GameObject.Find("Player").transform.position.y);

            if (_v_recoil >= 0) _v_recoil = 1;
            else _v_recoil = -1;

            GameObject.Find("Player").GetComponent<PlayerMovement>().h_speed += _h_recoil * -2f * weapons[curWeapon].recoil;
            GameObject.Find("Player").GetComponent<PlayerMovement>().v_speed += _v_recoil * -1f * weapons[curWeapon].recoil;
        }

        // Skjut mot musens position.
        Quaternion rot = Quaternion.Euler(0, 0, (180 / Mathf.PI) * Mathf.Atan2(Camera.main.ScreenToWorldPoint(Input.mousePosition).y - (transform.position.y + centerOffset.y), Camera.main.ScreenToWorldPoint(Input.mousePosition).x - (transform.position.x + centerOffset.x)));
        rot = Quaternion.Euler(0, 0, rot.eulerAngles.z+Random.Range(-weapons[curWeapon].spread, weapons[curWeapon].spread));
        GameObject b = Instantiate(bullet, (transform.position + centerOffset) + transform.right * 2, rot);
        b.transform.localScale = new Vector3(weapons[curWeapon].size+1, weapons[curWeapon].size+1, 1);

        // Ge skottet alla egenskaper som behövs.
        PlayerBullet bul = b.GetComponent<PlayerBullet>();
        bul.speed = weapons[curWeapon].speed + Random.Range(-weapons[curWeapon].speedVariation, weapons[curWeapon].speedVariation);
        bul.drag = weapons[curWeapon].drag;
        bul.lifetime = weapons[curWeapon].lifetime;
        bul.knockback = weapons[curWeapon].knockback;
        bul.damage = weapons[curWeapon].damage;
        bul.hitscan = weapons[curWeapon].hitscan;
        bul.target_scale = weapons[curWeapon].size;
    }
}
