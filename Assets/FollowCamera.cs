using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class FollowCamera : MonoBehaviour
{

    public Vector3 offset;//玩家到相机的路径向量
    public Vector3 lookPosOffset;//看到的点偏移向量
    public Transform targetTran;
    public float remainShakeTime;//剩余抖屏时间

    // Use this for initialization
    void Start()
    {
        offset = new Vector3(0, 17, -13);
        lookPosOffset = new Vector3(0, 1, 0);
        targetTran = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();//一开始得到玩家

        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (targetTran != null&&remainShakeTime==0)//有玩家且不在抖屏时间
        {
            transform.position = targetTran.position + offset;//控制相机位置
            transform.rotation = Quaternion.LookRotation(targetTran.position + lookPosOffset - transform.position);
        }
        if(remainShakeTime>0)
        {
            remainShakeTime -= Time.deltaTime;
            if(remainShakeTime<=0)
            {
                remainShakeTime = 0;
                //矫正相机
                transform.position = targetTran.position + offset;//控制相机位置
                transform.rotation = Quaternion.LookRotation(targetTran.position + lookPosOffset - transform.position);
            }
        }
    }

    public void ToShake(float time)
    {
        if(PlayerSettingData.Instance.shake_Toggle==true)
        {
            transform.DOShakePosition(time, new Vector3(1, 1, 1), 50, 50);
            remainShakeTime = time;
        }
        
    }







}
