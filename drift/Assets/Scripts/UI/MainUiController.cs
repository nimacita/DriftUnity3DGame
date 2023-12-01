using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(FpsCounter))]//компонент счетчика фпс

public class MainUiController : MonoBehaviour
{

    public PlayerController player_controller;
    public LetSpawnController let_spawn_controller;
    public RoadController road_controller;
    public DayOrNightController day_cycly_controller;
    private FpsCounter fps_counter;
    public GameObject photo_cam;
    public GameObject main_cam;

    //MainGame
    public bool is_polygon;//тестовая кнопка на полигоне ли мы
    public GameObject left_btn, right_btn;
    public GameObject pause_btn;
    public GameObject data_menu;
    public GameObject dist_img;
    public Text dist_text;
    public GameObject speed_img;
    public GameObject spedomentr, speed_arrow;
    public Text speed_text;
    public Text fps_text;
    public GameObject handbrake_btn;
    public GameObject handbrake_idle, handbrake_active;
    private Image handbrake_img;

    //heightscore
    public GameObject dist_hightscore;
    public GameObject speed_hightscore;
    private bool is_best_speed, is_best_dist;

    //UnPause and Restart
    public GameObject right_menu;
    public GameObject restart_btn;
    public GameObject play_btn;
    public GameObject sett_btn;
    public GameObject photo_btn;
    public GameObject garage_btn;
    public GameObject exit_btn;
    public GameObject best_menu;
    public Text best_speed;
    public Text best_dist;

    //Animation
    private Animation pause_btn_anim;
    private Animation right_menu_amim;
    private Animation distance_anim;
    private Animation pause_cd_anim;
    private Animation drift_score_anim;
    private Animation best_menu_anim;
    private Animation data_menu_anim;
    private Animation dist_hightscore_anim;
    private Animation speed_hightscore_anim;

    //all values
    private float speed_koef;
    private float time_sec;
    private float distance;
    private float max_dist_to_harder;
    private bool harder;
    public bool do_is_harder;
    private bool is_restart;
    private bool is_pause;
    private float best_speed_value;
    private float best_dist_value;

    //speed arrow
    private float min_arrow_rot, max_arrow_rot;
    private RectTransform arrow_tr;

    //pause countdown
    public GameObject pause_count_down;
    private Text pause_count_down_txt;
    private bool is_end_count;
    private float pause_countdown_value;
    private float max_countdown_value, min_countdown_value;
    private int last_cd_value;

    //chsnge car (test)
    private string[] car_define = new string[]{"bmwe30", "vwGolf2", "skygtr34" };
    private int car_define_index;
    public GameObject which_car_txt;
    public GameObject next_btn, back_btn;
    private bool is_change;
    public int next_scene;

    //handbrake
    private bool handbrake_btn_down;
    private float fill_value;
    private float handbrake_rollback_value, rollback_time;
    private float overheat_value, max_over_heat_value;
    public Color start_handbrale, overheat_handbrake;

    //drift_score
    public GameObject drift_score_comp;
    private Text drift_score_txt;
    private DisableBtn is_drift_anim;
    private bool is_drift;
    private float drift_score;
    private float drift_score_koef;
    private bool go_ds_anim,end_ds_anim;


    void Awake()
    {
        StartValues();
        DefinePrefsValues();
        DefineAllAnim();
        StartUIMenu();
        //MainGameMenu();
    }

    private void StartValues()
    {
        fps_counter = GetComponent<FpsCounter>();

        is_restart = false;
        is_pause = false;

        //driftscore
        drift_score_txt = drift_score_comp.GetComponent<Text>();
        drift_score_anim = drift_score_comp.GetComponent<Animation>();
        is_drift_anim = drift_score_comp.GetComponent<DisableBtn>();
        drift_score_koef = 100f;
        go_ds_anim = true;
        end_ds_anim = false;

        //heightscore
        is_best_dist = false;
        is_best_speed = false;

        //pause countdown
        pause_count_down_txt = pause_count_down.GetComponent<Text>();
        pause_count_down.SetActive(false);
        is_end_count = true;
        max_countdown_value = 3f;
        min_countdown_value = 1f;
        pause_countdown_value = 0f;
        last_cd_value = 0;

        //values
        speed_koef = 3.576f;//относительно реальной скорости к расстоянию
        max_dist_to_harder = 1000f;
        harder = false;

        //speed_arrow
        min_arrow_rot = 120f;
        max_arrow_rot = -30f;
        arrow_tr = speed_arrow.GetComponent<RectTransform>();

        //handbrake
        handbrake_btn_down = false;
        fill_value = 1f;
        handbrake_img = handbrake_btn.GetComponent<Image>();

        //photo
        photo_cam.SetActive(false);
        main_cam.SetActive(true);

        //test
        is_change = false;
        TestChangeCarBtns(is_change);
        car_define_index = DefineIndex();
        ChangeCarTxt();
    }

    private void DefineAllAnim()//определяем все анимации
    {
        pause_btn_anim = pause_btn.GetComponent<Animation>();
        right_menu_amim = right_menu.GetComponent<Animation>();
        pause_cd_anim = pause_count_down.GetComponent<Animation>();
        distance_anim = dist_img.GetComponent<Animation>();
        best_menu_anim = best_menu.GetComponent<Animation>();
        data_menu_anim = data_menu.GetComponent<Animation>();
        dist_hightscore_anim = dist_hightscore.GetComponent<Animation>();
        speed_hightscore_anim = speed_hightscore.GetComponent<Animation>();
    }

    private int DefineIndex()//определяем индекс по префс
    {
        if (PlayerPrefs.HasKey("car_define"))
        {
            for (int i = 0; i < car_define.Length; i++)
            {
                if (car_define[i] == PlayerPrefs.GetString("car_define"))
                {
                    return i;
                }
            }
        }
        else
        {
            return 0;
        }
        return 0;
    }

    private void DefinePrefsValues()//определяем значения прфес
    {
        if (!PlayerPrefs.HasKey("best_speed"))
        {
            PlayerPrefs.SetFloat("best_speed", 0f);
            best_speed_value = 0f;
        }
        else
        {
            best_speed_value = PlayerPrefs.GetFloat("best_speed");
        }
        if (!PlayerPrefs.HasKey("best_dist"))
        {
            PlayerPrefs.SetFloat("best_dist", 0f);
            best_dist_value = 0f;
        }
        else
        {
            best_dist_value = PlayerPrefs.GetFloat("best_dist");
        }
    }

    private void ChangeCarTxt()//меняем значения текста какая машина
    {
        which_car_txt.GetComponent<Text>().text = car_define[car_define_index];
    }
    
    void FixedUpdate()
    {
        GetEnd();
        GetAnyValues();
        SetDistanceTxt();
        SetSpeedTxt();
        ChechBestValues();
        CheckToNeedHarder();
        HandbrakeBtn();
        GoPauseCount();
        SetDriftScore();
        SpeedArrowControll();

        //SetFpsTxt();
        //CalculateTime();
    }

    private void GetAnyValues()//берем значения с других скриптов
    {
        handbrake_rollback_value = player_controller.GetHandbrakeRollbackValue();
        rollback_time = player_controller.GetRollbackTime();
        overheat_value = player_controller.GetOverHeatValue();
        max_over_heat_value = player_controller.GetMaxOverHeat();
        is_drift = player_controller.GetIsDrift();
    }

    private void CheckToNeedHarder()//сложнее ли
    {
        if (distance >= max_dist_to_harder && !harder && do_is_harder)
        {
            let_spawn_controller.MoreHarder();
            road_controller.MoreHarder();
            harder = true;
        }
    }

    private void StartUIMenu()//начальные настройки ui 
    {
        BtnForControll(true);
        data_menu.SetActive(true);
        right_menu.SetActive(true);
        handbrake_btn.SetActive(true);
        play_btn.SetActive(false);
        restart_btn.SetActive(false);
        sett_btn.SetActive(false);
        photo_btn.SetActive(false);
        garage_btn.SetActive(false);
        exit_btn.SetActive(false);
        photo_cam.SetActive(false);
        drift_score_comp.SetActive(false);
        best_menu.SetActive(false);
        dist_hightscore.SetActive(false);
        speed_hightscore.SetActive(false);
    }

    private void MainGameMenu()//меню игры
    {
        BtnForControll(true);
        speed_img.SetActive(true);
        dist_img.SetActive(true);
        handbrake_btn.SetActive(true);

        DistanceAnim(false);
        TestChangeCarBtns(false);
        BestMenu(false);
    }

    private void RestartMenu()//меню рестарта
    {
        BtnForControll(false);
        RightMenu(true,true);
        DistanceAnim(true);
        BestMenu(true);

        handbrake_btn.SetActive(false);
    }

    private void PauseMenu()//меню паузы
    {
        BtnForControll(false);
        RightMenu(true);
        DistanceAnim(true);
        BestMenu(true);

        handbrake_btn.SetActive(false);
    }

    private void RightMenu(bool active, bool restart = false, bool pause_active = true)//анимация правого меню
    {
        if (active)
        {
            pause_btn_anim.Play("PauseBtnOn");
            right_menu_amim.Play("RightMenuOn");
            if (restart)
            {
                play_btn.SetActive(false);
                restart_btn.SetActive(true);
            }
            else
            {
                restart_btn.SetActive(false);
                play_btn.SetActive(true);
            }
            sett_btn.SetActive(true);
            photo_btn.SetActive(true);
            garage_btn.SetActive(true);
            exit_btn.SetActive(true);
        }
        else
        {
            pause_btn.SetActive(pause_active);
            pause_btn_anim.Play("PauseBtnOff");
            right_menu_amim.Play("RightMenuOff");
        }
    }

    private void BtnForControll(bool active)//кнопки контроля
    {
        right_btn.SetActive(active);
        left_btn.SetActive(active);
    }

    private void DistanceAnim(bool is_active)//анимации дистанции
    {
        if (is_active)
        {
            distance_anim.Play("distance_pause_on");
        }
        else
        {
            distance_anim.Play("distance_pause_off");
        }
    }

    private void BestMenu(bool is_active)//анимации меню рекордов
    {
        if (is_active)
        {
            best_menu.SetActive(true);
            SetBestDistTxt();
            SetBestSpeedTxt();
            best_menu_anim.Play("best_menu_on");
        }
        else
        {
            best_menu_anim.Play("best_menu_off");
        }
    }

    private void ChechBestValues()//проверяем на рекордные значения
    {
        if (!is_polygon) {
            if (CalculateSpeed() > best_speed_value)
            {
                best_speed_value = CalculateSpeed();
                is_best_speed = true;
            }

            if (player_controller.GetDistance() > best_dist_value)
            {
                best_dist_value = player_controller.GetDistance();
                is_best_dist = true;
            }
        }
    }

    public void PhotoModeOn()//включение фото режиа
    {
        SpeedDistAmim(false);
        RightMenu(false, is_restart, false);
        BestMenu(false);

        photo_cam.SetActive(true);
        main_cam.SetActive(false);
    }

    public void PhotoModeOff()//выключение фото режима
    {
        SpeedDistAmim(true);
        RightMenu(true, is_restart);
        BestMenu(true);

        photo_cam.SetActive(false);
        main_cam.SetActive(true);
    }

    private void SpeedDistAmim(bool is_active)//анимация меню скорости и дистанции
    {
        if (is_active)
        {
            data_menu_anim.Play("data_menu_on");
        }
        else
        {
            data_menu_anim.Play("data_menu_off");
        }
    }

    private void SpeedArrowControll()
    {
        float new_z;
        new_z = min_arrow_rot - CalculateSpeed();
        //arrow_tr.rotation = new Quaternion(arrow_tr.rotation.x, arrow_tr.rotation.y,new_z, arrow_tr.rotation.w);
        if (new_z >= max_arrow_rot && new_z <= min_arrow_rot) {
            arrow_tr.eulerAngles = new Vector3(arrow_tr.eulerAngles.x, arrow_tr.eulerAngles.y, new_z);
        }
    }

    public void TestGarageBtn()//тестовая кнопка гаража
    {
        if (!is_change)
        {
            is_change = true;
        }
        else
        {
            is_change = false;
        }
        TestChangeCarBtns(is_change);
    }

    private void TestChangeCarBtns(bool active)//задаем значения активации тестовых кнопок выбора
    {
        which_car_txt.SetActive(active);
        next_btn.SetActive(active);
        back_btn.SetActive(active);
    }

    public void TestExitBtn()//тестовая кнопка выхода для перехода на полигон
    {
        SceneManager.LoadScene(next_scene);
    }

    private void SetDriftScore()//устанавливаем и считаем дрифт текст
    {
        if (is_drift && !is_pause && !is_restart)
        {
            if (go_ds_anim)
            {
                drift_score = 0f;
                drift_score_txt.text = "";
                drift_score_comp.SetActive(false);
                drift_score_anim.Play("drift_score_app");
                go_ds_anim = false;
                end_ds_anim = true;
            }

            if (!is_drift_anim.is_anim)
            {
                drift_score_anim.Play("drift_score_idle");
            }

            drift_score += Time.deltaTime * drift_score_koef;
            drift_score_comp.SetActive(true);
            drift_score_txt.text = drift_score.ToString("f0");

        }
        else
        {
            if (end_ds_anim)
            {
                go_ds_anim = true;
                drift_score_anim.Play("drift_score_end");
                end_ds_anim = false;
            }
            
        }
    }

    private void HandbrakeBtn()//эффекты кнопки ручника
    {
        if (!is_pause) {
            if (handbrake_btn_down && handbrake_rollback_value <= 0f)
            {
                handbrake_active.SetActive(true);
                handbrake_idle.SetActive(false);
            }
            else
            {
                handbrake_active.SetActive(false);
                handbrake_idle.SetActive(true);
            }

            if (handbrake_rollback_value > 0f)
            {
                handbrake_img.color = overheat_handbrake;
            }
            else
            {
                handbrake_img.color = start_handbrale;
            }

            FillHandBrakeBtn();
        }
    }

    private void FillHandBrakeBtn()//заполняем кнопку ручника
    {
        if (handbrake_rollback_value > 0f) 
        {
            fill_value = 1f - (handbrake_rollback_value / rollback_time);
        }
        else
        {
            fill_value = 1f - (overheat_value / max_over_heat_value);
        }

        if (fill_value >1f)
        {
            fill_value = 1f;
        }
        if (fill_value < 0f)
        {
            fill_value = 0f;
        }
        handbrake_img.fillAmount = fill_value;
    }

    public void NextCar()
    {
        car_define_index++;
        if (car_define_index >= car_define.Length)
        {
            car_define_index = 0;
        }
        PlayerPrefs.SetString("car_define", car_define[car_define_index]);
        ChangeCarTxt();
    }

    public void BackCar()
    {
        car_define_index--;
        if (car_define_index < 0)
        {
            car_define_index = car_define.Length-1;
        }
        PlayerPrefs.SetString("car_define", car_define[car_define_index]);
        ChangeCarTxt();
    }

    public void RestartGame()//перезапускаем игру
    {
        day_cycly_controller.CountTimeOfDay();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void GoPauseCount()//обратный отсчет до начала игры
    {
        if (pause_countdown_value > min_countdown_value) 
        {
            is_end_count = false;
            pause_count_down.SetActive(true);
            pause_countdown_value -= Time.fixedDeltaTime;
            pause_count_down_txt.text = Mathf.Round(pause_countdown_value).ToString("f0");

            if (last_cd_value != Mathf.RoundToInt(pause_countdown_value))
            {
                pause_cd_anim.Play("pause_cd");
                last_cd_value = Mathf.RoundToInt(pause_countdown_value);
            }
        }
        if (pause_countdown_value < min_countdown_value)
        {
            pause_countdown_value = min_countdown_value;
        }
        if (pause_countdown_value == min_countdown_value && !is_end_count)
        {
            pause_count_down.SetActive(false);
            player_controller.SetPlayGame();
            is_pause = false;

            is_end_count = true;
        }
    }

    public void PauseOn()//включаем паузу
    {
        if (is_end_count) {
            player_controller.SetIsPause();
            is_pause = true;
            PauseMenu();
        }
    }

    public void PlayGame()//возоюновляем игру
    {
        pause_countdown_value = max_countdown_value;
        MainGameMenu();
        RightMenu(false);
        //player_controller.SetPlayGame();
        //MainGameMenu();
        //RightMenu(false);

        //is_pause = false;
    }

    public void HandbrakeDown()//нажат ручник
    {
       handbrake_btn_down = true;
    }

    public void HandbrakeUp()//поднят ручник
    {
        handbrake_btn_down = false;
    }

    private void CalculateTime()//считаем общее вермя
    {
        time_sec += Time.fixedDeltaTime;
    }

    private float CalculateSpeed()//считаем скорость
    {
        return player_controller.GetLastSpeed() * speed_koef;
    }

    private void SetSpeedTxt()//устанавливаем скорость
    {
        speed_text.text = CalculateSpeed().ToString("f0");
    }

    private void SetDistanceTxt()//устанавливаем дистанцию
    {
        distance = player_controller.GetDistance();
        dist_text.text = distance.ToString("f0");
    }

    private void SetBestSpeedTxt()//устанавливаем рекорд скорости и сохраняем
    {
        if (best_speed_value > PlayerPrefs.GetFloat("best_speed"))
        {
            best_speed.text = best_speed_value.ToString("f0");
            PlayerPrefs.SetFloat("best_speed", best_speed_value);
        }
        else
        {
            best_speed.text = PlayerPrefs.GetFloat("best_speed").ToString("f0");
        }
        if (is_best_speed) {
            speed_hightscore.SetActive(true);
            speed_hightscore_anim.Play("highscore_idle");
        }
    }

    private void SetBestDistTxt()//устанавливаем рекорд дистанции и сохраняем
    {
        if (best_dist_value > PlayerPrefs.GetFloat("best_dist"))
        {
            best_dist.text = best_dist_value.ToString("f0");
            PlayerPrefs.SetFloat("best_dist", best_dist_value);
        }
        else
        {
            best_dist.text = PlayerPrefs.GetFloat("best_dist").ToString("f0");
        }
        if (is_best_dist)
        {
            dist_hightscore.SetActive(true);
            dist_hightscore_anim.Play("highscore_idle");
        }
    }

    private void SetFpsTxt()
    {
        if (!photo_cam.activeSelf) 
        {
            fps_text.text = Mathf.Clamp(fps_counter.AverageFPS, 0, 99).ToString() + "fps";
        }
        else
        {
            fps_text.text = "";
        }
    }

    private void GetEnd()
    {
        if (player_controller.GetIsEndGame())
        {
            if (!is_restart)
            {
                RestartMenu();
                is_restart = true;
            }
        }
    }

    public bool GetIsPause()
    {
        return is_pause;
    }

    public bool GetIsEndCount()
    {
        return is_end_count;
    }

    public bool GetIsRestart()
    {
        return is_restart;
    }

}
