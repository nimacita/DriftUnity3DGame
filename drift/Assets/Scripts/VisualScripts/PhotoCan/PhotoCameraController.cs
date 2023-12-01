using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhotoCameraController : MonoBehaviour
{

    public GameObject player;
    public MainUiController ui_controll;

    private Camera camera_comp;
    private Transform tr;
    private Transform player_tr;
    private Vector3 start_offset;
    //private Vector3 camera_move;
    private Vector3 touch;
    private float camera_speed;

    //camera_obst
    //private bool is_left, is_right, is_up, is_down, is_front, is_back;
    //public PhotoCamSens l_sens, r_sens, up_sens, down_sens, front_sens, back_sens;
    private float max_x_rot, min_x_rot;//угол высоты
    private float min_z_dist, max_z_dist;//значение до приблежения
    private float start_near,min_near;
    private float current_near;

    void Start()
    {
        AllStartValues();
    }

    private void AllStartValues()
    {
        tr = transform;
        player_tr = player.GetComponent<Transform>();
        camera_comp = gameObject.GetComponent<Camera>();
        start_offset = tr.position - player_tr.position;

        camera_speed = 35f;//25
        max_x_rot = 85f;//85 угол просмотра
        min_x_rot = 2.2f;//2.2
        min_z_dist = 8f;//8 приблежение
        max_z_dist = Vector3.Distance(tr.position, player_tr.position);

        touch = Vector3.zero;

        //near
        start_near = camera_comp.nearClipPlane;
        min_near = 0.01f;

        //is_left = is_right = is_down = is_up = false;
    }

    void FixedUpdate()
    {
        //DefineSensors();
        CameraMove();
        //NearCalc(start_near, min_near, max_z_dist, min_z_dist, Vector3.Distance(tr.position, player_tr.position));
    }

    private void NearCalc(float x1, float x2, float y1, float y2,float y)//считаем значение отрисовки в приблежении
    {
        current_near = x1 + ((x2 - x1) / (y2 - y1)) * (y - y1);
        if (current_near < min_near)
        {
            current_near = min_near;
        }
        if (current_near > start_near)
        {
            current_near = start_near;
        }
        camera_comp.nearClipPlane = current_near;
    }

    private void CameraMove()//двигаем камеру пальцами
    {
        tr.Translate(CalcCameraMove() * Time.deltaTime * camera_speed);
        tr.LookAt(player_tr.position);
    }

    private Vector3 CalcCameraMove()//считаем вектор движения камеры
    {
        Vector3 camera_move = Vector3.zero;
        if (Input.GetMouseButtonDown(0))
        {
            //touch = Camera.main.ScreenToViewportPoint(Input.mousePosition);
            touch = camera_comp.ScreenToViewportPoint(Input.mousePosition);
        }

        if (Input.touchCount == 2)
        {
            Touch touch_zero = Input.GetTouch(0);
            Touch touch_one = Input.GetTouch(1);

            Vector3 touch_zero_last_pos = touch_zero.position - touch_zero.deltaPosition;
            Vector3 touch_one_last_pos = touch_one.position - touch_one.deltaPosition;

            float dis_touch = (touch_zero_last_pos - touch_one_last_pos).magnitude;
            float cur_dis_touch = (touch_zero.position - touch_one.position).magnitude;
            float difference = cur_dis_touch - dis_touch;
            camera_move = Vector3.zero;
            camera_move.z = difference * 0.02f;
        }
        else if (Input.GetMouseButton(0))
        {
            //camera_move = touch - Camera.main.ScreenToViewportPoint(Input.mousePosition);
            camera_move = touch - camera_comp.ScreenToViewportPoint(Input.mousePosition);
        }

        if (Input.GetMouseButtonUp(0))
        {
            camera_move = Vector3.zero;
            if (touch == /*Camera.main.ScreenToViewportPoint(Input.mousePosition)*/camera_comp.ScreenToViewportPoint(Input.mousePosition))
            {
                ui_controll.PhotoModeOff();
                tr.position = player_tr.position + start_offset;
            }
        }

        return DeadZones(camera_move);
    }

    private Vector3 DeadZones(Vector3 camera_move)//мертвые зоны камеры где нельзя двигать
    {
        //if (is_left && camera_move.x < 0f)//влево
        //{
        //    camera_move.x = 0;
        //}
        //if (is_right && camera_move.x > 0f)//вправо
        //{
        //    camera_move.x = 0;
        //}
        if ((/*is_up || */tr.rotation.eulerAngles.x >= max_x_rot) && camera_move.y > 0f)//поднимаем
        {
            camera_move.y = 0;
        }
        if ((/*is_down || */tr.rotation.eulerAngles.x <= min_x_rot) && camera_move.y < 0f)//опускаем
        {
            camera_move.y = 0;
        }
        if ((Vector3.Distance(tr.position, player_tr.position) <= min_z_dist /*|| is_front*/) && camera_move.z > 0f) //приближаем
        {
            camera_move.z = 0;
        }
        if ((Vector3.Distance(tr.position, player_tr.position) >= max_z_dist /*|| is_back*/) && camera_move.z < 0f) //отдаляем
        {
            camera_move.z = 0;
        }
        return camera_move;
    }

    //private void DefineSensors()//получаем значения сенсоров
    //{
    //    is_left = l_sens.GetSens();
    //    is_right = r_sens.GetSens();
    //    is_up = up_sens.GetSens();
    //    is_down = down_sens.GetSens();
    //    is_front = front_sens.GetSens();
    //    is_back = back_sens.GetSens();
    //}
}
