using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DayOrNightController : MonoBehaviour
{
    public MainUiController main_ui;
    public GameObject main_camera;
    public GameObject photo_camera;
    public GameObject sun;
    public GameObject moon;
    private Light sun_light, moon_light;
    public GameObject rain;
    private ParticleSystem rain_particles;
    public GameObject thunder;
    private ParticleSystem thunder_particles;
    private Camera camera_component, photo_cam_comp;

    //skyboxes
    public Material day_skybox, night_skybox;

    //day and night
    [SerializeField]
    private int time_of_day_count, time_of_night_count;
    private int max_time_of_day = 8;
    private int max_time_of_night = 6;
    private bool is_day;

    //sun and moon
    private float sun_angle;
    private float max_angle, min_angle, angle_koef;
    private float sun_usual_intensity, sun_rain_intensity, sun_fog_intensity;
    private float moon_usual_intensity;
    private float intensity_koef;
    private float sun_thunder_intensity, moon_thunder_intensity;

    //raind and fog
    private bool is_rain, is_fog;
    private int rain_fog_dist, fog_dist,usual_fog_dist;
    private int rain_chance,fog_chance;
    private float rain_min_time, rain_max_time;
    private float fog_min_time, fog_max_time;
    [SerializeField]
    private float rain_time;
    [SerializeField]
    private float fog_time;
    [SerializeField]
    private float weather_cd;
    private float max_weather_cd;
    private bool is_weather;
    private bool is_anim;

    //thunder
    [SerializeField]
    private bool is_thunder;

    //fog_stats
    public Color day_rain_fog, night_rain_fog;
    public Color day_fog, night_fog;
    public Color usual_day_fog, usual_night_fog;
    [SerializeField]
    private int f_dist;
    private Color f_color;
    private int fog_claer_koef;

    //asphalt
    public Material dry_asphalt_mat, wet_asphalt_mat, asphalt;
    private Color dry_asphalt, wet_asphalt,current_asphalt;

    void Start()
    {
        AllStartValues();
        PrefsValues();
        IsWeather();
        DefineTimeOfDay();
        InvokeRepeating("IsRainAndFog", 0f,10f);

        DynamicGI.UpdateEnvironment();
    }

    private void AllStartValues()//старт значения
    {
        //components
        rain_particles = rain.GetComponent<ParticleSystem>();
        thunder_particles = thunder.GetComponent<ParticleSystem>();
        camera_component = main_camera.GetComponent<Camera>();
        photo_cam_comp = photo_camera.GetComponent<Camera>();
        sun_light = sun.GetComponent<Light>();
        moon_light = moon.GetComponent<Light>();
        rain.SetActive(false);
        thunder.SetActive(false);

        //angles
        max_angle = 170f;
        angle_koef = 23f;
        min_angle = 10f;

        //sun intensity
        sun_usual_intensity = sun_light.intensity;
        moon_usual_intensity = moon_light.intensity;
        sun_rain_intensity = 0.3f;
        sun_fog_intensity = 0.3f;
        intensity_koef = 0.25f;
        moon_thunder_intensity = 0.45f;
        sun_thunder_intensity = 0.5f;

        //all weather settings
        rain_chance = 55;//55
        fog_chance = 60;//60
        rain_min_time = 30f;//30
        rain_max_time = 180f;//120
        fog_min_time = 30f;//30
        fog_max_time = 90f;//100
        max_weather_cd = 90f;//90
        is_weather = true;
        is_anim = true;

        //thunder
        is_thunder = false;

        //fog settings
        rain_fog_dist = 100;//100
        fog_dist = 60;//60
        usual_fog_dist = 120;//140
        fog_claer_koef = 45;//70

        //particle settings
        rain_particles.Stop();
        thunder_particles.Stop();

        //asphalt materials
        dry_asphalt = dry_asphalt_mat.color;
        wet_asphalt = wet_asphalt_mat.color;
    }

    private void PrefsValues()//значения префс
    {
        if (PlayerPrefs.HasKey("time_of_day_count"))
        {
            time_of_day_count = PlayerPrefs.GetInt("time_of_day_count");
        }
        else
        {
            HasntPrefs();
        }

        if (PlayerPrefs.HasKey("time_of_night_count"))
        {
            time_of_night_count = PlayerPrefs.GetInt("time_of_night_count");
        }
        else
        {
            HasntPrefs();
        }

        if (PlayerPrefs.HasKey("is_day"))
        {
            IsDay();
        }
        else
        {
            HasntPrefs();
        }

        if (PlayerPrefs.HasKey("rain_time"))
        {
            rain_time = PlayerPrefs.GetFloat("rain_time");
            if (rain_time < 1f)
            {
                rain_time = 0f;
            }
        }
        else
        {
            HasntPrefs();
        }

        if (PlayerPrefs.HasKey("fog_time"))
        {
            fog_time = PlayerPrefs.GetFloat("fog_time");
            if (fog_time < 1f)
            {
                fog_time = 0f;
            }
        }
        else
        {
            HasntPrefs();
        }

        if (PlayerPrefs.HasKey("weather_cd"))
        {
            weather_cd = PlayerPrefs.GetFloat("weather_cd");
        }
        else
        {
            HasntPrefs();
        }

        if (rain_time > 0f)
        {
            f_dist = rain_fog_dist;
            current_asphalt = wet_asphalt;
        }
        else if (fog_time > 0f)
        {
            f_dist = fog_dist;
            current_asphalt = dry_asphalt;
        }
        else
        {
            f_dist = usual_fog_dist;
            current_asphalt = dry_asphalt;
        }

    }

    private void HasntPrefs()//если cохранения нет нет, то первое значение
    {
        PlayerPrefs.SetInt("time_of_day_count", 2);
        time_of_day_count = 2;
        PlayerPrefs.SetInt("time_of_night_count", 0);
        time_of_night_count = 0;
        PlayerPrefs.SetFloat("rain_time", 0f);
        rain_time = 0f;
        PlayerPrefs.SetFloat("fog_time", 0f);
        fog_time = 0f;
        PlayerPrefs.SetFloat("weather_cd",0f);
        weather_cd = 0f;
        PlayerPrefs.SetInt("is_day", 1);
        IsDay();
    }

    private void FixedUpdate()
    {
        RainTime();
        FogTime();
        IsWeather();
        FogSettings();
        IntensitySettings();
        BackGroundSettings();
        ChangeFogRenderSettings();
        ChangeMaterial();
    }

    private void IsDay()//определяем переменную is_day
    {
        if (PlayerPrefs.GetInt("is_day") > 0)
        {
            is_day = true;
        }
        else
        {
            is_day = false;
        }
    }

    private void DefineTimeOfDay()//смотрим день или ночь
    {
        camera_component.clearFlags = CameraClearFlags.SolidColor;
        if (is_day)
        {
            DayTime();
        }
        else
        {
            NightTime();
            sun_angle = min_angle;
        }
    }

    private void DayTime()//ставим день
    {
        sun.SetActive(true);
        moon.SetActive(false);
        sun_angle = ChangeAngle(sun_angle);
        sun.transform.localRotation = Quaternion.Euler(sun_angle, 0f, 0f);

        if (!is_fog && !is_rain) 
        {
            ChangeRenderSettings(sun.GetComponent<Light>(), day_skybox);
        }
    }

    private void NightTime()//ставим ночь
    {
        sun.SetActive(false);
        moon.SetActive(true);

        if (!is_fog && !is_rain)
        {
            ChangeRenderSettings(moon.GetComponent<Light>(), night_skybox);
        }
    }

    private void FogSettings()//настройка значений тумана
    {
        if (!is_fog && !is_rain)
        {
            if (f_dist < usual_fog_dist)
            {
                f_dist += Mathf.RoundToInt(Time.deltaTime * fog_claer_koef);
            }
            else
            {
                f_dist = usual_fog_dist;
            }
            if (is_day)
            {
                f_color = usual_day_fog;
            }
            else
            {
                f_color = usual_night_fog;
            }
        }

        if (is_rain) {
            if (f_dist > rain_fog_dist)
            {
                f_dist -= Mathf.RoundToInt(Time.deltaTime * fog_claer_koef);
            }
            else
            {
                f_dist = rain_fog_dist;
            }
        }

        if (is_fog) {
            if (f_dist > fog_dist)
            {
                f_dist -= Mathf.RoundToInt(Time.deltaTime * fog_claer_koef);
            }
            else
            {
                f_dist = fog_dist;
            }
        }
    }

    private void IntensitySettings()//настройка значений интенсивности солнца
    {
        if (is_day)
        {
            if (!is_thunder) {
                if (is_rain)
                {
                    if (sun_light.intensity > sun_rain_intensity)
                    {
                        sun_light.intensity -= Time.deltaTime * intensity_koef;
                    }
                }
                else if (is_fog)
                {
                    if (sun_light.intensity > sun_fog_intensity)
                    {
                        sun_light.intensity -= Time.deltaTime * intensity_koef;
                    }
                }
                else
                {
                    if (sun_light.intensity < sun_usual_intensity)
                    {
                        sun_light.intensity += Time.deltaTime * intensity_koef;
                    }
                }
            }
            else
            {
                sun_light.intensity = sun_thunder_intensity;
            }
        }
        else
        {
            if (!is_thunder)
            {
                if (moon_light.intensity > moon_usual_intensity)
                {
                    moon_light.intensity -= Time.deltaTime * intensity_koef;
                }
            }
            else
            {
                moon_light.intensity = moon_thunder_intensity;
            }
        }
    }

    private void BackGroundSettings()
    {
        if (is_rain || is_fog)
        {
            camera_component.clearFlags = CameraClearFlags.SolidColor;
            photo_cam_comp.clearFlags = CameraClearFlags.SolidColor;
        }
        else
        {
            camera_component.clearFlags = CameraClearFlags.Skybox;
            photo_cam_comp.clearFlags = CameraClearFlags.Skybox;
        }
    }

    private void IsWeather()//можно ли ставить погоду
    {
        if (weather_cd > 0f)
        {
            if (!main_ui.GetIsPause() && !main_ui.GetIsRestart()) 
            {
                weather_cd -= Time.deltaTime;
            }

            is_weather = false;
        }
        else
        {
            weather_cd = 0f;
            is_weather = true;
        }
    }

    private void IsRainAndFog()//определяем пойдет ли дождь туман или гроза
    {
        if (rain_time <= 0f && fog_time<=0f && !main_ui.GetIsPause() && !main_ui.GetIsRestart() && is_weather)
        {
            float rand_rain = Random.Range(1, rain_chance + 1);
            if (rand_rain == rain_chance)
            {
                rain_time = Random.Range(rain_min_time, rain_max_time);
            }
            float rand_fog = Random.Range(1, fog_chance + 1);
            if (rand_fog == fog_chance)
            {
                fog_time = Random.Range(fog_min_time, fog_max_time);
            }
        }

    }

    private void RainTime()//устанавливаем дождь
    {

        if (rain_time > 0f)
        {
            is_rain = true;
            if (!main_ui.GetIsPause() && !main_ui.GetIsRestart()) {
                rain_time -= Time.deltaTime;
            }
        }
        else
        {
            is_rain = false;
            rain_time = 0f;
        }

        if (is_rain)
        {
            IsThunder();
            if (weather_cd <= 0)
            {
                weather_cd += max_weather_cd + rain_time;
            }

            if (is_day)
            {
                camera_component.backgroundColor = day_rain_fog;
                photo_cam_comp.backgroundColor = day_rain_fog;
                f_color = day_rain_fog;
            }
            else
            {
                camera_component.backgroundColor = night_rain_fog;
                photo_cam_comp.backgroundColor = night_rain_fog;
                f_color = night_rain_fog;
            }
            if (is_anim) {
                rain.SetActive(true);
                rain_particles.Play();
                thunder.SetActive(true);
                thunder_particles.Play();
                is_anim = false;
            }

        }
        else if(!is_fog)
        {
            rain_particles.Stop();
            rain.SetActive(false);
            thunder_particles.Stop();
            thunder.SetActive(false);
            is_anim = true;
        }
    }

    private void FogTime()//устанавливаем туман
    {
        if (fog_time > 0f)
        {
            is_fog = true;
            if (!main_ui.GetIsPause() && !main_ui.GetIsRestart())
            {
                fog_time -= Time.deltaTime;
            }
        }
        else
        {
            is_fog = false;
            fog_time = 0f;
        }

        if (is_fog)
        {

            if (weather_cd <= 0)
            {
                weather_cd += max_weather_cd + fog_time;
            }

            if (is_day)
            {
                camera_component.backgroundColor = day_fog;
                photo_cam_comp.backgroundColor = day_fog;
                f_color = day_fog;
            }
            else
            {
                camera_component.backgroundColor = night_fog;
                photo_cam_comp.backgroundColor = night_fog;
                f_color = night_fog;
            }

        }
    }

    private void IsThunder()
    {
        if (thunder_particles.particleCount > 0)
        {
            is_thunder = true;
        }
        else
        {
            is_thunder = false;
        }
    }

    private void ChangeMaterial()//меняем матерьял асфальта
    {
        if (is_rain)
        {
            ToWetAsphalt();
        }
        else
        {
            ToDryAsphalt();
        }
        asphalt.color = current_asphalt;
    }

    private void ToWetAsphalt()//делаем цвет темнее
    {
        if (current_asphalt.r > wet_asphalt.r)
        {
            current_asphalt.r -= Time.deltaTime / 5f;
        }
        else
        {
            current_asphalt.r = wet_asphalt.r;
        }
        if (current_asphalt.g > wet_asphalt.g)
        {
            current_asphalt.g -= Time.deltaTime / 5f;
        }
        else
        {
            current_asphalt.g = wet_asphalt.g;
        }
        if (current_asphalt.b > wet_asphalt.b)
        {
            current_asphalt.b -= Time.deltaTime / 5f;
        }
        else
        {
            current_asphalt.b = wet_asphalt.b;
        }
    }

    private void ToDryAsphalt()//деламем цвет светлее
    {
        if (current_asphalt.r < dry_asphalt.r)
        {
            current_asphalt.r += Time.deltaTime / 5f;
        }
        else
        {
            current_asphalt.r = dry_asphalt.r;
        }
        if (current_asphalt.g < dry_asphalt.g)
        {
            current_asphalt.g += Time.deltaTime / 5f;
        }
        else
        {
            current_asphalt.g = dry_asphalt.g;
        }
        if (current_asphalt.b < dry_asphalt.b)
        {
            current_asphalt.b += Time.deltaTime / 5f;
        }
        else
        {
            current_asphalt.b = dry_asphalt.b;
        }
    }

    private float ChangeAngle(float angle)//меняем значение угла
    {
        if (angle < max_angle)
        {
            angle = min_angle + angle_koef * time_of_day_count;
        }
        return angle;
    }

    private void ChangeRenderSettings(Light sun, Material skybox)//меняем настройки рендера света
    {
        RenderSettings.skybox = skybox;
        RenderSettings.sun = sun;
        DynamicGI.UpdateEnvironment();
    }

    private void ChangeFogRenderSettings()//меняем настройки тумана
    {
        RenderSettings.fogColor = f_color;
        RenderSettings.fogEndDistance = f_dist;
        RenderSettings.fog = true;
    }

    public void CountTimeOfDay()//прибавляем и считаем нынешнее время суток и количество (запускается при рестарте)
    {
        if (is_day)
        {
            time_of_day_count++;
            if (time_of_day_count >= max_time_of_day)
            {
                is_day = false;
                PlayerPrefs.SetInt("is_day", 0);
                time_of_day_count = 0;
            }
            PlayerPrefs.SetInt("time_of_day_count", time_of_day_count);
        }
        else
        {
            time_of_night_count++;
            if (time_of_night_count >= max_time_of_night)
            {
                is_day = true;
                PlayerPrefs.SetInt("is_day", 1);
                time_of_night_count = 0;
            }
            PlayerPrefs.SetInt("time_of_night_count", time_of_night_count);
        }

        PlayerPrefs.SetFloat("rain_time", rain_time);
        PlayerPrefs.SetFloat("fog_time", fog_time);
        PlayerPrefs.SetFloat("weather_cd", weather_cd);
    }

    public bool GetIsDay()
    {
        return is_day;
    }

    public int GetMaxTimeOfDay()
    {
        return max_time_of_day;
    }

    public int GetTimeOfDay()
    {
        return time_of_day_count;
    }

    public bool GetIsRain()
    {
        return is_rain;
    }

    public bool GetIsFog()
    {
        return is_fog;
    }

}
