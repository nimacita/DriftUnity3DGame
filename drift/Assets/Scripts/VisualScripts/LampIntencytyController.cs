using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LampIntencytyController : MonoBehaviour
{

    [SerializeField][Range(0,5)]
    private float max_intencyty;

    private Light this_light;

    void Start()
    {
        AllStartValues();
        DefineIntencyty();
    }

    private void AllStartValues()
    {
        this_light = GetComponent<Light>();
    }

    private void DefineIntencyty()//начальные значения интенсивности фонарей
    {
        if (PlayerPrefs.GetInt("is_day") > 0)
        {
            this_light.intensity = 0f;
        }
        else
        {
            this_light.intensity = max_intencyty;
        }
    }

}
