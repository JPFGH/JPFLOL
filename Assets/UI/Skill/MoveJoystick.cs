using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;




//挂载到移动摇杆的外圆上
public class MoveJoystick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    public static Vector2 moveInput;//玩家移动摇杆输入单位偏移量
    public Transform CaculatePosTran;//适配辅助点,计算半径用



    float outerCircleRadius;//移动圈半径

    Transform innerCircleTrans;//得到子物体，技能内圆Transform组件
    Vector2 outerCircleWorldPos = Vector2.zero;

    void Awake()
    {
        innerCircleTrans = transform.GetChild(0);//得到排第一个的子物体
    }

    void Start()
    {
        outerCircleWorldPos = transform.position;//起始内圆位置归零
        outerCircleRadius = Vector3.Distance(transform.position ,CaculatePosTran.position);//得到宽度的一半
        //这个方法不同的机型得到的值是不一样的
    }





    /// <summary>
    /// 按下
    /// </summary>
    public void OnPointerDown(PointerEventData eventData)
    {
        innerCircleTrans.position = eventData.position;//内圆位置变为鼠标点击位置

        Vector2 touchOffset = eventData.position - outerCircleWorldPos;//触摸点相对于外圆中心位置的偏移向量
        moveInput = touchOffset.normalized;

    }

    /// <summary>
    /// 抬起
    /// </summary>
    public void OnPointerUp(PointerEventData eventData)
    {
        innerCircleTrans.localPosition = Vector3.zero;//内圆位置归零
        moveInput = Vector2.zero;//玩家输入归零
    }

    /// <summary>
    /// 滑动
    /// </summary>
    public void OnDrag(PointerEventData eventData)
    {
        //eventData.position是屏幕世界坐标系
        Vector2 touchOffset = eventData.position - outerCircleWorldPos;//触摸点相对于外圆中心位置的偏移向量
        moveInput = touchOffset.normalized;
        if (Vector3.Magnitude(touchOffset) < outerCircleRadius)//内圆位置没有越外圈
            innerCircleTrans.position = eventData.position;//
        else
            innerCircleTrans.position = outerCircleWorldPos + touchOffset.normalized * outerCircleRadius;//越了外圈
        //严格按照 起点加偏移向量得到终点 这个做法


        //Debug.DrawLine(eventData.position, outerCircleWorldPos, Color.red, 1.0f);
    }




}
