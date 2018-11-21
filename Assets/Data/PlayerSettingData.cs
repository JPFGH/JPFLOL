using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSettingData  {
    //这个脚本不继承MonoBehaviour，保存玩家的所有数据设置，
    //跨场景时发挥作用

    #region 单例模式
    //预处理器指令
    private static PlayerSettingData instance;
    public static PlayerSettingData Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new PlayerSettingData();
            }
            return instance;
        }
    }
    #endregion


    public string PlayerName = "Boy Next Door";

    public string readyToLoad = "";


    public UnitType PlayerChooseSkin;//玩家选择的皮肤类型
    public HeroType PlayerChooseHero = HeroType.NULL;//玩家选择的英雄类型
    public Dictionary<HeroType, HeroUI> heroUIDic;//所有英雄UI数据




    public PriorityMode PlayerPriorityMode = PriorityMode.HPLeast;//攻击模式

    public bool shake_Toggle = true;//震动开关

    public bool music_Toggle = true;//游戏音乐开关
    public float music_value = 1;//游戏音量












}
