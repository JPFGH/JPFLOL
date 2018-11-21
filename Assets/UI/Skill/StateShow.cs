using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StateShow : MonoBehaviour {

    public Unit nowUnit;//当前显示的抬头状态的单位

    public Text level;
    public Text Name;
    public Image HP;
    public Image MP;

    public GameObject vertigoObj;
    public GameObject decelerateObj;


    public GridLayoutGroup hpGG;//血条下面的组件拖过来
    //158.7 Cell Size x为1格血 1000 在这个基础上除
    const float stickLength = 163.3f;
    int oneStickHP = 1000;


    //1920:1080下的偏移量
    Vector3 offset = new Vector3(-10, 170,0);//有问题，屏幕适配
    Vector3 actualOffset;

	// Use this for initialization
	void Start () {
        
        if(nowUnit.gameObject.tag=="Enemy")
        {
            HP.color = Color.red;
        }
        

	}


    //Transform Blood;
    // Update is called once per frame
    void Update()
    {
        if (nowUnit!= null && nowUnit.alive == true)
        {

            float height = Screen.height;////获取游戏窗口的宽高
            //Debug.Log(height);
            //正比
            actualOffset = new Vector3(offset.x * (height / 1080), offset.y * (height / 1080), 0);
            transform.position = Camera.main.WorldToScreenPoint(nowUnit.gameObject.transform.position) + actualOffset;
            //Debug.Log(actualOffset);


            level.text = nowUnit.level.ToString();
            Name.text = nowUnit.Name;
            HP.fillAmount = (float)nowUnit.HP / nowUnit.HP_Max;
            //血条格数更新
            float num = (float)nowUnit.HP_Max / oneStickHP;
            hpGG.cellSize = new Vector2(stickLength / num, hpGG.cellSize.y);



            MP.fillAmount = (float)nowUnit.MP / nowUnit.MP_Max;

            if(nowUnit.isVertigo)
            {
                vertigoObj.SetActive(true);
            }
            else
            {
                vertigoObj.SetActive(false);
            }


            bool haveDec = false;
            if(nowUnit.states!=null)
            {
                for (int i = 0; i < nowUnit.states.Count; i++)
                {
                    if (nowUnit.states[i].stateType == StateType.Decelerate)
                        haveDec = true;
                }
            }
            if(haveDec)
            {
                decelerateObj.SetActive(true);
            }
            else
            {
                decelerateObj.SetActive(false);
            }




        }
        else
        {
            Destroy(gameObject);
        }





    }







}
