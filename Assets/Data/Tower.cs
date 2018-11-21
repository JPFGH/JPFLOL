using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Tower : MonoBehaviour {

    public bool isBuild = false;//点下建造按钮瞬间被建造，然后缓缓上升
    public bool isRun = false;//防御塔在理想位置正式运作


    float attckNum = 800;//攻击力 
    float range = 10f;//对半径内敌人造成伤害
    float CD = 2f;
    float Timer = 0;//计时器

    float upTime = 10f;//动画上升时间

    Transform muzzleTran;//炮口位置
    Transform circlrTran;//旗下圆圈



    Tweener upMove;//防御塔上升动画


    LineRenderer attackLine;

    // Use this for initialization
    void Start () {
        muzzleTran = transform.Find("Muzzle");
        muzzleTran.gameObject.tag = "Tower";//以炮口为基准计算范围

        circlrTran = transform.Find("Muzzle/TowerOuterCircle");
        circlrTran.gameObject.SetActive(false);
        circlrTran.localScale = new Vector3(2 * range, 2 * range, 1);

        attackLine=GetComponent<LineRenderer>();
        attackLine.SetPosition(0, muzzleTran.position+new Vector3(0,5,0));//设置世界坐标的第一个点位置
        attackLine.enabled = false;

        upMove = transform.DOLocalMove(new Vector3(0, 0, 0), upTime);//填终点坐标
        upMove.SetAutoKill(true);//动画播放完自动销毁
        upMove.SetEase(Ease.InOutCubic);//设置动画的缓动曲线
        upMove.Pause();//不能一开始就播放
        upMove.OnComplete(ToUse);//设置播放完触发


    }
	
	// Update is called once per frame
	void Update () {
		if(isRun)
        {
            Timer -= Time.deltaTime;
            //检测范围内敌人，并攻击，显示范围圈，攻击线，开炮
            GameObject targetObj = ScopeManager.CheckTargetInCircle(muzzleTran.gameObject, range);

            if (targetObj != null)
            {
                circlrTran.gameObject.SetActive(true);//显示圈
                attackLine.enabled = true;
                attackLine.SetPosition(1, targetObj.transform.position + new Vector3(0, 1, 0));

                if(Timer<=0)//CD结束可以开炮
                {
                    Timer = CD;

                    GameObject obj = ObjectsPool.Instance.DequeueInstance(ObjectsPoolType.TowerMissile_Prefab);
                    Fly flyInfo = obj.GetComponent<Fly>();
                    Vector3 startPos = muzzleTran.position;//发射口位置
                    flyInfo.InitializeFly(15, attckNum, null, startPos, Quaternion.identity, null, false, false);
                    flyInfo.InitializeFlyTarget(targetObj, range * 2);//锁敌，跟踪距离稍长，大约两倍
                }
            }
            else
            {
                Debug.Log("隐藏了圈");
                circlrTran.gameObject.SetActive(false);//隐藏圈
                attackLine.enabled = false;
            }
        }


	}


    public void ToBuild()//开始建造（防御塔开始上升）
    {
        isBuild = true;
        upMove.PlayForward();//开始播放上升动画

    }

    //正式启用
    void ToUse()
    {
        isRun = true;
    }




    





}
