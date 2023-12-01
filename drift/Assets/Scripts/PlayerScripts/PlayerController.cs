using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;


public class PlayerController : MonoBehaviour
{

    private DayOrNightController day_or_night;
    private Vector3 direction,rotate_direction;
    private Transform tr;
    private Transform pref_tr;

    private BtnCarController btn_controller;

    [SerializeField]
    private float speed;//скорость машины
    private float last_speed;//скорость до паузы
    private float min_speed, max_speed;//минимальная и максимальная скорость
    private float speed_koef;//коэфициент набора скорости
    private float speed_down_koef;//коэфициент потери скорости
    private float end_drift_speed;//усорение после выхода из поворота
    private float max_end_drift_speed;//максимальное ускорение при выходе из поворота


    private float distance;//пройденная дистанция
    private Vector3 start_position;//начальная позиция
    private Vector3 cur_position;//нынешняя позиция

    [SerializeField]
    private float rotate_speed;//скорость поворота
    private int rot_dir;//направления поворота
    private float rot_to_speed_koef;

    private bool is_drift;//находится ли машина в дрифте
    private bool is_end_drigt;//вышел из дрифта
    private bool is_end_game;//закончилоась ли игра
    private bool is_pause;//на пузе ли игра

    //body_rot
    [SerializeField]
    private float br_factor;//фактор скорости набирания заноса
    private float max_body_rot_speed; //максимальный скорость заноса
    private float start_body_rot_speed;//угол (скорость) поворота после которой начинается занос корпуса
    private float extension_speed, start_extension_speed;//скорость выпрямления после заноса
    private float extension_acceleration;//ускорение скорости выпрямления
    [SerializeField]
    private float value_br;//значение заноса корпуса
    private float max_br_value;//максимальный угол поворота корпуса
    private float br_angle;//нынешний угол поворота корпуса
    private int body_rot_dir;

    //handbrake
    [SerializeField]
    private bool is_handbrake;//работает ли ручник
    private bool is_handbrake_btn_down;//нажата ли кнопка ручника
    private float handbrake_value;//значение добавки с ручником
    private float max_handbrake_value;//максимальное значение ручника
    private float handbrake_koef;//коэфициент снижение значения ручника
    private float overheat_value;//значение перегрева
    private bool is_overheat;//начался ли перегрев
    private float max_overheat_value;//максимальное значение до полного перегрева
    private float overheat_down_koef;//скорость падения перегрева
    private float overheat_koef;//скорость набирания перегрева
    private float handbrake_rollback_time;//время до отката ручника
    private float handbrake_rollback_value;//значение отката ручника
    private float click_overheat_value;//значение перегрева сразу по нажатию
    private bool is_click_plus;//прибавили ли
    private float handbrake_speed_down_koef;//значение снижения скорости от русника

    //wheel_rot
    private float wheel_rot_speed = 80f;//30 скорость поворота колес
    private float wheel_rot_angle = 30f;//20 максимальный угол поворота колес
    private float plus_rot_for_wheel = 15;//дабавка для поворота
    private float r_wheel_angle, l_wheel_angle;//нынешний угол поворота колес
    private float start_r_wheel_angle, start_l_wheel_angle;//первые значения поворота колес
    private GameObject r_front_wheel, l_front_wheel;//передние правое и левое колесо
    private GameObject r_front_disk, r_back_disk;//правые диски машины
    private GameObject l_front_disk, l_back_disk;//левые диски машины
    private float disk_speed_koef = 40f;//коэфициент скорости вращения дисков

    //incline
    private GameObject body_incline;//корпус машины для наклона
    private float body_incline_angle;//нынешний угол наклона корпуса
    private float max_incline;//максимальное значение наклона
    private float incline_speed;//скорость наклона корпуса

    //car prefabs
    private GameObject pref;
    private Vector3 pref_sise;

    //another_scripts_value
    private bool istouch;
    public bool is_btn_controll;
    public PhysicMaterial phisics_material;

    //rain adds
    private float br_factor_add, max_body_rot_speed_add, max_br_value_add;
    private bool is_add,is_down;

    void Start()
    {
        AllStartValues();
    }

    
    void FixedUpdate()
    {
        BtnControllerScriptsValues();

        CalculateHanbrakeValue();

        SpeedCalculate();
        CarMove();
        IsNeedStraighten();
        FindIsDrift();
        Wheel_rotate();
        RaindAdd();

        CalculateDistance();

        IsRestart();
    }

    private void AllStartValues()//начальные значения
    {
        day_or_night = GameObject.FindGameObjectWithTag("don").GetComponent<DayOrNightController>();
        btn_controller = GameObject.FindGameObjectWithTag("BtnController").GetComponent<BtnCarController>();
        tr = transform;
        speed = min_speed;
        last_speed = speed;
        value_br = 0f;
        body_rot_dir = 0;

        //handbrake
        is_handbrake = false;
        is_handbrake_btn_down = false;
        is_overheat = false;
        handbrake_value = 0f;
        overheat_value = 0f;
        handbrake_rollback_value = 0f;
        is_click_plus = false;

        distance = 0f;
        start_position = this.transform.position;
        cur_position = start_position;

        direction = new Vector3(0, 0, 1);
        rotate_direction = new Vector3(0, 1, 0);
        is_drift = false;
        is_end_drigt = true;
        is_end_game = false;
        is_pause = false;

        is_add = false;
        is_down = true;

        BtnControllerScriptsValues();
        SetPhisicsMaterial(0.0f);
    }

    private void BtnControllerScriptsValues()//тип управления кнопками
    {
        istouch = btn_controller.GetIsTouch();
        rot_dir = btn_controller.GetDirection();
        rotate_speed = btn_controller.GetRotateSpeed();
        if (istouch) {
            rotate_speed += handbrake_value;
        }
        if (rotate_speed > 0f && rot_to_speed_koef > 0f)
        {
            rotate_speed += (speed * rot_to_speed_koef);
        }
    }

    private void RaindAdd()//добавление значений во время дождя
    {
        br_factor_add = 0.2f;
        max_body_rot_speed_add = 2f;
        max_br_value_add = 10f;
        if (day_or_night.GetIsRain())
        {
            if (!is_add)
            {
                br_factor += br_factor_add;
                max_body_rot_speed += max_body_rot_speed_add;
                max_br_value += max_br_value_add;
                is_down = false;
                is_add = true;
            }
        }
        else
        {
            if (!is_down)
            {
                br_factor -= br_factor_add;
                max_body_rot_speed -= max_body_rot_speed_add;
                max_br_value -= max_br_value_add;
                is_add = false;
                is_down = true;
            }
        }
    }

    private void IsRestart()//нужно ли перезапустить
    {
        if (pref.transform.position.y < -2f && !is_end_game)
        {
            GameEnd();
            //Destroy(pref);
            pref.SetActive(false);
        }
    }

    private void GameEnd()//после врезания
    {
        is_end_game = true;
        is_pause = false;
        SetPhisicsMaterial(0.6f);
    }

    private void CalculateDistance()//вычисляем пройденную истанцию
    {
        cur_position = this.transform.position;
        distance += Vector3.Distance(start_position,cur_position);
        start_position = cur_position;
    }

    private void SpeedCalculate()//расчет скорости
    {
        if (!is_end_game && !is_pause) {
            if (speed < min_speed)
            {
                speed = min_speed;
            }
            if ((!istouch && !is_handbrake) && speed < max_speed)
            {
                speed += Time.deltaTime * speed_koef;
            }
            if (istouch && !is_handbrake && speed > min_speed)
            {
                speed -= Time.deltaTime * speed_down_koef;
            }
            if (is_handbrake && speed > min_speed)
            {
                speed -= Time.deltaTime * handbrake_speed_down_koef;
            }
            last_speed = speed;

            if (end_drift_speed > 0f)//рывок на выходе из поворота
            {
                speed += end_drift_speed;
                end_drift_speed = 0f;
            }
        }
        else
        {
            speed = 0f;
        }
    }

    private void CarMove()//движение машины
    {
        if (istouch && !is_end_game && !is_pause)
        {
            tr.Rotate(rotate_direction * rotate_speed  * rot_dir * Time.deltaTime);//поворот машины
            BodyRot();//поворот корпуса
            BodyIncline();//наклон корпуса
        }
        if (!istouch)
        {
            BodyInclineStraight();//выпрямление корпуса после наклона
        }

        tr.Translate(direction * Time.deltaTime * speed);//движение вперед
    }

    public void ModelSelection(GameObject selected_model)//выбор моедли для спавна
    {
        Spawn_model_car(selected_model);
    }

    private void Spawn_model_car(GameObject car_pref)//создаем модель машины
    {
        pref = Instantiate(car_pref, transform.position, transform.rotation) as GameObject;
        pref_tr = pref.transform;
        pref_tr.SetParent(this.transform);
        pref_tr.localScale = pref_sise;

        DefineCarObjects();
    }

    private void BodyRot()//поворот корпуса
    {
        br_angle = pref_tr.localEulerAngles.y;
        if ((br_angle < 0f + max_br_value || br_angle > 360f - max_br_value)) {
            if (rot_dir == 1)
            {
                pref_tr.Rotate(rotate_direction * BodyRotationValue() * rot_dir * Time.deltaTime);
            }
            if (rot_dir == -1)
            {
                pref_tr.Rotate(rotate_direction * BodyRotationValue() * rot_dir * Time.deltaTime);
            }
        }
    }

    private float BodyRotationValue()//определяем скорость поворота корпуса
    {
        if (rotate_speed >=start_body_rot_speed + 1 /*&& (br_angle < 0f + max_br_value || br_angle > 360f - max_br_value)*/)
        {
            value_br = (Mathf.Round(rotate_speed) - start_body_rot_speed) * br_factor;
            if (value_br > max_body_rot_speed)
            {
                value_br = max_body_rot_speed;
            }
            if (body_rot_dir == 0)
            {
                body_rot_dir = rot_dir;
            }
            if (body_rot_dir != rot_dir)
            {
                value_br = max_body_rot_speed * 2f;
            }
        }
        else
        {
            value_br = 0f;
        }
        if (pref_tr.localEulerAngles.y <= 3f || pref_tr.localEulerAngles.y >= 357f) 
        {
            body_rot_dir = rot_dir;
        }
        return value_br;
    }

    private void IsNeedStraighten()//нужны ли выпримиться
    {
        if (/*BodyRotationValue() == 0*/rotate_speed < start_body_rot_speed + 1 && !is_end_game && !is_pause)//выпремляемся
        {
            StraightenTheBody();
            if (br_angle < 180f)
            {
                if (extension_speed < start_extension_speed * 2)
                {
                    extension_speed += Time.deltaTime * (br_angle / extension_acceleration);
                }
            }
            if (br_angle > 180f)
            {
                if (extension_speed < start_extension_speed * 2)
                {
                    extension_speed += Time.deltaTime * ((360f-br_angle) / extension_acceleration);
                }
            }

        }
        else
        {
            extension_speed = start_extension_speed;
        }
    }

    private void StraightenTheBody()//выпремляем корпус
    {

        //&& pref.transform.localEulerAngles.y > 1f && pref.transform.localEulerAngles.y < 359f погрешность в if
        if (pref_tr.localEulerAngles.y > 0 && pref_tr.localEulerAngles.y < 180 )//разворот влево
        {
            pref_tr.Rotate(rotate_direction * extension_speed * -1 * Time.deltaTime);
        }
        if (pref_tr.localEulerAngles.y > 0 && pref_tr.localEulerAngles.y > 180 )//разворот вправо
        {
            pref_tr.Rotate(rotate_direction * extension_speed * 1 * Time.deltaTime);
        }
    }

    private void FindIsDrift()//определяем в дрифте ли машина
    {
        if (rotate_speed >= start_body_rot_speed + 1)
        {
            is_drift = true;
        }
        if (pref != null)
        {
            if (pref_tr.localEulerAngles.y <= 3f && rotate_speed < start_body_rot_speed + 1)
            {
                is_drift = false;
            }
            if (pref_tr.localEulerAngles.y >= 357f && rotate_speed < start_body_rot_speed + 1)
            {
                is_drift = false;
            }
        }

        if (!is_drift && !is_end_drigt)//смотрим выход из дрифта
        {
            end_drift_speed = max_end_drift_speed;
            is_end_drigt = true;
        }
        if (is_drift)
        {
            is_end_drigt = false;
        }

        //if (rotate_speed >= start_body_rot_speed + 1)
        //{
        //    is_drift = true;
        //}
        //if ()
        //{
        //    is_drift = false;
        //}
    }

    private void CalculateHanbrakeValue()//вычесление значение добавки ручника
    {

        CalculateOverHeatValue();

        if (!is_pause && !is_end_game) {
            //если ручник отжат или перегрев, то значение ручника опускаем
            if (!is_handbrake_btn_down && !is_pause)
            {
                is_click_plus = false;
                if (handbrake_value > 0f && !is_pause && !is_end_game) {
                    handbrake_value -= Time.deltaTime * handbrake_koef;
                    is_handbrake = true;
                }
                else 
                {
                    handbrake_value = 0f;
                    is_handbrake = false;
                }
            }
            //если ручник не нажат, то ставим его макс значение
            else if (handbrake_rollback_value <= 0f)
            {
                handbrake_value = max_handbrake_value;
                is_handbrake = true;
            }
        }
    }

    private void CalculateOverHeatValue()//вычисление значения перегрева
    {
        if (!is_pause && !is_end_game) {
            //если нажат ручник и нет отката то набираем значение перегрева до максимального
            if (is_handbrake_btn_down && overheat_value <= max_overheat_value && !is_overheat && handbrake_rollback_value <= 0f)
            {
                overheat_value += Time.deltaTime * overheat_koef;
            }

            //если ручник отпущен или перегрет, то опускаем значение перегрева
            if ((!is_handbrake_btn_down || is_overheat) && overheat_value >= 0f)
            {
                overheat_value -= Time.deltaTime * overheat_down_koef;
            }

            //если перегрев максимальный, то сатвим время откату и перегрев
            if (overheat_value >= max_overheat_value)
            {
                is_overheat = true;
                is_handbrake_btn_down = false;
                handbrake_rollback_value = handbrake_rollback_time;
            }
            //выключаем перегрев
            if (overheat_value <= 0f)
            {
                is_overheat = false;
            }

            //если откат, то откатываем
            if (handbrake_rollback_value > 0f)
            {
                handbrake_rollback_value -= Time.deltaTime;
            }
        }
    }

    public void HandbrakeDown()//нажатие на ручник
    {
        if (handbrake_rollback_value <= 0f) 
        {
            is_handbrake_btn_down = true;
            if (!is_click_plus)
            {
                overheat_value += click_overheat_value;
                is_click_plus = true;
            }
        }
    }

    public void HandbrakeUp()//отжатие ручника
    {
        is_handbrake_btn_down = false;
    }

    private void Wheel_rotate()//поворот колеса в стороны
    {
        if (r_front_wheel != null && l_front_wheel != null)
        {
            r_wheel_angle = r_front_wheel.transform.localEulerAngles.y;
            l_wheel_angle = l_front_wheel.transform.localEulerAngles.y;


            if (istouch && !is_pause && !is_end_game)
            {
                if (/*!is_drift*/ rotate_speed < start_body_rot_speed + plus_rot_for_wheel)
                {
                    TunrnToDrift();
                }

                if (/*is_drift*/rotate_speed >= start_body_rot_speed + plus_rot_for_wheel)
                {
                    TunrnAfterDrift();
                }
            }


            if (!istouch && !is_pause && !is_end_game)
            {
                StraightenTheWheels();
            }
        }

        DiskRot();
    }

    private void DiskRot()//вращение дисков
    {
        l_front_disk.transform.Rotate(new Vector3(0, 0, 1) * speed * disk_speed_koef * -1 * Time.deltaTime);
        r_front_disk.transform.Rotate(new Vector3(0, 0, 1) * speed * disk_speed_koef * 1 * Time.deltaTime);
        if (!is_handbrake_btn_down)
        {
            l_back_disk.transform.Rotate(new Vector3(0, 0, 1) * speed * disk_speed_koef * -1 * Time.deltaTime);
            r_back_disk.transform.Rotate(new Vector3(0, 0, 1) * speed * disk_speed_koef * 1 * Time.deltaTime);
        }
    } 

    private void TunrnToDrift()//поворот колес до дрфита
    {
        if (rot_dir == 1 && r_wheel_angle < start_r_wheel_angle + wheel_rot_angle)
        {
            r_front_wheel.transform.Rotate(new Vector3(1, 0, 0) * wheel_rot_speed * rot_dir * Time.deltaTime);
        }
        if (rot_dir == -1 && r_wheel_angle > start_r_wheel_angle - wheel_rot_angle)
        {
            r_front_wheel.transform.Rotate(new Vector3(1, 0, 0) * wheel_rot_speed * rot_dir * Time.deltaTime);
        }
        if (rot_dir == 1 && l_wheel_angle < start_l_wheel_angle + wheel_rot_angle)
        {
            l_front_wheel.transform.Rotate(new Vector3(1, 0, 0) * wheel_rot_speed * rot_dir * Time.deltaTime);
        }
        if (rot_dir == -1 && l_wheel_angle > start_l_wheel_angle - wheel_rot_angle)
        {
            l_front_wheel.transform.Rotate(new Vector3(1, 0, 0) * wheel_rot_speed * rot_dir * Time.deltaTime);
        }
    }

    private void TunrnAfterDrift()//поворот колес во время дрфита
    {
        if (rot_dir == 1 && r_wheel_angle > start_r_wheel_angle - wheel_rot_angle)
        {
            r_front_wheel.transform.Rotate(new Vector3(1, 0, 0) * wheel_rot_speed * 1f * -rot_dir * Time.deltaTime);
        }
        if (rot_dir == -1 && r_wheel_angle < start_r_wheel_angle + wheel_rot_angle)
        {
            r_front_wheel.transform.Rotate(new Vector3(1, 0, 0) * wheel_rot_speed * 1f * -rot_dir * Time.deltaTime);
        }
        if (rot_dir == 1 && l_wheel_angle > start_l_wheel_angle - wheel_rot_angle)
        {
            l_front_wheel.transform.Rotate(new Vector3(1, 0, 0) * wheel_rot_speed * 1f * -rot_dir * Time.deltaTime);
        }
        if (rot_dir == -1 && l_wheel_angle < start_l_wheel_angle + wheel_rot_angle)
        {
            l_front_wheel.transform.Rotate(new Vector3(1, 0, 0) * wheel_rot_speed * 1f * -rot_dir * Time.deltaTime);
        }
    }

    private void StraightenTheWheels()//выпремляем колеса
    {
        if (r_wheel_angle < start_r_wheel_angle)
        {
            //r_front_wheel.transform.Rotate(new Vector3(1, 0, 0) * extension_speed * 1 * Time.deltaTime);
            r_front_wheel.transform.Rotate(new Vector3(1, 0, 0) * wheel_rot_speed * 1 * Time.deltaTime);
        }
        if (r_wheel_angle > start_r_wheel_angle)
        {
            r_front_wheel.transform.Rotate(new Vector3(1, 0, 0) * wheel_rot_speed * -1 * Time.deltaTime);
        }
        if (l_wheel_angle < start_l_wheel_angle)
        {
            l_front_wheel.transform.Rotate(new Vector3(1, 0, 0) * wheel_rot_speed * 1 * Time.deltaTime);
        }
        if (l_wheel_angle > start_l_wheel_angle)
        {
            l_front_wheel.transform.Rotate(new Vector3(1, 0, 0) * wheel_rot_speed * -1 * Time.deltaTime);
        }
    }

    private void BodyIncline()//наклон корпуса
    {
        body_incline_angle = body_incline.transform.localEulerAngles.z;
        if (body_incline_angle > 180f)
        {
            body_incline_angle -= 360f;
        }
        if (rot_dir == 1 && body_incline_angle > -max_incline)
        {
            body_incline.transform.Rotate(new Vector3(0,0,1) * incline_speed * -rot_dir * Time.deltaTime);
        }
        if (rot_dir == -1 && body_incline_angle < max_incline)
        {
            body_incline.transform.Rotate(new Vector3(0, 0, 1) * incline_speed * -rot_dir * Time.deltaTime);
        }
    }

    private void BodyInclineStraight()//выпрямление корпуса после наклона
    {
        if (body_incline != null) {
            body_incline_angle = body_incline.transform.localEulerAngles.z;
            if (body_incline_angle > 180f)
            {
                body_incline_angle -= 360f;
            }
            if (body_incline_angle < 0f)
            {
                body_incline.transform.Rotate(new Vector3(0, 0, 1) * incline_speed * 1 * Time.deltaTime);
            }
            if (body_incline_angle > 0f)
            {
                body_incline.transform.Rotate(new Vector3(0, 0, 1) * incline_speed * -1 * Time.deltaTime);
            }
        }
    }

    private void DefineCarObjects()
    {
        //left_turn_controller = sensors.transform.Find("LeftTurnControler").gameObject;
        r_front_wheel = pref.transform.Find("R_front_wheel").gameObject;
        l_front_wheel = pref.transform.Find("L_front_wheel").gameObject;
        start_l_wheel_angle = l_front_wheel.transform.localEulerAngles.y;
        start_r_wheel_angle = r_front_wheel.transform.localEulerAngles.y;

        l_front_disk = l_front_wheel.transform.Find("L_front_disk").gameObject;
        l_back_disk = pref.transform.Find("L_back_wheel").gameObject.transform.Find("L_back_disk").gameObject;
        r_front_disk = r_front_wheel.transform.Find("R_front_disk").gameObject;
        r_back_disk = pref.transform.Find("R_back_wheel").gameObject.transform.Find("R_back_disk").gameObject;

        body_incline = pref.transform.Find("Body_incline").gameObject;

        GetComponent<PlayerLightController>().DefineObjects(body_incline);
    }

    private void SetPhisicsMaterial(float friction)//установка значения трения
    {
        phisics_material.dynamicFriction = friction;
        phisics_material.staticFriction = friction;
    }

    public bool GetIsDrift()
    {
        return is_drift;
    }

    public bool GetIsHandbrake()
    {
        return is_handbrake;
    }

    public float GetDistance()
    {
        return distance;
    }

    public float GetLastSpeed()
    {
        return last_speed;
    }

    public bool GetIsEndGame()
    {
        return is_end_game;
    }

    public bool GetIsPause()
    {
        return is_pause;
    }

    public bool GetIsOverheat()
    {
        return is_overheat;
    }

    public float GetOverHeatValue()
    {
        return overheat_value;
    }

    public float GetMaxOverHeat()
    {
        return max_overheat_value;
    }

    public float GetHandbrakeRollbackValue()
    {
        return handbrake_rollback_value;
    }

    public float GetRollbackTime()
    {
        return handbrake_rollback_time;
    }

    public void SetEndGame()
    {
        GameEnd();
    }

    public void SetIsPause()
    {
        is_pause = true;
    }

    public void SetPlayGame()
    {
        is_pause = false;
        speed = last_speed;
    }

    public void SetModelValues(Vector3 set_pef_sise,float set_min_speed, float set_max_speed, 
        float set_speed_koef, float set_speed_down_koef,float set_max_end_drift_speed, float set_rot_to_speed_koef,float set_br_factor, 
        float set_max_body_rot_speed,float set_start_body_rot_value, float set_start_extension_speed, 
        float set_extension_acceleration,float set_max_br_value, float set_max_handbrake_value, float set_handbrake_koef, 
        float set_max_incline,float set_incline_speed, float set_max_overheat_value, float set_overheat_down_koef, 
        float set_overheat_koef,float set_handbrake_rollback_time, float set_click_overheat_value, 
        float set_handbrale_speed_down_koef)//устанавливаем значения для машин
    {
        pref_sise = set_pef_sise;
        min_speed = set_min_speed;
        max_speed = set_max_speed;
        speed_koef = set_speed_koef;
        speed_down_koef = set_speed_down_koef;
        max_end_drift_speed = set_max_end_drift_speed;
        rot_to_speed_koef = set_rot_to_speed_koef;
        br_factor = set_br_factor;
        max_body_rot_speed = set_max_body_rot_speed;
        start_body_rot_speed = set_start_body_rot_value;
        start_extension_speed = set_start_extension_speed;
        extension_acceleration = set_extension_acceleration;
        max_br_value = set_max_br_value;
        max_handbrake_value = set_max_handbrake_value;
        handbrake_koef = set_handbrake_koef;
        max_incline = set_max_incline;
        incline_speed = set_incline_speed;
        max_overheat_value = set_max_overheat_value;
        overheat_down_koef = set_overheat_down_koef;
        overheat_koef = set_overheat_koef;
        handbrake_rollback_time = set_handbrake_rollback_time;
        click_overheat_value = set_click_overheat_value;
        handbrake_speed_down_koef = set_handbrale_speed_down_koef;
}

}
