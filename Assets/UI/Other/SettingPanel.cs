using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public enum PriorityMode//攻击模式
{
    Distance,//距离最短优先
    HPLeast,//血量最少优先
}


public class SettingPanel : MonoBehaviour {
    //应该设置面板不销毁的，但是两个场景里做了两个，每增加要拖动的，两个场景里都要拖



    public Toggle Toggle_HP;
    public Toggle Toggle_Dis;

    public Toggle ZDXGToggle;//震动效果开关
    public GameObject ZDXGOff;//控制显示与隐藏的下面的图片父节点

    public Toggle YXYYToggle;//游戏音乐开关
    public GameObject YXYYOff;//控制显示与隐藏的下面的图片父节点
    public Slider music_Slider;//音乐音量滑条

    public GameObject backSceneButton;

    void Start () {
        if(SceneManager.GetActiveScene().name== "ChooseHero")
        {
            backSceneButton.SetActive(false);
        }

    }
	

	void Update () {
		
	}

    public void InitSettingData()//这个要放在GameMode里执行，因为一开始面板是禁用的
    {
        //读取设置数据，改变设置面板
        if(PlayerSettingData.Instance.PlayerPriorityMode == PriorityMode.Distance)
        {
            Toggle_Dis.isOn = true;
        }
        else
        {
            Toggle_HP.isOn = true;
        }
        ChangeToDistance();
        ChangeToHPLeast();


        if (PlayerSettingData.Instance.music_Toggle==true)
        {
            YXYYToggle.isOn = true;
        }
        else
        {
            YXYYToggle.isOn = false;
        }
        YXYYOnAndOff();


        if(PlayerSettingData.Instance.shake_Toggle==true)
        {
            ZDXGToggle.isOn = true;
        }
        else
        {
            ZDXGToggle.isOn = false;
        }
        ZDXGOnAndOff();


        music_Slider.value = PlayerSettingData.Instance.music_value;
    }






    public void ChangePriorityMode(GameObject toggle)
    {
        
    }

    /// <summary>
    /// 音乐音量滑条事件
    /// </summary>
    public void ChangeMusicVolume()
    {
        AudioSource AS = Camera.main.GetComponent<AudioSource>();
        AS.volume = music_Slider.value;
        PlayerSettingData.Instance.music_value = music_Slider.value;//同步设置数据
    }


    /// <summary>
    /// 返回选英雄场景按钮
    /// </summary>
    public void BackSceneButton()
    {
        //清空所有存储空间数据
        BackPack.Instance.ClearStorageArray();
        Forge.Instance.ClearStorageArray();
        Equips.Instance.ClearStorageArray();

        BackPack.Instance.StorageItemArray = new Item[8];


        PlayerSettingData.Instance.readyToLoad = "ChooseHero";
        SceneManager.LoadScene("Middle");
    }

    public void ExitGameButton()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
Application.Quit();
#endif

    }

    /// <summary>
    /// 改变到距离优先
    /// </summary>
    public void ChangeToDistance()
    {
        if(Toggle_Dis.isOn)
        {
            PlayerSettingData.Instance.PlayerPriorityMode = PriorityMode.Distance;
        }
        
    }

    /// <summary>
    /// 改变到血量优先
    /// </summary>
    public void ChangeToHPLeast()
    {
        if (Toggle_HP.isOn)
        {
            PlayerSettingData.Instance.PlayerPriorityMode = PriorityMode.HPLeast;
        }
        
    }




    /// <summary>
    /// 震动效果开关
    /// </summary>
    public void ZDXGOnAndOff()
    {
        if(ZDXGToggle.isOn)//切换到开启状态
        {
            ZDXGOff.SetActive(false);
        }
        else
        {
            ZDXGOff.SetActive(true);
        }
        PlayerSettingData.Instance.shake_Toggle = ZDXGToggle.isOn;//同步设置数据
    }





    /// <summary>
    /// 游戏音乐开关
    /// </summary>
    public void YXYYOnAndOff()
    {
        if(YXYYToggle.isOn)//切换到开启状态
        {
            YXYYOff.SetActive(false);
            music_Slider.transform.Find("Fill Area/Fill").GetComponent<Image>().color = new Color32(0, 167, 255, 255);//蓝色
            music_Slider.enabled = true;
            Camera.main.GetComponent<AudioSource>().mute = false;
        }
        else
        {
            YXYYOff.SetActive(true);
            music_Slider.transform.Find("Fill Area/Fill").GetComponent<Image>().color = new Color32(97, 97, 97, 255);//灰色
            music_Slider.enabled = false;
            Camera.main.GetComponent<AudioSource>().mute = true;
        }
        PlayerSettingData.Instance.music_Toggle = YXYYToggle.isOn;//同步设置数据
    }






}
