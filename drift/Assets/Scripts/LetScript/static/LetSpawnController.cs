using System.Collections.Generic;
using UnityEngine;

public class LetSpawnController : MonoBehaviour
{
    RoadController road_controller;

    private GameObject pref;

    //static
    public GameObject open_well;
    public GameObject road_car_worker;
    public GameObject conus;
    public GameObject broken_car;
    public GameObject crush_truck;

    //dynamic
    public GameObject lex_type_bot;
    public GameObject bus_type_bot;
    public GameObject pickup_type_bot;
    public GameObject vaz_type_bot;
    public GameObject mits_truck;

    //lists
    private List<GameObject> open_well_lets = new List<GameObject>();
    private List<GameObject> road_car_worker_lets = new List<GameObject>();
    private List<GameObject> broken_car_lets = new List<GameObject>();
    private List<GameObject> crush_truck_lets = new List<GameObject>();
    private List<GameObject> bus_type_lets = new List<GameObject>();
    private List<GameObject> pickup_type_lets = new List<GameObject>();
    private List<GameObject> lex_type_lets = new List<GameObject>();
    private List<GameObject> vaz_type_lets = new List<GameObject>();
    private List<GameObject> mits_truck_type_lets = new List<GameObject>();

    private float cur_x, cur_y, cur_z;
    private Vector3 current_let_pos;
    private Quaternion let_rot;
    private Vector3 start_let_border, end_let_border;
    private Vector3 current_straight_pos;

    private float max_rot_offset = 8f;
    private float open_well_rot_add = 0f;
    private float open_well_y_add = 0.276f;//0.276
    private float road_car_worker_rot_add = 180f;
    private float road_car_worker_y_add = 0.288f;//0.288
    private float broken_car_y_add = 0.279f;//0.279
    private float bot_car_y_add = 0.25f;//0.25
    private float crush_truck_y_add = 1.188f;//1.188

    private int max_random_chance = 10;
    private short empty_road_kol;
    private short max_empty_road, min_empty_road;//максимальное и минимальное количество пустых дорог 
    private int last_static_let;
    private int max_static_let;
    private int max_random_dynamic;
    private int without_dyn_let;
    private int max_without_dyn_let;
    private bool is_lamp_road;

    private float let_border_vert_length = 3.6f;

    void Awake()
    {
        AllStartValues();
    }

    private void AllStartValues()//начальные значения
    {
        current_let_pos = new Vector3(0f,0f,0f);
        let_rot = new Quaternion(0f,0f,0f,0f);

        max_random_dynamic = 10;//15

        empty_road_kol = 3;
        max_empty_road = 10;//5 количество макс дорог без статик лэт
        min_empty_road = 3;

        is_lamp_road = false;

        without_dyn_let = 3;
        max_without_dyn_let = 2;//4

        last_static_let = -1;

        road_controller = gameObject.GetComponent<RoadController>();
    }

    public void SpawnLet(Vector3 s_l_border, Vector3 e_l_border, Quaternion straight_rot, Vector3 straight_pos, 
        bool lamp_road)//публичный метод для дальнейшего спанва
    {
        start_let_border = s_l_border;
        end_let_border = e_l_border;
        let_rot = straight_rot;
        current_straight_pos = straight_pos;
        is_lamp_road = lamp_road;

        IsSpawnLet();
    }

    private void IsSpawnLet()//спавним ли препятствие
    {
        int random_let_num = Random.Range(0,max_random_chance);

        if(empty_road_kol >= max_empty_road)
        {
            random_let_num = 0;
        }

        if (random_let_num == 0 && empty_road_kol >= min_empty_road)
        {
            ChooseRandomStaticLet();
            empty_road_kol = 0;
        }
        else
        {
            PlusEmptyRoad();
            IsSpawnDynamicLet();
        }
    }

    private void ChooseRandomStaticLet()//выбор препятствия
    {
        int rand_let;
        max_static_let = 6;//количество на выбор препятсвий + 1 ((6) вернуть)

        rand_let = Random.Range(0, max_static_let);

        if (rand_let == last_static_let || (rand_let == 5 && !is_lamp_road))//если прошлый или если фонарь но бещ фонаря
        {
            ChooseRandomStaticLet();
            return;
        }
        last_static_let = rand_let;

        switch (rand_let)
        {
            case 0:
                OpenWellSpawn();
                break;
            case 1:
                RoadCarWorkerSpawn();
                break;
            case 2:
                BrokenCarSpawn();
                break;
            case 3:
                Turn90LetSpawn();
                break;
            case 4:
                CrushTruckSpawn();
                break;
            case 5:
                FallLampRoadSpawn();
                break;
            default:
                Turn90LetSpawn();
                break;
        }
    }

    private void IsSpawnDynamicLet()//спавним ли динамическое препятствие
    {
        int is_dynamic;
        if (without_dyn_let >= max_without_dyn_let)
        {
            is_dynamic = 0;
        }
        else
        {
            is_dynamic = Random.Range(0, max_random_dynamic);
        }

        if (is_dynamic == 0)
        {
            ChooseRandomDynamicLet();
            without_dyn_let = 0;
        }
        else
        {
            without_dyn_let++;
        }
    }

    private void ChooseRandomDynamicLet()//выбор препятствия
    {
        int rand_let;
        rand_let = Random.Range(0, 5);
        switch (rand_let)
        {
            case 0:
                BotCarSpawn(bus_type_lets,bus_type_bot);
                break;
            case 1:
                BotCarSpawn(lex_type_lets,lex_type_bot);
                break;
            case 2:
                BotCarSpawn(pickup_type_lets,pickup_type_bot);
                break;
            case 3:
                BotCarSpawn(vaz_type_lets, vaz_type_bot);
                break;
            case 4:
                BotCarSpawn(mits_truck_type_lets, mits_truck);
                break;
            default:
                BotCarSpawn(bus_type_lets, bus_type_bot);
                break;
        }
    }

    private void OpenWellSpawn()//спавн открытого люка
    {
        CalculateStaticLetPos(open_well_y_add);
        CalculateLetRot(open_well_rot_add);

        if(SpawnLetFormList(open_well_lets, open_well))
        {
            pref.GetComponent<StopWellController>().ReSpawn();
        }
    }

    private void RoadCarWorkerSpawn()//спавн машины дорожных раблт
    {
        CalculateStaticLetPos(road_car_worker_y_add);
        CalculateLetRot(road_car_worker_rot_add);

        if(SpawnLetFormList(road_car_worker_lets, road_car_worker))
        {
            pref.GetComponent<RWCarController>().ReSpawn();
        }
    }

    private void BrokenCarSpawn()//спавн сломаной машины
    {
        int rand_side = CalculateRoadSidePos(broken_car_y_add, Mathf.Round(let_rot.eulerAngles.y));
        CalculateSideLetRot(0.5f,rand_side);

        if (SpawnLetFormList(broken_car_lets, broken_car))
        {
            pref.GetComponent<DeadCarController>().ReSpawn();
        }
    }

    private void CrushTruckSpawn()//спавн упавшего грузовика
    {
        int rand_side = CalculateRoadSidePos(crush_truck_y_add, Mathf.Round(let_rot.eulerAngles.y));
        CalculateSideLetRot(0.5f, rand_side);

        if(SpawnLetFormList(crush_truck_lets, crush_truck))
        {
            pref.GetComponent<CrushTruckController>().ReSpawn();
        }
    }

    private void BotCarSpawn(List<GameObject> bots_type,GameObject bot_type)//спавн бота машины 
    {
        int rand_side = CalculateRoadSidePos(bot_car_y_add, Mathf.Round(let_rot.eulerAngles.y));
        CalculateSideLetRot(0f, rand_side);
        //BotsController bots_controller = pref.GetComponent<BotsController>();
        if (SpawnLetFormList(bots_type, bot_type))
        {
            pref.GetComponent<BotsController>().IsRestart();
            pref.GetComponent<BotsController>().ChooseCorpusColor();
        }
    }

    private void Turn90LetSpawn()//спавним тупик устанавливая нужное значение в скрипте дороги
    {
        road_controller.SetIsTurn90Let();
    }

    private void FallLampRoadSpawn()//спавним дорогу с упавшим фонарем
    {
        road_controller.SetIsFallLampLet();
    }

    private int CalculateRoadSidePos(float y_add, float angle)//вычисляем позицию для объекта у края дороги
    {
        cur_y = start_let_border.y + y_add;
        float lambda = Random.Range(0.1f,10f);//отношение сторон
        angle *= (Mathf.PI / 180f);

        int rand_side;
        rand_side = Random.Range(0, 2);
        Vector3 second_point;
        if (rand_side == 0)//по левой стороне
        {
            second_point = new Vector3(end_let_border.x - Mathf.Sin(angle) * let_border_vert_length, end_let_border.y,
                end_let_border.z - Mathf.Cos(angle) * let_border_vert_length);//находим противоположную по вертикали точку по формуле
            cur_x = (end_let_border.x + (second_point.x * lambda)) / (1 + lambda);
            cur_z = (end_let_border.z + (second_point.z * lambda)) / (1 + lambda);
        }
        else//по правой
        {
            second_point = new Vector3(start_let_border.x + Mathf.Sin(angle) * let_border_vert_length, start_let_border.y,
                start_let_border.z + Mathf.Cos(angle) * let_border_vert_length);//находим противоположную по вертикали точку по формуле
            cur_x = (start_let_border.x + (second_point.x * lambda)) / (1 + lambda);
            cur_z = (start_let_border.z + (second_point.z * lambda)) / (1 + lambda);
        }
        //формула нахождение координат точки делящей отрезок по лямбда отношению

        CalculateCurrentPos();
        return rand_side;
    }

    private void CalculateStaticLetPos(float y_add)//вычисление позиции для статических препятствий
    {
        cur_y = start_let_border.y + y_add;
        float last_x, last_z;
        last_x = Random.Range(start_let_border.x, end_let_border.x);
        last_z = Random.Range(start_let_border.z, end_let_border.z);


        if (Mathf.Round(let_rot.eulerAngles.y) == 45 || Mathf.Round(let_rot.eulerAngles.y) == 315 || 
            Mathf.Round(let_rot.eulerAngles.y) == 135 || Mathf.Round(let_rot.eulerAngles.y) == 225) {
            cur_x = TurnMatrixCalculateX(last_x, last_z, let_rot.eulerAngles.y, current_straight_pos);
            cur_z = TurnMatrixCalculateZ(last_x, last_z, let_rot.eulerAngles.y, current_straight_pos);
        }
        else
        {
            cur_x = last_x;
            cur_z = last_z;
        }

        CalculateCurrentPos();
    }

    private void CalculateCurrentPos()//считает вектор позиции
    {
        current_let_pos = new Vector3(cur_x, cur_y, cur_z);
    }

    private void CalculateLetRot(float y_add_rot)//считаем поворот объекта относительно прямой
    {
        y_add_rot += Random.Range(-max_rot_offset,max_rot_offset);
        let_rot = Quaternion.AngleAxis(let_rot.eulerAngles.y + y_add_rot, new Vector3(0f, 1f, 0f));
    }

    private void CalculateSideLetRot(float y_rot_koef,int rand_side)//считаем поворот объекта относительно прямой
    {
        float y_add_rot = 0;
        if (rand_side == 0) {
            y_add_rot += 180f;
        }
        y_add_rot += Random.Range(-max_rot_offset * y_rot_koef, max_rot_offset * y_rot_koef);
        let_rot = Quaternion.AngleAxis(let_rot.eulerAngles.y + y_add_rot, new Vector3(0f, 1f, 0f));
    }

    private float TurnMatrixCalculateX(float x, float z, float angle,Vector3 origin_point)//считаем Х координату через матрицу поворота
    {
        x -= origin_point.x;
        z -= origin_point.z;
        if (Mathf.Round(angle) == 45 || Mathf.Round(angle) == 225)
        {
            angle = 360 - angle;
        }
        angle *= (Mathf.PI / 180f);
        return (x * Mathf.Cos(angle) - z * Mathf.Sin(angle)) + origin_point.x;
    }

    private float TurnMatrixCalculateZ(float x, float z, float angle, Vector3 origin_point)//считаем Z координату через матрицу поворота
    {
        x -= origin_point.x;
        z -= origin_point.z;
        if (Mathf.Round(angle) == 45 || Mathf.Round(angle) == 225)
        {
            angle = 360 - angle;
        }
        angle *= (Mathf.PI / 180f);
        return (x * Mathf.Sin(angle) + z * Mathf.Cos(angle)) + origin_point.z;
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

    private bool SpawnLetFormList(List<GameObject> lst_part,GameObject part)
    {
        int ind = DefineDeletePart(lst_part);
        if (ind > -1)
        {
            lst_part[ind].transform.position = current_let_pos;
            lst_part[ind].transform.rotation = let_rot;
            lst_part[ind].SetActive(true);
            pref = lst_part[ind];
            return true;
        }
        else
        {
            pref = Instantiate(part, current_let_pos, let_rot) as GameObject;
            lst_part.Add(pref);
            return false;
        }
    }

    public void MoreHarder()//сделать сложнее
    {
        max_empty_road = 4;
        max_without_dyn_let = 1;
        max_random_dynamic = 5;
    }

    public void PlusEmptyRoad()
    {
        empty_road_kol++;
    }


}
