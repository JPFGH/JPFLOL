using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using UnityEngine.UI;

[Serializable]
public class UIManager : MonoBehaviour {

    public static UIManager Instance;


    public GameObject StateShow_Prefab;//抬头状态显示预制体


    
    public Transform AllUnitStateShow;//所有单位抬头状态显示的父节点（画布）


    public Transform deathPanelTran;

    void Awake()
    {
        Instance = this;
    }
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}


    public void ShowDeathPanel()
    {
        deathPanelTran.gameObject.SetActive(true);
        deathPanelTran.Find("Text").GetComponent<Text>().text = string.Format("你存活了{0}秒",(int)GameMode.AliveTime);
    }






    public void SetUnitStateShow(GameObject unitObj)//设置单位的抬头状态
    {
        GameObject showObj= Instantiate(StateShow_Prefab, AllUnitStateShow);
        //一开始就生成在Canvas下面，就没有适配问题
        showObj.GetComponent<StateShow>().nowUnit = unitObj.GetComponent<Unit>();

    }






}


//编辑器扩展，把public静态的字段显示到面板上
//[CustomEditor(typeof(UIManager))]
//class aaa : Editor
//{
//    public override void DrawPreview(Rect previewArea)
//    {
//        base.DrawPreview(previewArea);
//        UIManager.StateShow_Prefab = EditorGUILayout.ObjectField(default(Object),typeof(GameObject));
//    }
//}
