using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TouchpadController : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler 
{

    [SerializeField]
    private Vector2 first_positions, current_position;
    private float difference_x;
    [SerializeField]
    private int direction;
    private float rotation_speed;
    private bool is_touch;
    private float min_rot_speed, max_rot_speed,koef_rot_speed;
    private int swipe_direction;

    void Start()
    {
        StartValues();
    }

    private void StartValues()
    {
        direction = 1;
        difference_x = 0;
        rotation_speed = 0f;
        is_touch = false;

        swipe_direction = 1;
    }

    public void SetCarsValues(float set_min_speed,float set_max_speed,float set_koef_rot_speed)
    {
        min_rot_speed = set_min_speed;
        max_rot_speed = set_max_speed;
        koef_rot_speed = set_koef_rot_speed;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        first_positions = eventData.position;
        is_touch = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
        //FindSwipeDir(eventData.position);
        current_position = eventData.position;
        FindDirection();
        SpeedCalculate();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        rotation_speed = 0f;
        is_touch = false;
    }

    private void FindDirection()
    {
        if (current_position.x > first_positions.x)
        {
            direction = 1;
        }

        if (current_position.x < first_positions.x)
        {
            direction = -1;
        }
    }

    private void SpeedCalculate()
    {
        difference_x = Mathf.Abs(current_position.x) - Mathf.Abs(first_positions.x);
        rotation_speed = SpeedForDifference(difference_x);
    }

    private float SpeedForDifference(float dif)
    {
        float result;
        result = Mathf.Abs(dif) / koef_rot_speed;
        if (result < min_rot_speed)
        {
            result = min_rot_speed;
        }
        if (result > max_rot_speed)
        {
            result = max_rot_speed;
        }
        return result;
    }

    private void FindSwipeDir(Vector2 position)
    {
        if (swipe_direction != 1)
        {
            if (position.x > current_position.x)
            {
                first_positions = position;
                swipe_direction = 1;
            }
        }
        if (swipe_direction != -1)
        {
            if (position.x < current_position.x)
            {
                first_positions = position;
                swipe_direction = -1;
            }
        }
    }

    public bool GetIsTouch()
    {
        return is_touch;
    }

    public int GetDirection()
    {
        return direction;
    }

    public float GetRotateSpeed()
    {
        return rotation_speed;
    }
}
