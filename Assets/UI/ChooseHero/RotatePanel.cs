using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class RotatePanel : MonoBehaviour, IDragHandler
{
    Transform stageTran;


    //1920:1080下速度，分辨率降低速度加快
    //反比
    public float rotateSmoothing = 0.7f;
    float height;//真实屏高
    float actualSmoothing;
    // Use this for initialization
    void Start () {
        stageTran = GameObject.Find("Stage").transform;
        height = Screen.height;////获取游戏窗口的宽高
        actualSmoothing =(float)(rotateSmoothing * 1080) / height;
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void OnDrag(PointerEventData eventData)
    {
        
        stageTran.RotateAround(stageTran.position, Vector3.up, -eventData.delta.x * actualSmoothing);

    }

}
