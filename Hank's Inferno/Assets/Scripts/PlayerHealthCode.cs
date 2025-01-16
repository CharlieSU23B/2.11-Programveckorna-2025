using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerHealthCode : MonoBehaviour
{
    // Initializing variables
    public Image[] sprites = new Image[5];
    public float[] scale = new float[5];
    public int hp = 3;
    public int max_hp = 3;
    public Sprite sprite_1;
    public Sprite sprite_2;

    // Start is called before the first frame update
    void Start()
    {
        hp = 3;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        hp = Mathf.Clamp(hp, 0, max_hp);
        max_hp = Mathf.Clamp(max_hp, 1, 5);

        for (var _i = 0; _i < sprites.Length; _i++)
        {
            scale[_i] += (1 - scale[_i]) * 10f * Time.deltaTime;

            sprites[_i].transform.localScale = new Vector3(scale[_i], scale[_i], scale[_i]);

            if (_i < hp)
            {
                sprites[_i].sprite = sprite_2;
            }
            else
            {
                sprites[_i].sprite = sprite_1;
            }

            if (_i >= max_hp) sprites[_i].enabled = false;
            else sprites[_i].enabled = true;
        }

        if (hp <= 0)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}
