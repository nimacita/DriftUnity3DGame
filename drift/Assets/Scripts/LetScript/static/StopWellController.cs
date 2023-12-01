using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopWellController : MonoBehaviour
{

    private GameObject flash_lights;
    public GameObject body_well, well_cover;
    protected float flash_speed;
    protected bool anim_value;
    private Color em_light_all_time, em_light_active;

    public Material[] well_colors;


    void Start()
    {
        AllStartValues();
        ChangeWellColor();
    }

    private void AllStartValues()
    {
        flash_lights = transform.Find("Flashlight").gameObject;

        em_light_all_time = new Color(0.235f, 0.0431f,0f);
        em_light_active = new Color(0.941f, 0.172f,0f);

        anim_value = false;
        flash_speed = 0.5f;
        InvokeRepeating("Flashing", flash_speed, flash_speed);
    }

    private void Flashing()//мигание
    {
        anim_value = !anim_value;
        if (anim_value)
        {
            ChangeEmissionColor(flash_lights.GetComponent<Renderer>().material, em_light_active);
        }
        else
        {
            ChangeEmissionColor(flash_lights.GetComponent<Renderer>().material, em_light_all_time);
        }
    }

    private void ChangeWellColor()//смена цвета
    {
        int rand_color = Random.Range(0, well_colors.Length);
        body_well.GetComponent<Renderer>().sharedMaterial = well_colors[rand_color];
        well_cover.GetComponent<Renderer>().sharedMaterial = well_colors[rand_color];
    }

    public void ReSpawn()
    {
        ChangeWellColor();
    }

    protected void ChangeEmissionColor(Material mat, Color col)//меняем цвет свечения
    {
        mat.SetColor("_EmissionColor", col);
    }
}
