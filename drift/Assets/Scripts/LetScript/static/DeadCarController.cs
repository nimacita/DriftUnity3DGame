using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadCarController : MonoBehaviour
{

    private GameObject main_rot_body;
    private GameObject hood;//капот
    private GameObject front_support;//подпор капота
    private GameObject trunk;//багажник
    private GameObject back_support;//подпор багажника
    private GameObject r_front_wheel, r_back_wheel;
    private GameObject l_front_wheel, l_back_wheel;
    private GameObject l_broke_wheel_pos, r_broke_wheel_pos, back_broke_wheel_pos;
    private GameObject r_jack, l_jack;
    private GameObject emergency_sign;
    private GameObject hood_smoke;
    private ParticleSystem h_smoke;
    private GameObject front_sensor;
    private Animation hood_anim, trunk_anim;

    private bool has_crush;
    private bool open_hood;
    private bool open_trunk;
    private float max_main_body_rot,main_body_rot;
    private Vector3 emergency_sign_dir;
    private Transform emergency_sign_start_trans;
    private float max_pos_translate;
    private bool back_wheel;

    //lights
    private GameObject l_turn_glass, r_turn_glass;//фары
    public Material turn_glass_all_time, turn_glass_active;
    private Renderer l_turn_glass_rend, r_turn_glass_rend;
    private float turn_speed;
    private bool anim_value;

    //colors
    [SerializeField]
    private Material[] corpus_colors, glass_colors;
    private GameObject corpus, mirrors, glasses, trunk_glass;

    void Start()
    {
        DefineAllObjects();
        DefineLights();
        ChooseCorpusColor();
        AllStartValues();
        LightsStartValues();
    }

    private void AllStartValues()
    {

        has_crush = false;
        open_hood = false;
        front_support.SetActive(false);
        open_trunk = false;
        back_support.SetActive(false);
        h_smoke = hood_smoke.GetComponent<ParticleSystem>();
        h_smoke.Stop();
        hood_smoke.SetActive(false);

        back_wheel = false;

        max_main_body_rot = 4.5f;
        main_body_rot = 0f;

        max_pos_translate = 0.3f;//для смещения знака (0.1)
        emergency_sign_dir = new Vector3(0f, 0f, 1f);

        r_jack.SetActive(false);
        l_jack.SetActive(false);
        l_broke_wheel_pos.SetActive(false);
        r_broke_wheel_pos.SetActive(false);
        back_broke_wheel_pos.SetActive(false);

        emergency_sign_start_trans = emergency_sign.transform;

        DefineBrokeType();
        EmergencySignMove();
    }

    private void LightsStartValues()
    {
        anim_value = false;
        turn_speed = 0.4f;
        InvokeRepeating("EmergencyLightColor", turn_speed, turn_speed);
    }

    private void ChooseCorpusColor()//выбираем цвет
    {
        int rand_color = Random.Range(0, corpus_colors.Length);
        corpus.GetComponent<Renderer>().material = corpus_colors[rand_color];
        mirrors.GetComponent<Renderer>().material = corpus_colors[rand_color];
        hood.GetComponent<Renderer>().material = corpus_colors[rand_color];
        glasses.GetComponent<Renderer>().material = glass_colors[rand_color];
        trunk_glass.GetComponent<Renderer>().material = glass_colors[rand_color];
    }

    private void DefineBrokeType()//определяем тип поломки
    {
        int broke_type = Random.Range(0,2);
        if (broke_type == 0)//капот
        {
            OpenHood();
        }
        else//багажник и колеса
        {
            DefineBrokeWheel();
            OpenTrunk();
        }
    }

    private void DefineBrokeWheel()//определяем сторону ос сломаным колесом
    {
        int rand_side = Random.Range(0, 2);
        if (rand_side == 0)
        {
            RightBrokeWheel();
        }
        else
        {
            LeftBrokeWheel();
        }
    }

    private void RightBrokeWheel()//определяем какое колесо будет под новой позой
    {
        r_jack.SetActive(true);
        main_body_rot = max_main_body_rot;
        MainBodyRot();

        int rand_wheel = Random.Range(0, 2);
        RightBrokeWheelNewPos();
        if (rand_wheel == 0)//переднее
        {
            r_front_wheel.SetActive(false);
        }
        else//заднее
        {
            r_back_wheel.SetActive(false);
        }
    }

    private void RightBrokeWheelNewPos()//определеяем новую позу для правого колеса
    {
        //включаем одну из позиций сломанного колеса
        int rand_pos = Random.Range(0,2);
        if (rand_pos == 0)
        {
            //return r_broke_wheel_pos.transform;
            r_broke_wheel_pos.SetActive(true);
        }
        else
        {
            //return back_broke_wheel_pos.transform;
            back_broke_wheel_pos.SetActive(true);
            back_wheel = true;
        }
    }

    private void LeftBrokeWheel()//определяем какое колесо будет под новой позой
    {
        l_jack.SetActive(true);
        main_body_rot = -max_main_body_rot;
        MainBodyRot();

        int rand_wheel = Random.Range(0, 2);
        LeftBrokeWheelNewPos();
        if (rand_wheel == 0)//переднее
        {
            l_front_wheel.SetActive(false);
        }
        else//заднее
        {
            l_back_wheel.SetActive(false);
        }
    }

    private void LeftBrokeWheelNewPos()//определеяем новую позу для левого колеса
    {
        //включаем одну из позиций сломанного колеса
        int rand_pos = Random.Range(0, 2);
        if (rand_pos == 0)
        {
            l_broke_wheel_pos.SetActive(true);
        }
        else
        {
            back_broke_wheel_pos.SetActive(true);
            back_wheel = true;
        }
    }

    private void OpenHood()//открытие капота
    {
        hood_anim.Play("open_hood");
        open_hood = true;
        front_support.SetActive(true);
        hood_smoke.SetActive(true);
        h_smoke.Play();
    }

    private void CloseHood()//закрытие капота
    {
        if (open_hood)
        {
            hood_anim.Play("close_hood");
            front_support.SetActive(false);
            h_smoke.Stop();
            hood_smoke.SetActive(false);
        }
    }

    private void OpenTrunk()//открытие багажника
    {
        trunk_anim.Play("open_trunk");
        open_trunk = true;
        back_support.SetActive(true);
    }

    private void CloseTrunk()//закрытие багажника
    {
        if (open_trunk)
        {
            trunk_anim.Play("close_trunk");
            back_support.SetActive(false);
        }
    }

    private void MainBodyRot()//наклон корпуса под домкратом
    {
        //если main_body_rot отрицательный, то корпус наклонен влево, если положительный то вправо, если 0, то не наклонен
        main_rot_body.transform.Rotate(new Vector3(0f, 0f, 1f) * main_body_rot);//поворот машины
    }

    private void BodyInclineCrush()//запуск анимации падения корпуса
    {
        if (main_body_rot < 0f)
        {
            main_rot_body.GetComponent<Animation>().Play("l_body_incline_crush");
            if (!back_wheel)
            {
                if (!front_sensor.GetComponent<DeadCarFrontSens>().GetIsEnter()) 
                {
                    l_broke_wheel_pos.GetComponent<Animation>().Play("l_wheel_crush");
                }
                else
                {
                    l_broke_wheel_pos.GetComponent<Animation>().Play("l_front_wheel_crush");
                }
            }
            main_body_rot = 0f;
        }
        if (main_body_rot > 0f)
        {
            main_rot_body.GetComponent<Animation>().Play("r_body_incline_crush");
            if (!back_wheel)
            {
                if (!front_sensor.GetComponent<DeadCarFrontSens>().GetIsEnter()) 
                {
                    r_broke_wheel_pos.GetComponent<Animation>().Play("r_wheel_crush");
                }
                else
                {
                    r_broke_wheel_pos.GetComponent<Animation>().Play("r_front_wheel_crush");
                }
            }
            main_body_rot = 0f;
        }
    }

    protected void EmergencyLightColor()//аварийка
    {
        anim_value = !anim_value;
        if (anim_value)
        {
            r_turn_glass_rend.material = turn_glass_active;
            l_turn_glass_rend.material = turn_glass_active;
        }
        else
        {
            r_turn_glass_rend.material = turn_glass_all_time;
            l_turn_glass_rend.material = turn_glass_all_time;
        }
    }

    private void EmergencySignMove()//рандомное передвижение знака опасности
    {
        float rand_rot_y = Random.Range(-30f, 30f);
        float rand_move_koef = Random.Range(0f, max_pos_translate);
        emergency_sign.transform.Rotate(new Vector3(0f, 1f, 0f) * rand_rot_y);
        emergency_sign.transform.Translate(emergency_sign_dir * rand_move_koef);
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Player" || other.gameObject.tag == "dynamic_let_first")
        {
            if (!has_crush) {
                CloseHood();
                CloseTrunk();
                BodyInclineCrush();
                has_crush = true;
            }
        }
        
    }

    public void ReSpawn()
    {
        CloseHood();
        CloseTrunk();
        open_hood = false;
        front_support.SetActive(false);
        open_trunk = false;
        back_support.SetActive(false);
        back_wheel = false;
        r_jack.SetActive(false);
        l_jack.SetActive(false);
        l_broke_wheel_pos.SetActive(false);
        r_broke_wheel_pos.SetActive(false);
        back_broke_wheel_pos.SetActive(false);
        h_smoke.Stop();
        hood_smoke.SetActive(false);
        main_rot_body.transform.Rotate(new Vector3(0f, 0f, 0f) * main_body_rot);

        emergency_sign.transform.position = emergency_sign_start_trans.position;
        emergency_sign.transform.rotation = emergency_sign_start_trans.rotation;
        EmergencySignMove();

        DefineBrokeType();
        ChooseCorpusColor();
    }

    private void DefineAllObjects()
    {
        main_rot_body = transform.Find("MainBodyToRot").gameObject;

        hood = main_rot_body.transform.Find("Hood").gameObject;
        front_support = main_rot_body.transform.Find("Front_support").gameObject;
        trunk = main_rot_body.transform.Find("Trunk").gameObject;
        back_support = main_rot_body.transform.Find("Back_support").gameObject;
        hood_smoke = main_rot_body.transform.Find("DedCarSmoke").gameObject;
        hood_anim = hood.GetComponent<Animation>();
        trunk_anim = trunk.GetComponent<Animation>();

        corpus = main_rot_body.transform.Find("Body").gameObject;
        mirrors = main_rot_body.transform.Find("Body_mirror").gameObject;
        glasses = main_rot_body.transform.Find("Glass").gameObject;
        trunk_glass = trunk.transform.Find("Glass_back").gameObject;

        r_front_wheel = transform.Find("r_front_wheel").gameObject;
        r_back_wheel = transform.Find("r_back_wheel").gameObject;
        l_front_wheel = transform.Find("l_front_wheel").gameObject;
        l_back_wheel = transform.Find("l_back_wheel").gameObject;

        r_broke_wheel_pos = transform.Find("r_broke_wheel_pos").gameObject;
        l_broke_wheel_pos = transform.Find("l_broke_wheel_pos").gameObject;
        back_broke_wheel_pos = main_rot_body.transform.Find("back_broke_wheel_pos").gameObject;

        r_jack = transform.Find("r_jack").gameObject;
        l_jack = transform.Find("l_jack").gameObject;

        emergency_sign = transform.Find("Emergency_sign_back").gameObject;

        front_sensor = transform.Find("front_sen").gameObject;
    }

    private void DefineLights()
    {
        l_turn_glass = main_rot_body.transform.Find("L_turn_signal").gameObject;
        r_turn_glass = main_rot_body.transform.Find("R_turn_signal").gameObject;

        r_turn_glass_rend = r_turn_glass.GetComponent<Renderer>();
        l_turn_glass_rend = l_turn_glass.GetComponent<Renderer>();
    }
}
