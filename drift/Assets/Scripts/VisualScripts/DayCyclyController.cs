using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayCyclyController : MonoBehaviour
{

    protected PlayerController player_controller;

    [Range(0,1)][SerializeField]
    private float time_of_day;
    private float day_duration;

    public GameObject sun;
    public AnimationCurve sun_curve;
    public GameObject moon;
    public AnimationCurve moon_curve;

    public Material day_skybox,night_skybox;
    public AnimationCurve skybox_curve;

    private float sun_intensity;
    private float moon_intensity;

    void Start()
    {
        AllStartVAlues();
    }

    private void AllStartVAlues()//начальные значения
    {
        player_controller = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();

        sun.SetActive(true);
        moon.SetActive(true);

        day_duration = 120f;

        moon_intensity = moon.GetComponent<Light>().intensity;
        sun_intensity = sun.GetComponent<Light>().intensity;

        PrefsValues();
        SunRot();
        MoonRot();
    }

    private void PrefsValues()//сохраненые значения
    {
        if (PlayerPrefs.HasKey("time_of_day"))
        {
            time_of_day = PlayerPrefs.GetFloat("time_of_day");
        }
        else
        {
            PlayerPrefs.SetFloat("time_of_day", 0f);
            time_of_day = 0f;
        }

    }

    
    void FixedUpdate()
    {
        CountDayTime();
        SunRot();
        MoonRot();
        ChangeSkyBox();
    }

    private void CountDayTime()//считаем время дня
    {
        if (!player_controller.GetIsPause()) {
            time_of_day += Time.deltaTime / day_duration;
            if (time_of_day >= 1f)
            {
                time_of_day -= 1f;
            }
            PlayerPrefs.SetFloat("time_of_day", time_of_day);
        }
    }

    private void SunRot()//поворачиваем солнце
    {
        sun.transform.localRotation = Quaternion.Euler(time_of_day * 360f, 0f, 0f);
        sun.GetComponent<Light>().intensity = sun_intensity * sun_curve.Evaluate(time_of_day);
    }

    private void MoonRot()//поворачиваем луну
    {
        moon.transform.localRotation = Quaternion.Euler(time_of_day * 360f + 180f, 0f, 0f);
        moon.GetComponent<Light>().intensity = moon_intensity * moon_curve.Evaluate(time_of_day);
    }

    private void ChangeSkyBox()//меняем скайбокс
    {
        RenderSettings.skybox.Lerp(night_skybox, day_skybox, skybox_curve.Evaluate(time_of_day));
        RenderSettings.sun = skybox_curve.Evaluate(time_of_day) > 0.1f ? sun.GetComponent<Light>() : moon.GetComponent<Light>();
        DynamicGI.UpdateEnvironment();
    }
}
