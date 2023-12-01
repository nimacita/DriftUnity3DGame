using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HouseController : MonoBehaviour
{
    private Transform tr;

    public GameObject types;
    //public GameObject test_body;
    public GameObject house;
    public GameObject foundation;
    public GameObject other_struc;
    public GameObject[] houses;
    public GameObject[] others;
    public Renderer[] house_boyd_materials;
    public Renderer[] glass1_materials;
    public Renderer[] glass2_materials;
    public Renderer[] window_frame_materials;
    public Renderer[] roofs_materials;
    private int house_number;
    private int more_random_chance;
    private int other_chance;

    public Material day_glasses;
    public Material night_glasses;
    public Material[] sections_color;
    public Material[] window_frame_colors;
    public Material[] roof_colors;

    private float new_x,new_z;
    private bool is_player;
    private Vector3 start_pos;
    private bool is_change_start_pos;
    
    void Start()
    {
        DisactiveAllObjects();
        AllStartValues();
        //DefineType();
        start_pos = transform.position;
    }

    
    private void AllStartValues()//начальные значения
    {
        tr = transform;

        is_change_start_pos = false;
        new_x = new_z = 0f;
        is_player = false;
        more_random_chance = 30;
        other_chance = 60;//100
    }

    private void DefineType()//определяем тип структуры
    {
        types.SetActive(true);
        foundation.SetActive(true);
        RandStructureRot();
        int rand_struc = Random.Range(0, other_chance);
        if (rand_struc == 0) {
            DefineOtherStructure();
        }
        else
        {
            HouseStructure();
        }
    }

    private void RandStructureRot()//задаем рандомный поворот структуре
    {
        int rand_rot_koef = Random.Range(1, 5);
        transform.Rotate(new Vector3(0f, 1f, 0f) * Mathf.Round(rand_rot_koef * 90));
    }

    private void HouseStructure()//структура типа дома
    {
        house.SetActive(true);

        house_number = DefineHouse();
        DefineHouseColor();
        DefineGlassColor();
        DefineWindowFrameColor();
        IsNeedChangeRoof();
    }

    private int DefineHouse()//определяем какой дом включаем
    {
        int rand_house = Random.Range(0,houses.Length);
        if (rand_house != 8 && rand_house != 9 && rand_house != 10) {
            houses[rand_house].SetActive(true);
        }
        else // увеличеный рандом шанс для панельного стекляного дома
        {
            int more_random = Random.Range(0, more_random_chance);
            if (more_random == 0)
            {
                houses[rand_house].SetActive(true);
            }
            else
            {
                return DefineHouse();
            }
        }
        return rand_house;
    }

    private void DefineHouseColor()//задаем цвет корпусу секции
    {
        if (house_number != 8 && house_number != 9 && house_number != 10) {
            int rand_color;
            rand_color = Random.Range(0, sections_color.Length);
            house_boyd_materials[house_number].material = sections_color[rand_color];
        }
    }

    private void DefineGlassColor()//задаем цвет стеклу
    {
        if (PlayerPrefs.GetInt("is_day") > 0)//день
        {
            glass1_materials[house_number].sharedMaterial = day_glasses;
            glass2_materials[house_number].sharedMaterial = day_glasses;
        }
        else//ночь
        {

            if (NightGlassMaterialChange(glass1_materials[house_number]) == 0)
            {
                glass2_materials[house_number].material = day_glasses;
            }
            else
            {
                NightGlassMaterialChange(glass2_materials[house_number]);
            }
        }
    }

    private int NightGlassMaterialChange(Renderer night_gl)//меняем цвет ночных стекл на рандомный
    {
        int rand_glass = Random.Range(0, 2);
        if (rand_glass == 0)
        {
            night_gl.material = night_glasses;
        }
        else
        {
            night_gl.material = day_glasses;
        }
        return rand_glass;
    }

    private void DefineWindowFrameColor()//меняем цвет оконных рам
    {
        if (house_number != 8 && house_number != 9 && house_number != 10) {
            int rand_color = Random.Range(0, window_frame_colors.Length);
            window_frame_materials[house_number].material = window_frame_colors[rand_color];
        }
    }

    private void IsNeedChangeRoof()//нужно ли менять цвет крыше
    {
        if (house_number == 2)//меняем крышу только у третьего пока
        {
            DefineRoofColor(roofs_materials[0]);
        }
    }

    private void DefineRoofColor(Renderer roof)//меняем цвет определнных крыш
    {
        int rand_color = Random.Range(0, roof_colors.Length);
        roof.material = roof_colors[rand_color];
    }

    private void DefineOtherStructure()//рандомную структуру типа другой
    {
        other_struc.SetActive(true);
        int rand_other = Random.Range(0, others.Length);
        others[rand_other].SetActive(true);
    }

    public void RespawnStructure()//пересобираем структуру после переспавна дороги
    {
        DisactiveAllObjects();
        //DefineType();
        ChangePos();
    }

    public void DefineStartPos()
    {
        start_pos = transform.position;
        is_change_start_pos = true;
    }

    public void ChangePos()
    {
        if (is_change_start_pos)
        {
            transform.position = start_pos;
            is_change_start_pos = false;
        }
    }

    private void DisactiveAllObjects()//выключаем все объекты в начале
    {
        for (int i = 0; i < houses.Length; i++) 
        {
            houses[i].SetActive(false);
        }
        for (int i = 0; i < others.Length; i++)
        {
            others[i].SetActive(false);
        }
        house.SetActive(false);
        other_struc.SetActive(false);
        foundation.SetActive(false);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "house")//если дом с домом, то усредняем положение
        {
            new_x = (other.transform.position.x + transform.position.x) / 2f;
            new_z = (other.transform.position.z + transform.position.z) / 2f;
            if (other.gameObject.activeSelf)
            {
                other.GetComponent<HouseController>().DefineStartPos();
                other.transform.position = new Vector3(new_x, transform.position.y, new_z);
                gameObject.SetActive(false);
            }
        }
        if(other.gameObject.tag == "road" || other.gameObject.tag == "road_for_player" 
            || other.gameObject.tag == "road_for_bots" || other.gameObject.tag == "let" )//если с другим объектом то выключаем
        {
            gameObject.SetActive(false);
        }

        if (other.gameObject.tag == "Player")
        {
            is_player = true;
        }

        if ((other.gameObject.CompareTag("MainCamera") || other.gameObject.CompareTag("photo_cam")) && !is_player)
        {
            types.SetActive(false);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("MainCamera") || other.gameObject.CompareTag("photo_cam"))
        {
            types.SetActive(true);
        }
    }
}
