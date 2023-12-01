using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadController : MonoBehaviour
{

    LetSpawnController let_spawn_controller;

    public GameObject straight_line;
    public GameObject straight_line_lamp;
    public GameObject straight_bus_stop;
    public GameObject turn_90_right, turn_90_left;
    public GameObject turn_45_right, turn_45_left;
    public GameObject turn_180_right, turn_180_left;

    public GameObject turn_90_let_right, turn_90_let_left;
    public GameObject fall_lamp_road;

    private List<GameObject> straight_roads = new List<GameObject>();
    private List<GameObject> straight_lamp_roads = new List<GameObject>();
    private List<GameObject> straight_bus_stop_roads = new List<GameObject>();
    private List<GameObject> turn_90_right_roads = new List<GameObject>();
    private List<GameObject> turn_90_left_roads = new List<GameObject>();
    private List<GameObject> turn_90_let_right_roads = new List<GameObject>();
    private List<GameObject> turn_90_let_left_roads = new List<GameObject>();
    private List<GameObject> turn_45_right_roads = new List<GameObject>();
    private List<GameObject> turn_45_left_roads = new List<GameObject>();
    private List<GameObject> turn_180_right_roads = new List<GameObject>();
    private List<GameObject> turn_180_left_roads = new List<GameObject>();
    private List<GameObject> fall_lamp_roads = new List<GameObject>();

    private float straight_road_end;//значение центра после концап для прямой
    private Quaternion straight_rot;
    private float turn_90_end;//значение центра после конца для поворота 90
    private float turn_45_end;//значение центра после конца для поворота 45
    private float turn_180_end;//значение центра после конца для поворота 180
    private float turn_90_let_end;//значение центра после конца для поворота-тупика 90
    private Quaternion turn_rot;
    private Quaternion turn_last_rot;

    private int road_number;
    private float turn_kol_left, turn_kol_right;
    private int straight_kol, max_straight;
    private short lamp_counter, max_lamp_counter;//количество дорог без фонарей, макс дорог без фонарей
    private bool can_fall_lamp_road;//можем ли спавнить упавший фонарь
    private int bus_stop_counter, max_bus_stop_counter;//количество дорог без остановок, макс дорог без остановок
    private bool is_bus_stop_road;
    private bool is_turn_90_let;
    private bool is_fall_lamp_let;

    private float curr_x, curr_y, curr_z;
    private Vector3 current_part_pos, end_part_pos;

    private int road_part_pool;
    private bool is_anim;

    private Vector3 straight_move;
    private Vector3 turn_90_r_move, turn_90_l_move;
    private Vector3 turn_45_r_move, turn_45_l_move;
    private Vector3 turn_180_r_move, turn_180_l_move;
    private Vector3 turn_90_let_move;

    void Start()
    {
        AllStartValues();
    }

    private void AllStartValues()//начальные значения
    {
        let_spawn_controller = GetComponent<LetSpawnController>();

        road_part_pool = 10;//10
        turn_kol_left = turn_kol_right = 0;
        straight_kol = 1;
        max_straight = 3;
        is_turn_90_let = false;
        is_fall_lamp_let = false;
        road_number = 0;

        lamp_counter = 1;
        max_lamp_counter = 2;
        can_fall_lamp_road = false;
        max_bus_stop_counter = 10;
        bus_stop_counter = Random.Range(0,max_bus_stop_counter);
        is_bus_stop_road = false;

        straight_road_end = 10f;//10f
        turn_90_end = 9.745f;//9.5f
        turn_45_end = 10f;//10f
        turn_180_end = 10f;//10f
        turn_90_let_end = 9.43f;//9.4f

        straight_rot = new Quaternion(0f, 0f, 0f, 0f);
        turn_rot = new Quaternion(0f, 0f, 0f, 0f);
        turn_last_rot = turn_rot;

        straight_move = new Vector3(0f, 0f, 1f);
        turn_90_r_move = new Vector3(0f, 0f, -1f);
        turn_90_l_move = new Vector3(1f, 0f, 0f);
        turn_45_r_move = new Vector3(0f, 0f, -1f);
        turn_45_l_move = new Vector3(0f, 0f, -1f);
        turn_180_r_move = new Vector3(0f, 0f, -1f);
        turn_180_l_move = new Vector3(0f, 0f, -1f);
        turn_90_let_move = new Vector3(0f, 0f, -1f);

        curr_x = curr_y = curr_z = 0f;
        end_part_pos = new Vector3(0, 0, straight_road_end);

        is_anim = false;

        CalcCurrentVectorPos();

        FillThePool();

        FirstParts();
        //TestSpawn();
    }

    private void FirstParts()//спавн первыхчастей дороги
    {
        for (int i = 0; i < road_part_pool; i++)
        {
            ChooseRoadPart();
        }
        is_anim = true;
    }

    void TestSpawn()
    {
        StraightSpawn();
        StraightSpawn();
        StraightSpawn();
        StraightSpawn();
        StraightSpawn();
        StraightSpawn();
        StraightSpawn();
        StraightSpawn();
        StraightSpawn();
        StraightSpawn();
        StraightSpawn();
        StraightSpawn();
        StraightSpawn();
        StraightSpawn();
        StraightSpawn();
    }

    public void ChooseRoadPart()//выбор части дороги для спавна
    {
        if (!is_turn_90_let && !is_fall_lamp_let)
        {
            int rand_road_part;
            rand_road_part = Random.Range(0,4);
            //rand_road_part = 0;//спавним только прямые для теста

            switch (rand_road_part)
            {
                case 1:
                    TurnRoadSpawn();
                    break;
                default:
                    StraightRoadSpawn();
                    break;

            }
        }
        else
        {
            if (is_turn_90_let) {
                Turn90LetRoadSpawn();
                is_turn_90_let = false;
            }
            if (is_fall_lamp_let)
            {
                FallLampRoadSpawn();
                is_fall_lamp_let = false;
            }
        }

    }

    private void StraightRoadSpawn()//спавним ли прямую дорогу
    {
        if (straight_kol < max_straight)
        {
            StraightSpawn();
            straight_kol++;
        }
        else
        {
            TurnRoadSpawn();
            straight_kol = 0;
        }
    }

    private void StraightSpawn()//спавн прямой части
    {

        DefineCurCord();
        CalcCurrentVectorPos();
        int which_straight = WhichStraightsSpawn();
        List<GameObject> straights = WhichStraightsList(which_straight);
        GameObject straight = WhichStraightsPref(which_straight);
        GameObject pref =  SpawnFormList(straights, straight, straight_rot, straight_move, straight_road_end);
        if (!is_bus_stop_road)
        {
            SpawnLet(pref);
        }

    }

    private GameObject WhichStraightsPref(int which_straight)
    {
        if (which_straight == 0)
        {
            return straight_line;
        }
        else if (which_straight == 1)
        {
            return straight_bus_stop;
        }
        else if (which_straight == 2)
        {
            return straight_line_lamp;
        }
        return straight_line;
    }

    private List<GameObject> WhichStraightsList(int which_straight)
    {
        if (which_straight == 0)
        {
            return straight_roads;
        }
        else if (which_straight == 1)
        {
            return straight_bus_stop_roads;
        }
        else if(which_straight == 2)
        {
            return straight_lamp_roads;
        }
        return straight_roads;
    }

    private int WhichStraightsSpawn()//выбираем какую прямую спавним (с фонарем или без)
    {
        if (lamp_counter < max_lamp_counter)
        {
            if (lamp_counter == max_lamp_counter - 1)//если следущий будет фонарь то даем возможность заспавнить сломанный фонарь
            {
                can_fall_lamp_road = true;
            }
            else
            {
                can_fall_lamp_road = false;
            }
            lamp_counter++;

            if (bus_stop_counter < max_bus_stop_counter)
            {
                bus_stop_counter++;
                is_bus_stop_road = false;
                //return straight_roads;
                return 0;//прямая
            }
            else
            {
                bus_stop_counter = 0;
                is_bus_stop_road = true;
                //return straight_bus_stop_roads;
                return 1;//прямая с остановкой
            }

        }
        else
        {
            lamp_counter = 0;
            can_fall_lamp_road = false;
            is_bus_stop_road = false;
            //return straight_lamp_roads;
            return 2;//прямая с остановкой
        }
    }

    private void TurnRoadSpawn()//выбираем какой поворот ставим
    {
        int rand_turn;
        rand_turn = Random.Range(0, 3);//0 , 3

        switch (rand_turn)
        {
            case 0:
                Turn90RoadSpawn();
                break;
            case 1:
                Turn45RoadSpawn();
                break;
            case 2:
                Turn180RoadSpawn();
                break;
            default:
                Turn90RoadSpawn();
                break;
        }
        let_spawn_controller.PlusEmptyRoad();
    }

    private void Turn90RoadSpawn()//спавн поворота 90 (переписать спавн поворота в 90 копируя с прямой)
    {
        DefineCurCord();

        int rand_turn;
        rand_turn = DefineRandTurn(1f);
        DefineTurn90Dir(rand_turn);
        CalcCurrentVectorPos();
        if (rand_turn == 0)//правый
        {
            SpawnFormList(turn_90_right_roads, turn_90_right, turn_rot, turn_90_r_move, turn_90_end);
        }
        else if (rand_turn == 1)//левый
        {
            SpawnFormList(turn_90_left_roads, turn_90_left, turn_rot, turn_90_l_move, turn_90_end);
        }
    }

    private int DefineTurn90Dir(int rand_turn)//задаем угол поворота 90
    {
        float turn_y = 0f, last_turn_y = 0f;
        float turn_value = 90f;
        if (rand_turn == 0)//правый
        {
            turn_y = Mathf.Round(turn_last_rot.eulerAngles.y) + 180f;
            last_turn_y = Mathf.Round(turn_last_rot.eulerAngles.y) + turn_value;
            CalcRotTurn(turn_value);
        }
        if (rand_turn == 1)//левый
        {
            turn_y = Mathf.Round(turn_last_rot.eulerAngles.y) + 270f;

            if (Mathf.Round(turn_last_rot.eulerAngles.y) > 0f) {
                last_turn_y = Mathf.Round(turn_last_rot.eulerAngles.y) - turn_value;
            }
            else
            {
                last_turn_y = Mathf.Round(360f - turn_value);
            }

            CalcRotTurn(-turn_value);
        }
        if (Mathf.Round(last_turn_y)==360f)
        {
            last_turn_y = 0f;
        }
        turn_last_rot = Quaternion.AngleAxis(Mathf.Round(last_turn_y), new Vector3(0f, 1f, 0f));
        turn_rot = Quaternion.AngleAxis(Mathf.Round(turn_y), new Vector3(0f, 1f, 0f));
        return rand_turn;
    }

    private void Turn90LetRoadSpawn()//спавн тупика 90
    {
        DefineCurCord();

        int rand_turn;
        rand_turn = DefineRandTurn(1f);
        DefineTurn90LetDir(rand_turn);
        CalcCurrentVectorPos();
        GameObject pref;
        if (rand_turn == 0)
        {//правый;
            pref = SpawnFormList(turn_90_let_right_roads, turn_90_let_right, turn_rot, turn_90_let_move, turn_90_let_end);
            pref.GetComponent<T90LetController>().ReSpawn();
        }
        else if (rand_turn == 1)//левый
        {
            pref = SpawnFormList(turn_90_let_left_roads, turn_90_let_left, turn_rot, turn_90_let_move, turn_90_let_end);
            pref.GetComponent<T90LetController>().ReSpawn();
        }

    }

    private int DefineTurn90LetDir(int rand_turn)//задаем угол тупика 90
    {
        float turn_y = 0f, last_turn_y = 0f;
        float turn_value = 90f;
        if (rand_turn == 0)//правый
        {
            turn_y = Mathf.Round(turn_last_rot.eulerAngles.y) + 180f;
            last_turn_y = Mathf.Round(turn_last_rot.eulerAngles.y) + turn_value;
            CalcRotTurn(turn_value);
        }
        if (rand_turn == 1)//левый 
        {
            turn_y = Mathf.Round(turn_last_rot.eulerAngles.y) + 180f;
            if (Mathf.Round(turn_last_rot.eulerAngles.y) > 0f)
            {
                last_turn_y = Mathf.Round(turn_last_rot.eulerAngles.y - turn_value);
            }
            else
            {
                last_turn_y = Mathf.Round(360f - turn_value);
            }
            CalcRotTurn(-turn_value);
        }
        if (Mathf.Round(last_turn_y) == 360f)
        {
            last_turn_y = 0f;
        }
        turn_last_rot = Quaternion.AngleAxis(Mathf.Round(last_turn_y), new Vector3(0f, 1f, 0f));
        turn_rot = Quaternion.AngleAxis(Mathf.Round(turn_y), new Vector3(0f, 1f, 0f));
        return rand_turn;
    }

    private void Turn45RoadSpawn()//спавн поворота 45
    {
        DefineCurCord();

        int rand_turn;
        rand_turn = DefineRandTurn(0.5f);
        DefineTurn45Dir(rand_turn);
        CalcCurrentVectorPos();
        if (rand_turn == 0)
        {//правый
            SpawnFormList(turn_45_right_roads, turn_45_right, turn_rot, turn_45_r_move, turn_45_end);
        }
        else if (rand_turn == 1)//левый
        {
            SpawnFormList(turn_45_left_roads, turn_45_left, turn_rot, turn_45_l_move, turn_45_end);
        }

    }

    private int DefineTurn45Dir(int rand_turn)//задаем угол поворота 45
    {
        float turn_y = 0f, last_turn_y = 0f;
        float turn_value = 45f;
        if (rand_turn == 0)//правый
        {
            turn_y = Mathf.Round(turn_last_rot.eulerAngles.y) + 180f;
            last_turn_y = Mathf.Round(turn_last_rot.eulerAngles.y + turn_value);
            CalcRotTurn(turn_value);
        }
        if (rand_turn == 1)//левый
        {
            turn_y = Mathf.Round(turn_last_rot.eulerAngles.y) + 180f;
            if (Mathf.Round(turn_last_rot.eulerAngles.y) > 0f)
            {
                last_turn_y = Mathf.Round(turn_last_rot.eulerAngles.y - turn_value);
            }
            else
            {
                last_turn_y = Mathf.Round(360f - turn_value);
            }
            CalcRotTurn(-turn_value);
        }

        if (Mathf.Round(last_turn_y) == 360)
        {
            last_turn_y = 0f;
        }

        turn_last_rot = Quaternion.AngleAxis(Mathf.Round(last_turn_y), new Vector3(0f, 1f, 0f));
        turn_rot = Quaternion.AngleAxis(Mathf.Round(turn_y), new Vector3(0f, 1f, 0f));
        return rand_turn;
    }

    private void Turn180RoadSpawn()//спавн поворота 180
    {
        DefineCurCord();

        int rand_turn;
        rand_turn = DefineRandTurn180();
        if (rand_turn < 2) {
            DefineTurn180Dir(rand_turn);
            CalcCurrentVectorPos();
            if (rand_turn == 0)
            {//правый
                SpawnFormList(turn_180_right_roads, turn_180_right, turn_rot, turn_180_r_move, turn_180_end);
            }
            else if (rand_turn == 1)//левый
            {
                SpawnFormList(turn_180_left_roads, turn_180_left, turn_rot, turn_180_l_move, turn_180_end);
            }
        }
        else
        {
            TurnRoadSpawn();
        }

    }

    private int DefineTurn180Dir(int rand_turn)//задаем угол поворота 180
    {
        float turn_y = 0f, last_turn_y = 0f;
        float turn_value = 180f;
        if (rand_turn == 0)//правый
        {
            turn_y = Mathf.Round(turn_last_rot.eulerAngles.y) + 180f;
            last_turn_y = Mathf.Round(turn_last_rot.eulerAngles.y + turn_value);
            CalcRotTurn(turn_value);
        }
        if (rand_turn == 1)//левый
        {
            turn_y = Mathf.Round(turn_last_rot.eulerAngles.y) + 180f;
            if (Mathf.Round(turn_last_rot.eulerAngles.y) > 0f)
            {
                last_turn_y = Mathf.Round(turn_last_rot.eulerAngles.y - turn_value);
            }
            else
            {
                last_turn_y = Mathf.Round(360f - turn_value);
            }
            CalcRotTurn(-turn_value);
        }

        if (Mathf.Round(last_turn_y) == 360)
        {
            last_turn_y = 0f;
        }

        turn_last_rot = Quaternion.AngleAxis(Mathf.Round(last_turn_y), new Vector3(0f, 1f, 0f));
        turn_rot = Quaternion.AngleAxis(Mathf.Round(turn_y), new Vector3(0f, 1f, 0f));
        return rand_turn;
    }

    private int DefineRandTurn(float koef)//определяем сторону поворота
    {
        int rand_turn_loc;
        rand_turn_loc = Random.Range(0, 2);
        if (rand_turn_loc == 0)
        {
            if (turn_kol_right + koef > 2f)
            {
                turn_kol_right -= koef;
                turn_kol_left += koef;
                return 1;
            }
            if (turn_kol_right < 2)
            {
                turn_kol_right += koef;
                if (turn_kol_left > 0)
                {
                    turn_kol_left -= koef;
                    if (turn_kol_left< 0)
                    {
                        turn_kol_left = 0;
                    }
                }
            }
        }
        if (rand_turn_loc == 1)
        {
            if (turn_kol_left + koef > 2f)
            {
                turn_kol_right += koef;
                turn_kol_left -= koef;
                return 0;
            }
            if (turn_kol_left < 2)
            {
                turn_kol_left += koef;
                if (turn_kol_right > 0)
                {
                    turn_kol_right -= koef;
                    if (turn_kol_right < 0)
                    {
                        turn_kol_right = 0;
                    }
                }
                
            }
        }
        return rand_turn_loc;
    }

    private int DefineRandTurn180()//определяем сторону поворота180
    {
        int rand_turn_loc;
        rand_turn_loc = Random.Range(0, 2);
        if (rand_turn_loc == 0)//правый
        {
            if (turn_kol_right == 2)
            {
                turn_kol_right-=2;
                turn_kol_left += 2;
                return 1;
            }
            if (turn_kol_right == 0 )
            {
                turn_kol_right += 2;
                if (turn_kol_left > 0)
                {
                    turn_kol_left = 0;
                }
            }
            if (turn_kol_right < 2)
            {
                return 2;
            }

        }
        if (rand_turn_loc == 1)//левый
        {
            if (turn_kol_left == 2)
            {
                turn_kol_right += 2;
                turn_kol_left -= 2;
                return 0;
            }
            if (turn_kol_left == 0)
            {
                turn_kol_left += 2;
                if (turn_kol_right > 0)
                {
                    turn_kol_right = 0;
                }
            }
            if (turn_kol_left < 2)
            {
                return 2;
            }
        }
        return rand_turn_loc;
    }

    private void FallLampRoadSpawn()//спавним дорогу со сломаной лампой
    {
        lamp_counter = 0;
        DefineCurCord();

        CalcCurrentVectorPos();
        SpawnFormList(fall_lamp_roads, fall_lamp_road, straight_rot, straight_move, straight_road_end);
    }

    private void MoveRoadPart(GameObject pref,Vector3 dir,float distance)//двигаем кусок дороги по его направлению движения
    {
        pref.transform.Translate(dir * distance);
    }

    private void CalcRotTurn(float dir)//вычисляем поворот для кусков (90)
    {
        straight_rot = Quaternion.AngleAxis(straight_rot.eulerAngles.y + dir, new Vector3(0f, 1f, 0f));
    }

    private void CalcCurrentVectorPos()//вычисляем нынешнюю позицию куска
    {
        current_part_pos = new Vector3(curr_x, curr_y, curr_z);//позиция куска
    }

    private void DefineCurCord()//определяем координаты для позиции куска
    {
        curr_x = end_part_pos.x;
        curr_y = end_part_pos.y;
        curr_z = end_part_pos.z;
    }

    private int DefineDeletePart(List<GameObject> lst)//определяем есть ли отключенные
    {
        if (lst.Count == 0)
        {
            return -1;
        }
        for (int i = 0; i < lst.Count; i++) 
        {
            if (!lst[i].activeSelf)
            {
                return i;
            }
        }
        return -1;
    }

    private void FillThePool()//наполним пул при старте готовыми выключеными объектами
    {
        GameObject pref;
        for (int i = 0; i < 8; i++) //прямые
        {
            pref = SpawnRoadObject(straight_line, current_part_pos, straight_rot);
            pref.SetActive(false);
            straight_roads.Add(pref);
        }
        for (int i = 0; i < 4; i++) //с лампой и повороты
        {
            pref = SpawnRoadObject(straight_line_lamp, current_part_pos, straight_rot);
            pref.SetActive(false);
            straight_lamp_roads.Add(pref);
            pref = SpawnRoadObject(turn_90_right, current_part_pos, straight_rot);
            pref.SetActive(false);
            turn_90_right_roads.Add(pref);
            pref = SpawnRoadObject(turn_90_left, current_part_pos, straight_rot);
            pref.SetActive(false);
            turn_90_left_roads.Add(pref);
            pref = SpawnRoadObject(turn_45_right, current_part_pos, straight_rot);
            pref.SetActive(false);
            turn_45_right_roads.Add(pref);
            pref = SpawnRoadObject(turn_45_left, current_part_pos, straight_rot);
            pref.SetActive(false);
            turn_45_left_roads.Add(pref);
        }
        for (int i = 0; i < 3; i++) //повороты 180
        {
            pref = SpawnRoadObject(turn_180_right, current_part_pos, straight_rot);
            pref.SetActive(false);
            turn_180_right_roads.Add(pref);
            pref = SpawnRoadObject(turn_180_left, current_part_pos, straight_rot);
            pref.SetActive(false);
            turn_180_left_roads.Add(pref);
        }
        for (int i = 0; i < 2; i++) //остановка и тупик
        {
            pref = SpawnRoadObject(straight_bus_stop, current_part_pos, straight_rot);
            pref.SetActive(false);
            straight_bus_stop_roads.Add(pref);
            pref = SpawnRoadObject(turn_90_let_right, current_part_pos, straight_rot);
            pref.SetActive(false);
            turn_90_let_right_roads.Add(pref);
            pref = SpawnRoadObject(turn_90_let_left, current_part_pos, straight_rot);
            pref.SetActive(false);
            turn_90_let_left_roads.Add(pref);
        }
        for (int i = 0; i < 1; i++) //остановка и тупик
        {
            pref = SpawnRoadObject(fall_lamp_road, current_part_pos, straight_rot);
            pref.SetActive(false);
            fall_lamp_roads.Add(pref);
        }
    }

    private GameObject SpawnFormList(List<GameObject> lst_part,GameObject part,Quaternion rot,
        Vector3 move_part,float part_end)
    {
        int ind = DefineDeletePart(lst_part);
        if (ind > -1)
        {
            lst_part[ind].GetComponent<RoadPartController>().ReSpawn();
            lst_part[ind].transform.position = current_part_pos;
            lst_part[ind].transform.rotation = rot;
            lst_part[ind].SetActive(true);
            MoveRoadPart(lst_part[ind], move_part, part_end);
            end_part_pos = lst_part[ind].GetComponent<RoadPartController>().GetEndPos().position;
            PlusRoadNum();
            return lst_part[ind];
        }
        else
        {
            GameObject pref = SpawnRoadObject(part, current_part_pos, rot);
            MoveRoadPart(pref, move_part, part_end);
            end_part_pos = pref.GetComponent<RoadPartController>().GetEndPos().position;
            lst_part.Add(pref);
            return pref;
        }
    }//спавним объекты из листа или передвигем если есть свободные

    private GameObject SpawnRoadObject(GameObject road_part, Vector3 road_part_pos,Quaternion road_part_rot)//общий метод для спавна куска дороги по переданным значениям
    {
        GameObject pref = Instantiate(road_part, road_part_pos, road_part_rot) as GameObject;
        PlusRoadNum();
        return pref;
    }

    private void SpawnLet(GameObject pref)//метод для спавна препятствия
    {
        let_spawn_controller.SpawnLet(pref.GetComponent<RoadPartController>().GetStartBorderTransform(),
        pref.GetComponent<RoadPartController>().GetEndBorderTransform(), straight_rot, pref.transform.position,can_fall_lamp_road);
    }

    public void MoreHarder()//сделать сложнее
    {
        max_straight = 2;
    }

    public void MinusRoadNum()//вычесть количеств дорог
    {
        road_number -= 1;
    }

    private void PlusRoadNum()//прибавить количество дорог
    {
        road_number += 1;
    }

    public void SetIsTurn90Let()
    {
        is_turn_90_let = true;
    }

    public void SetIsFallLampLet()
    {
        is_fall_lamp_let = true;
    }

    public int GetRoadNum()
    {
        return road_number;
    }
}
