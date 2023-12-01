using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotsController : MonoBehaviour
{
    protected PlayerController player_controller;

    protected Transform tr;
    protected GameObject sensors;
    protected GameObject left_turn_controller, right_turn_controller;
    protected GameObject left_turn_let_controller, right_turn_let_controller;
    protected GameObject left_turn_closed_let, right_turn_closed_let;
    protected GameObject r_front_wheel, l_front_wheel;//передние правое и левое колесо
    protected GameObject l_front_disk, l_back_disk, r_front_disk, r_back_disk;//диски машины
    protected TurnController r_turn_controller, l_turn_controller;
    protected TurnToClosedLet r_closed_controller, l_closed_controller;
    protected TurnToLetController r_turn_let_controller, l_turn_let_controller;

    [SerializeField]
    protected float speed;
    protected float min_speed;
    protected float max_speed;
    protected float speed_koef, speed_down_koef;
    protected bool is_speed_down;

    //turn
    protected bool left_turn, right_turn;
    protected float rotate_speed;
    protected float min_rotatate_speed;
    protected float max_rotate_speed;
    protected float rotate_speed_koef;
    protected float rot_to_speed_koef;
    protected bool is_turn_left,is_turn_right;
    protected int rotate_dir;

    //let
    protected bool left_turn_to_let, right_turn_to_let;
    protected float rotate_speed_let;
    protected float min_rotatate_speed_let;
    protected float max_rotate_speed_let;
    protected float rotate_speed_koef_let;
    protected float rot_to_speed_koef_let;
    protected bool is_turn_right_let, is_turn_left_let;
    protected int rotate_dir_let;

    //stop
    [SerializeField]
    protected bool left_stop_turn, right_stop_turn;

    //wheels
    protected float wheel_rot_speed;//скорость поворота колес
    protected float wheel_rot_angle;//максимальный угол поворота колес
    protected float r_wheel_angle, l_wheel_angle;//нынешний угол поворота колес
    protected float start_r_wheel_angle, start_l_wheel_angle;//первые значения поворота колес
    protected float disk_speed_koef;//коэфициент скорости вращения дисков

    //lights
    protected GameObject lights;
    protected GameObject front_light, back_light;//свет
    protected GameObject back_glass, front_glass;//фары
    protected Material back_glass_mat, front_glass_mat;//матерьялы фар
    protected GameObject l_turn_glass, r_turn_glass;//фары
    protected Material l_turn_glass_mat, r_turn_glass_mat;//матерьялы фар
    protected Color front_light_day, front_light_night;
    protected Color back_light_all_time, back_light_stop;
    protected Color turn_light_all_time, turn_light_active;
    protected DayOrNightController day_or_night_controller;
    protected int min_time_of_day, max_time_of_day, time_od_day;
    protected bool is_day;
    protected float turn_speed;
    protected bool anim_value;

    protected Vector3 direction;
    protected float y_delete_border;
    [SerializeField]
    protected bool has_crush;

    [SerializeField]
    protected bool is_go;

    void Start()
    {
        //DefineAllCarObjects();
        //DefineAllSensors();
        //AllStartValues();
        ////CarStatValues();
    }

    void FixedUpdate()
    {
        //TimerToGoCount();
        //SpeedCalculate();
        //CalculateRotateSpeed();
        //CarMove();
        //GetTurns();
        //WheelRotate();

        //IsDestroy();
    }

    protected void AllStartValues()
    {
        player_controller = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        tr = transform;
        direction = new Vector3(0, 0, 1);
        has_crush = false;
        is_speed_down = false;

        left_turn = right_turn = false;
        left_turn_to_let = right_turn_to_let = false;
        left_stop_turn = right_stop_turn = false;

        is_turn_left = is_turn_right = false;
        
        is_turn_left_let = is_turn_right_let = false;

        start_l_wheel_angle = l_front_wheel.transform.localEulerAngles.y;
        start_r_wheel_angle = r_front_wheel.transform.localEulerAngles.y;

        y_delete_border = -2f;

        is_go = false;

        StartLightSettings();
    }

    protected void StartLightSettings()//начальные значение для световых настроек
    {
        front_light_day = new Color(0.267f, 0.247f, 0.216f, 1.000f);
        front_light_night = new Color(1.000f, 1.000f, 1.000f, 1.000f);
        back_light_all_time = new Color(0.255f, 0.000f, 0.000f, 1.000f);
        back_light_stop = new Color(0.510f, 0.000f, 0.000f, 1.000f);
        turn_light_all_time = new Color(0.235f, 0.043f, 0.000f, 1.000f);
        turn_light_active = new Color(0.941f, 0.172f, 0.000f, 1.000f);

        day_or_night_controller = GameObject.Find("DayCiclyController").GetComponent<DayOrNightController>();

        min_time_of_day = 0;
        max_time_of_day = day_or_night_controller.GetMaxTimeOfDay();
        time_od_day = day_or_night_controller.GetTimeOfDay();
        is_day = day_or_night_controller.GetIsDay();

        anim_value = false;
        turn_speed = 0.4f;
        InvokeRepeating("TicBoolValue", Random.Range(0.1f,0.6f), turn_speed);

        front_light.SetActive(false);
        back_light.SetActive(false);
    }

    protected void TicBoolValue()//возвращение переменной для анимации
    {
        anim_value = !anim_value;
    }

    protected void CarMove()//двигаем машину
    {
        tr.Translate(direction * Time.deltaTime * speed);//движение вперед
        if (!player_controller.GetIsPause() && is_go) {
            CarRotate();
            CarRotateLet();
        }
    }

    protected void SpeedCalculate()//считаем скорость машины
    {
        if (!has_crush && !player_controller.GetIsPause() && is_go)
        {
            if (speed < min_speed)
            {
                speed = min_speed;
            }
            if ((!is_turn_right_let && !is_turn_left_let && !is_turn_left && !is_turn_right) && speed < max_speed)
            {
                speed += Time.deltaTime * speed_koef;
            }
            if ((is_turn_right_let || is_turn_left_let || is_turn_left || is_turn_right) && speed > min_speed)
            {
                speed -= Time.deltaTime * speed_down_koef; 
            }
        }
        else
        {
            speed = 0f;
        }

        if (speed < max_speed - (max_speed - min_speed) / 2) 
        {
            is_speed_down = true;
        }
        else
        {
            is_speed_down = false;
        }
    }

    protected void CalculateRotateSpeed()//считаем скорость поворота
    {
        //turn
        if (is_turn_right || is_turn_left)
        {
            if (rotate_speed < max_rotate_speed)
            {
                rotate_speed += Time.deltaTime * (rotate_speed_koef + (speed * rot_to_speed_koef));
            }
        }
        else
        {
            rotate_speed = min_rotatate_speed;
        }

        //let
        if (is_turn_left_let || is_turn_right_let)
        {
            if (rotate_speed_let < max_rotate_speed_let)
            {
                rotate_speed_let += Time.deltaTime * (rotate_speed_koef_let + (speed * rot_to_speed_koef_let));
                rotate_speed -= Time.deltaTime * rotate_speed_koef / 1.2f;
            }
        }
        else
        {
            rotate_speed_let = min_rotatate_speed_let;
        }
    }

    protected void CarRotate()//поворот машины
    {
        if (!has_crush && !left_stop_turn && !right_stop_turn && is_go) {
            //turn
            if ((left_turn) && !is_turn_left)//вправо
            {
                is_turn_right = true;
                rotate_dir = 1;
                tr.Rotate(new Vector3(0, 1, 0) * rotate_speed * rotate_dir * Time.deltaTime);
            }
            if ((right_turn) && !is_turn_right)//влево
            {
                is_turn_left = true;
                rotate_dir = -1;
                tr.Rotate(new Vector3(0, 1, 0) * rotate_speed * rotate_dir * Time.deltaTime);
            }
            if (!right_turn)
            {
                is_turn_left = false;
            }
            if (!left_turn)
            {
                is_turn_right = false;
            }

        }
    }

    protected void CarRotateLet()//поворот машины от препятствий
    {
        if (!has_crush && !left_stop_turn && !right_stop_turn && is_go) {
            //let
            if (left_turn_to_let && !is_turn_left_let)//вправо
            {
                is_turn_right_let = true;
                rotate_dir_let = 1;
                tr.Rotate(new Vector3(0, 1, 0) * rotate_speed_let * rotate_dir_let * Time.deltaTime);
            }
            if (right_turn_to_let && !is_turn_right_let)//влево
            {
                is_turn_left_let = true;
                rotate_dir_let = -1;
                tr.Rotate(new Vector3(0, 1, 0) * rotate_speed_let * rotate_dir_let * Time.deltaTime);
            }
            if (!right_turn_to_let)
            {
                is_turn_left_let = false;
            }
            if (!left_turn_to_let)
            {
                is_turn_right_let = false;
            }
        }
    }

    protected void AllLightController()//запускаем все фонари на машине
    {
        FrontLightColor();
        BackLightColor();
        if (!has_crush) 
        {
            TurnLightColor();
        }
        else
        {
            EmergencyLightColor();
        }
    }

    private void FrontLightColor()//контрль передних фонарей
    {
        if (is_day)
        {
            ChangeEmissionColor(front_glass_mat, front_light_day);
            front_light.SetActive(false);
        }
        else
        {
            ChangeEmissionColor(front_glass_mat, front_light_night);
            if (is_go)
            {
                front_light.SetActive(true);
            }
            else
            {
                front_light.SetActive(false);
            }
        }
        front_glass.GetComponent<Renderer>().material = front_glass_mat;
    }

    protected void BackLightColor()//контрль задних фонарей
    {
        if (is_speed_down)
        {
            ChangeEmissionColor(back_glass_mat, back_light_stop);
            if (!is_day && is_go)
            {
                back_light.SetActive(true);
            }
        }
        else
        {
            ChangeEmissionColor(back_glass_mat, back_light_all_time);
            if (!is_day)
            {
                back_light.SetActive(false);
            }
        }
        back_glass.GetComponent<Renderer>().material = back_glass_mat;
    }

    protected void TurnLightColor()//контрль боковых фонарей
    {
        if (is_turn_right_let)
        {
            if (anim_value) {
                ChangeEmissionColor(r_turn_glass_mat, turn_light_active);
            }
            else
            {
                ChangeEmissionColor(r_turn_glass_mat, turn_light_all_time);
            }
        }
        else
        {
            ChangeEmissionColor(r_turn_glass_mat, turn_light_all_time);
        }

        if (is_turn_left_let)
        {
            if (anim_value)
            {
                ChangeEmissionColor(l_turn_glass_mat, turn_light_active);
            }
            else
            {
                ChangeEmissionColor(l_turn_glass_mat, turn_light_all_time);
            }
        }
        else
        {
            ChangeEmissionColor(l_turn_glass_mat, turn_light_all_time);
        }

        //l_turn_glass.GetComponent<Renderer>().material = l_turn_glass_mat;
        //r_turn_glass.GetComponent<Renderer>().material = r_turn_glass_mat;
    }

    protected void EmergencyLightColor()//аварийка
    {
        if (anim_value)
        {
            ChangeEmissionColor(r_turn_glass_mat, turn_light_active);
            ChangeEmissionColor(l_turn_glass_mat, turn_light_active);
        }
        else
        {
            ChangeEmissionColor(r_turn_glass_mat, turn_light_all_time);
            ChangeEmissionColor(l_turn_glass_mat, turn_light_all_time);
        }

        //l_turn_glass.GetComponent<Renderer>().material = l_turn_glass_mat;
        //r_turn_glass.GetComponent<Renderer>().material = r_turn_glass_mat;

    }

    protected void ChangeEmissionColor(Material mat, Color col)//меняем цвет свечения
    {
        if (mat != null)
        {
            mat.SetColor("_EmissionColor", col);
        }
    }

    protected void WheelRotate()//поворот колеса в стороны
    {
        if (r_front_wheel != null && l_front_wheel != null)
        {
            r_wheel_angle = r_front_wheel.transform.localEulerAngles.y;
            l_wheel_angle = l_front_wheel.transform.localEulerAngles.y;


            if (is_turn_right_let || is_turn_left_let || is_turn_left || is_turn_right && !has_crush && is_go)
            {
                TurnWheels();   
            }


            if (!is_turn_right_let && !is_turn_left_let && !is_turn_left && !is_turn_right && !has_crush && is_go)
            {
                StraightenTheWheels();
            }
        }
        DiskRot();
    }

    protected void TurnWheels()//поворот колес
    {
        if (is_turn_left || is_turn_right) {
            if (rotate_dir == 1 && r_wheel_angle < start_r_wheel_angle + wheel_rot_angle)
            {
                r_front_wheel.transform.Rotate(new Vector3(1, 0, 0) * wheel_rot_speed * rotate_dir * Time.deltaTime);
            }
            if (rotate_dir == -1 && r_wheel_angle > start_r_wheel_angle - wheel_rot_angle)
            {
                r_front_wheel.transform.Rotate(new Vector3(1, 0, 0) * wheel_rot_speed * rotate_dir * Time.deltaTime);
            }
            if (rotate_dir == 1 && l_wheel_angle < start_l_wheel_angle + wheel_rot_angle)
            {
                l_front_wheel.transform.Rotate(new Vector3(1, 0, 0) * wheel_rot_speed * rotate_dir * Time.deltaTime);
            }
            if (rotate_dir == -1 && l_wheel_angle > start_l_wheel_angle - wheel_rot_angle)
            {
                l_front_wheel.transform.Rotate(new Vector3(1, 0, 0) * wheel_rot_speed * rotate_dir * Time.deltaTime);
            }
        }
        if (is_turn_left_let || is_turn_right_let)
        {
            if (rotate_dir_let == 1 && r_wheel_angle < start_r_wheel_angle + wheel_rot_angle)
            {
                r_front_wheel.transform.Rotate(new Vector3(1, 0, 0) * wheel_rot_speed * rotate_dir_let * Time.deltaTime);
            }
            if (rotate_dir_let == -1 && r_wheel_angle > start_r_wheel_angle - wheel_rot_angle)
            {
                r_front_wheel.transform.Rotate(new Vector3(1, 0, 0) * wheel_rot_speed * rotate_dir_let * Time.deltaTime);
            }
            if (rotate_dir_let == 1 && l_wheel_angle < start_l_wheel_angle + wheel_rot_angle)
            {
                l_front_wheel.transform.Rotate(new Vector3(1, 0, 0) * wheel_rot_speed * rotate_dir_let * Time.deltaTime);
            }
            if (rotate_dir_let == -1 && l_wheel_angle > start_l_wheel_angle - wheel_rot_angle)
            {
                l_front_wheel.transform.Rotate(new Vector3(1, 0, 0) * wheel_rot_speed * rotate_dir_let * Time.deltaTime);
            }
        }
    }

    protected void StraightenTheWheels()//выпремляем колеса
    {
        if (r_wheel_angle < start_r_wheel_angle)
        {
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

    protected void DiskRot()//вращение дисков
    {
        l_back_disk.transform.Rotate(new Vector3(0, 0, 1) * speed * disk_speed_koef * -1 * Time.deltaTime);
        l_front_disk.transform.Rotate(new Vector3(0, 0, 1) * speed * disk_speed_koef * -1 * Time.deltaTime);
        r_front_disk.transform.Rotate(new Vector3(0, 0, 1) * speed * disk_speed_koef * 1 * Time.deltaTime);
        r_back_disk.transform.Rotate(new Vector3(0, 0, 1) * speed * disk_speed_koef * 1 * Time.deltaTime);
    }

    protected void IsDestroy()//удаляем машину после падения
    {
        if (transform.position.y < y_delete_border)
        {
            gameObject.SetActive(false);
            //Destroy(gameObject);
        }
    }

    public void HasCrush()//столкнулась ли машина
    {
        speed = 0f;
        has_crush = true;
    }

    public virtual void ChooseCorpusColor() { }

    public void IsRestart()//переспавним
    {
        has_crush = false;
        is_go = false;
    }

    protected void GetTurns()//получаем значение поворота
    {
        left_turn = l_turn_controller.GetTurnToRoad();
        right_turn = r_turn_controller.GetTurnToRoad();

        if (!l_closed_controller.GetTurnToLet()) {
            left_turn_to_let = l_turn_let_controller.GetTurnToLet();
        }
        else
        {
            left_turn_to_let = true;
        }

        if (!r_closed_controller.GetTurnToLet())
        {
            right_turn_to_let = r_turn_let_controller.GetTurnToLet();
        }
        else
        {
            right_turn_to_let = true;
        }

    }

    protected void DefineAllCarObjects()
    {
        r_front_wheel = transform.Find("R_front_wheel").gameObject;
        r_front_disk = r_front_wheel.transform.Find("R_front_disk").gameObject;
        r_back_disk = transform.Find("R_back_disk").gameObject;

        l_front_wheel = transform.Find("L_front_wheel").gameObject;
        l_front_disk = l_front_wheel.transform.Find("L_front_disk").gameObject;
        l_back_disk = transform.Find("L_back_disk").gameObject;
    }

    protected void DefineAllLights()
    {
        lights = transform.Find("Lights").gameObject;
        front_light = lights.transform.Find("front_ligh").gameObject;
        back_light = lights.transform.Find("back_ligh").gameObject;
        front_glass = transform.Find("Front_light").gameObject;
        back_glass = transform.Find("Back_light").gameObject;
        front_glass_mat = front_glass.GetComponent<Renderer>().material;
        back_glass_mat = back_glass.GetComponent<Renderer>().material;

        l_turn_glass = transform.Find("L_turn_signal").gameObject;
        r_turn_glass = transform.Find("R_turn_signal").gameObject;
        l_turn_glass_mat = l_turn_glass.GetComponent<Renderer>().material;
        r_turn_glass_mat = r_turn_glass.GetComponent<Renderer>().material;
    }

    protected void DefineAllSensors()
    {
        sensors = transform.Find("Sensors").gameObject;

        left_turn_controller = sensors.transform.Find("LeftTurnControler").gameObject;
        right_turn_controller = sensors.transform.Find("RightTurnControler").gameObject;

        left_turn_let_controller = sensors.transform.Find("LeftLetTrigger").gameObject;
        right_turn_let_controller = sensors.transform.Find("RightLetTrigger").gameObject;

        left_turn_closed_let = sensors.transform.Find("LeftClosedLetTrigger").gameObject;
        right_turn_closed_let = sensors.transform.Find("RightClosedLetTrigger").gameObject;

        r_turn_controller = right_turn_controller.GetComponent<TurnController>();
        l_turn_controller = left_turn_controller.GetComponent<TurnController>();
        r_closed_controller = right_turn_closed_let.GetComponent<TurnToClosedLet>();
        l_closed_controller = left_turn_closed_let.GetComponent<TurnToClosedLet>();
        r_turn_let_controller = right_turn_let_controller.GetComponent<TurnToLetController>();
        l_turn_let_controller = left_turn_let_controller.GetComponent<TurnToLetController>();
    }

    protected void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Player")
        {
            HasCrush();
        }
        
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "sphere_trig")
        {
            is_go = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "sphere_trig")
        {
            is_go = false;
        }
    }
}

    
