using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChooseModelStat : MonoBehaviour
{
    private PlayerController player_controller;
    private BtnCarController btn_car_controller;
    
    public GameObject bmwE30;
    public GameObject vwGolf2;
    public GameObject skygtr34;

    private string car_define;

    //игрока
    private Vector3 pref_sise;//рамзер машины
    private float value_pref_sise;//рамзер машины
    private float min_speed, max_speed;//минимальная и максимальная скорость
    private float speed_koef, speed_down_koef;//множитель набирания и потери скорости
    private float max_end_drift_speed;//максимальное ускорение при выходе из поворота
    private float rot_to_speed_koef; // увеличиная скорость поворота в отношении к скорости
    private float br_factor;//фактор скорости набирания заноса
    private float max_body_rot_speed;//максимальная скорость поворота корпуса
    private float start_body_rot_value;//скорость при которой начинается поворот корпуса
    private float start_extension_speed;//скорость выпрямления корпуса после заноса
    private float extension_acceleration;//ускорение скорости выпрямление
    private float max_br_value; //максимальный угол поворота корпуса
    private float max_handbrake_value;//максимальное значение ручника
    private float max_incline;//максимальное значение наклона
    private float incline_speed;//скорость наклона корпуса
    private float handbrake_koef;//коэфициент снижение значения ручника
    private float max_overheat_value;//максимальное значение до полного перегрева
    private float overheat_down_koef;//скорость падения перегрева
    private float overheat_koef;//скорость набирания перегрева
    private float handbrake_rollback_time;//время до отката ручника
    private float click_overheat_value;//значение перегрева сразу по нажатию
    private float handbrake_speed_down_koef;//значение снижения скорости от ручника

    //управление
    private float min_rot_speed, max_rot_speed;
    private float speed_koef_btncontroller;//для кнопочного контроля
    private float buildup_koef;//коэфициент раскачки


    void Start()
    {
        PrefsValues();
        //car_define = "bmwe30";
        //car_define = "vwGolf2";

        ObjectDefine();
        StockStartValue();
        DefineCar();
    }

    private void PrefsValues()
    {
        if (PlayerPrefs.HasKey("car_define"))
        {
            car_define = PlayerPrefs.GetString("car_define");
        }
        else
        {
            PlayerPrefs.SetString("car_define", "bmwe30");
        }
    }

    private void StockStartValue()//стоковые значения для всех машин
    {
        value_pref_sise = 1f;
        pref_sise = new Vector3(value_pref_sise,value_pref_sise,value_pref_sise);
    }

    private void ObjectDefine()//определяем скрипты
    {
        player_controller = this.GetComponent<PlayerController>();
        btn_car_controller = GameObject.FindGameObjectWithTag("BtnController").GetComponent<BtnCarController>();
    }

    private void DefineCar()//определяем какую машину используем
    {
        switch (car_define)
        {
            case "bmwe30":
                BmwE30();
                break;
            case "vwGolf2":
                VwGolf2();
                break;
            case "skygtr34":
                SkyGtr34();
                break;
            default:
                BmwE30();
                break;

        }
    }

    private void ModelSelected(GameObject selected_model)//передаем данные о машине
    {
        player_controller.SetModelValues(pref_sise, min_speed, max_speed, speed_koef, speed_down_koef, max_end_drift_speed, rot_to_speed_koef, br_factor, 
            max_body_rot_speed,start_body_rot_value, start_extension_speed, extension_acceleration, max_br_value, max_handbrake_value, handbrake_koef, max_incline,
            incline_speed, max_overheat_value, overheat_down_koef, overheat_koef, handbrake_rollback_time, click_overheat_value, handbrake_speed_down_koef);

        btn_car_controller.SetCarsValue(min_rot_speed, max_rot_speed, speed_koef_btncontroller, buildup_koef);

        player_controller.ModelSelection(selected_model);
    }

    private void BmwE30()
    {
        //в скрипт игрока
        min_speed = 12f; //12
        max_speed = 41f; //28 (1 к 3.577)
        speed_koef = 1.3f; //1.2
        speed_down_koef = 0.9f; //1.1
        max_end_drift_speed = 0f;//0
        rot_to_speed_koef = 0f;//0
        handbrake_speed_down_koef = 1.2f;//1.35

        br_factor = 0.5f; //0.45
        max_body_rot_speed = 18f; //18
        start_body_rot_value = 55f; //55
        start_extension_speed = 20f; //20
        extension_acceleration = 0.2f; //0.2 (чем больше, тем меньше скорость)
        max_br_value = 80f; //60

        max_handbrake_value = 20f;//20
        handbrake_koef = 60f;//60
        max_overheat_value = 100f;//100
        overheat_down_koef = 45f;//45
        overheat_koef = 60f;//60
        handbrake_rollback_time = 10f;//10
        click_overheat_value = 25f;//25

        max_incline = 2.8f;//2.8
        incline_speed = 4f;//4

        //в скрипты управления
        min_rot_speed = 1f;//5
        max_rot_speed = 80f;//70
        speed_koef_btncontroller = 60f;//55
        buildup_koef = 35f;//35

        ModelSelected(bmwE30);
    }

    private void VwGolf2()
    {
        //в скрипт игрока
        min_speed = 11f; //10
        max_speed = 45f; //30 (1 к 3.577)
        speed_koef = 1.7f; //1.7
        speed_down_koef = 1.1f; //1.1
        max_end_drift_speed = 0f;//0
        rot_to_speed_koef = 0.1f;//0
        handbrake_speed_down_koef = 1.5f;//1.6

        br_factor = 0.35f; //0.35
        max_body_rot_speed = 14f; //14
        start_body_rot_value = 70f; //70
        start_extension_speed = 20f; //20
        extension_acceleration = 0.4f; //0.4 (чем больше, тем меньше скорость)
        max_br_value = 60f; //60

        max_handbrake_value = 20f;//20
        handbrake_koef = 60f;//60
        max_overheat_value = 100f;//100
        overheat_down_koef = 45f;//45
        overheat_koef = 60f;//60
        handbrake_rollback_time = 10f;//10
        click_overheat_value = 25f;//25

        max_incline = 2.6f;//2.6
        incline_speed = 4f;//4

        //в скрипты управления
        min_rot_speed = 5f;//10
        max_rot_speed = 90f;//90
        speed_koef_btncontroller = 70f;//70
        buildup_koef = 30f;//40

        ModelSelected(vwGolf2);
    }

    private void SkyGtr34()
    {
        //в скрипт игрока
        min_speed = 13f; //13
        max_speed = 56f; //56 (1 к 3.577)
        speed_koef = 1.9f; //1.9
        speed_down_koef = 1.6f; //1.6
        max_end_drift_speed = 1f;//1
        rot_to_speed_koef = 0.6f;//0.6
        handbrake_speed_down_koef = 1.7f;//1.7

        br_factor = 0.25f; //0.2
        max_body_rot_speed = 14f; //14
        start_body_rot_value = 80f; //60
        start_extension_speed = 18f; //20
        extension_acceleration = 0.4f; //0.4 (чем больше, тем меньше скорость)
        max_br_value = 40f; //40

        max_handbrake_value = 30f;//30
        handbrake_koef = 60f;//60
        max_overheat_value = 100f;//100
        overheat_down_koef = 45f;//45
        overheat_koef = 60f;//60
        handbrake_rollback_time = 10f;//10
        click_overheat_value = 25f;//25

        max_incline = 2.8f;//2.8
        incline_speed = 4f;//4

        //в скрипты управления
        min_rot_speed = 1f;//1
        max_rot_speed = 120f;//120
        speed_koef_btncontroller = 75f;//75
        buildup_koef = 20f;//20

        ModelSelected(skygtr34);
    }

}
