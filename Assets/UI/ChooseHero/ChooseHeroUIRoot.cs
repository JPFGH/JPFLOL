using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using LitJson;//把LitJson.dll放到Plugins插件文件夹，Plugins文件夹下的代码会预先编译
using System.IO;
using System.Text;
using UnityEngine.SceneManagement;


public class Skin
{
    public HeroType hType;
    public UnitType type;
    public Sprite icon;
    public Sprite poster;
    public string heroName;
    public string skinName;
    public GameObject skinModel;

    public Skin(UnitType type, Sprite icon, Sprite poster, string heroName,string skinName,GameObject skinModel)
    {
        this.type = type;
        this.icon = icon;
        this.poster = poster;
        this.heroName = heroName;
        this.skinName = skinName;
        this.skinModel = skinModel;
    }
}

public class HeroUI
{
    public string name;
    public HeroType hType;
    public string occupation;//职业
    public string GLJQ;
    public Sprite s0Icon;
    public string s0Name;
    public string s0Info;
    public string s0Content;
    public Sprite s1Icon;
    public string s1Name;
    public string s1Info;
    public string s1Content;
    public Sprite s2Icon;
    public string s2Name;
    public string s2Info;
    public string s2Content;
    public Sprite s3Icon;
    public string s3Name;
    public string s3Info;
    public string s3Content;

    public HeroUI(string name,HeroType hType, string occupation, string GLJQ,
        Sprite s0Icon, string s0Name, string s0Info, string s0Content,
        Sprite s1Icon, string s1Name, string s1Info, string s1Content,
        Sprite s2Icon, string s2Name, string s2Info, string s2Content,
        Sprite s3Icon, string s3Name, string s3Info, string s3Content)
    {
        this.name = name;
        this.hType = hType;
        this.occupation = occupation;
        this.GLJQ = GLJQ;

        this.s0Icon = s0Icon;
        this.s0Name = s0Name;
        this.s0Info = s0Info;
        this.s0Content = s0Content;

        this.s1Icon = s1Icon;
        this.s1Name = s1Name;
        this.s1Info = s1Info;
        this.s1Content = s1Content;

        this.s2Icon = s2Icon;
        this.s2Name = s2Name;
        this.s2Info = s2Info;
        this.s2Content = s2Content;

        this.s3Icon = s3Icon;
        this.s3Name = s3Name;
        this.s3Info = s3Info;
        this.s3Content = s3Content;
    }
}




public class ChooseHeroUIRoot : MonoBehaviour {

    //拖过来的技能
    public Transform S0Tran;
    public Transform S1Tran;
    public Transform S2Tran;
    public Transform S3Tran;
    public Text occupationText;
    public Text GLJQtext;
    public Transform chooseCircleTran;//拖过来的光圈
    public Text skillNameT;
    public Text skillInfoT;
    public Text skillContentT;



    public bool isCircleShow = false;


    public static ChooseHeroUIRoot Instance;

    public Dictionary<UnitType, Skin> skinsDic;//根据 皮肤类型 的UI皮肤数据
    public Dictionary<HeroType, HeroUI> heroUIDic;//根据 英雄类型的 的英雄UI数据



    public List<HeroType> heroTypeList;
    public int index;
    public HeroType chooseHeroType ;//玩家所选英雄类型


    

    public static List<Skin> HBSS_List = new List<Skin>();
    public static List<Skin> PCNJ_List = new List<Skin>();
    public static List<Skin> SJLR_List = new List<Skin>();
    public Skin chooseSkinType;//玩家所选皮肤类型


    public Image poster;//海报
    public bool isPosterShow = false;




    public void Awake()
    {
        
        Instance = this;
        DecodeSkinJson();
        DecodeHeroUIJson();
    }

    // Use this for initialization
    void Start () {
        transform.Find("SettingPanel").GetComponent<SettingPanel>().InitSettingData();
        chooseHeroType = HeroType.HBSS;//切换到选英雄场景默认寒冰射手
        AddHero();
        poster = transform.Find("Poster").GetComponent<Image>();


        chooseCircleTran.gameObject.SetActive(false);
        ChangeHeroUI();

    }
	
	// Update is called once per frame
	void Update () {
		
	}


    public void AddHero()
    {
        heroTypeList.Add(HeroType.HBSS);
        heroTypeList.Add(HeroType.PCNJ);
        heroTypeList.Add(HeroType.SJLR);
    }



    public void DecodeSkinJson()
    {
        //从战斗场景切换到英雄场景需要清空
        HBSS_List.Clear();
        PCNJ_List.Clear();
        SJLR_List.Clear();


        skinsDic = new Dictionary<UnitType, Skin>();
        JsonData jsondata = JsonMapper.ToObject(Resources.Load<TextAsset>("Config/Skin").text);
        for (int i = 0; i < jsondata.Count; i++)
        {
            HeroType hType= (HeroType)System.Enum.Parse(typeof(HeroType), jsondata[i]["HeroType"].ToString());
            UnitType type= (UnitType)System.Enum.Parse(typeof(UnitType), jsondata[i]["UnitType"].ToString());
            string iconPath = jsondata[i]["ImagePath"].ToString();
            Sprite icon = Resources.Load<Sprite>(iconPath);
            string posterPath = jsondata[i]["PosterPath"].ToString();
            Sprite poster = Resources.Load<Sprite>(posterPath);

            string heroName= jsondata[i]["heroName"].ToString();
            string skinName = jsondata[i]["skinName"].ToString();
            string skinModelPath= jsondata[i]["skinModelPath"].ToString();
            GameObject model = Resources.Load<GameObject>(skinModelPath);
            Skin skin = new Skin(type, icon,poster, heroName, skinName, model);
            switch (hType)
            {
                case HeroType.HBSS:
                    HBSS_List.Add(skin);
                    break;
                case HeroType.PCNJ:
                    PCNJ_List.Add(skin);
                    break;
                case HeroType.SJLR:
                    SJLR_List.Add(skin);
                    break;
                default:
                    break;
            }
            skinsDic.Add(type, skin);
        }
    }

    public void DecodeHeroUIJson()
    {
        heroUIDic = new Dictionary<HeroType, HeroUI>();
        JsonData jsondata = JsonMapper.ToObject(Resources.Load<TextAsset>("Config/HeroUI").text);
        //???????????????????????????????
        for (int i = 0; i < jsondata.Count; i++)
        {
            string name= jsondata[i]["HeroName"].ToString();
            HeroType hType = (HeroType)System.Enum.Parse(typeof(HeroType), jsondata[i]["HeroType"].ToString());
            string occupation= jsondata[i]["Occupation"].ToString();
            string GLJQ= jsondata[i]["GLJQ"].ToString();

            string S0IconPath = jsondata[i]["S0IconPath"].ToString();
            Sprite S0Icon = Resources.Load<Sprite>(S0IconPath);
            string S0Name= jsondata[i]["S0Name"].ToString();
            string S0Info = jsondata[i]["S0Info"].ToString();
            string S0Content = jsondata[i]["S0Content"].ToString();

            string S1IconPath = jsondata[i]["S1IconPath"].ToString();
            Sprite S1Icon = Resources.Load<Sprite>(S1IconPath);
            string S1Name = jsondata[i]["S1Name"].ToString();
            string S1Info = jsondata[i]["S1Info"].ToString();
            string S1Content = jsondata[i]["S1Content"].ToString();

            string S2IconPath = jsondata[i]["S2IconPath"].ToString();
            Sprite S2Icon = Resources.Load<Sprite>(S2IconPath);
            string S2Name = jsondata[i]["S2Name"].ToString();
            string S2Info = jsondata[i]["S2Info"].ToString();
            string S2Content = jsondata[i]["S2Content"].ToString();

            string S3IconPath = jsondata[i]["S3IconPath"].ToString();
            Sprite S3Icon = Resources.Load<Sprite>(S3IconPath);
            string S3Name = jsondata[i]["S3Name"].ToString();
            string S3Info = jsondata[i]["S3Info"].ToString();
            string S3Content = jsondata[i]["S3Content"].ToString();

            HeroUI heroUI = new HeroUI(name, hType, occupation, GLJQ,
                S0Icon, S0Name, S0Info, S0Content,
                S1Icon, S1Name, S1Info, S1Content,
                S2Icon, S2Name, S2Info, S2Content,
                S3Icon, S3Name, S3Info, S3Content);


            switch (hType)
            {
                case HeroType.HBSS:
                    heroUIDic.Add(HeroType.HBSS, heroUI);
                    break;
                case HeroType.PCNJ:
                    heroUIDic.Add(HeroType.PCNJ, heroUI);
                    break;
                case HeroType.SJLR:
                    heroUIDic.Add(HeroType.SJLR, heroUI);
                    break;
                default:
                    break;
            }
        }


        PlayerSettingData.Instance.heroUIDic = heroUIDic;
    }



    public void RightChangeButton()//右箭头切换英雄
    {
        int targetIndex = index + 1;
        if(targetIndex> heroTypeList.Count-1)
        {
            index = 0;
        }
        else
        {
            index += 1;
        }
        chooseHeroType = heroTypeList[index];
        PlayerSettingData.Instance.PlayerChooseHero = chooseHeroType;//同步设置数据
        SkinScrollPanel.Instance.ChangeSkins(heroTypeList[index]);
        SkinScrollPanel.Instance.ChangeModelAndPoster(0);

        ChangeHeroUI();
    }

    public void LeftChangeButton()//左箭头切换英雄
    {
        int targetIndex = index - 1;
        if (targetIndex < 0)
        {
            index = heroTypeList.Count-1;
        }
        else
        {
            index -= 1;
        }
        chooseHeroType = heroTypeList[index];
        PlayerSettingData.Instance.PlayerChooseHero = chooseHeroType;//同步设置数据
        SkinScrollPanel.Instance.ChangeSkins(heroTypeList[index]);
        SkinScrollPanel.Instance.ChangeModelAndPoster(0);

        ChangeHeroUI();
    }

    public void ChangeHeroUI()
    {
        occupationText.text = heroUIDic[chooseHeroType].occupation;
        GLJQtext.text = heroUIDic[chooseHeroType].GLJQ;
        S0Tran.GetComponent<Image>().sprite = heroUIDic[chooseHeroType].s0Icon;
        S1Tran.GetComponent<Image>().sprite = heroUIDic[chooseHeroType].s1Icon;
        S2Tran.GetComponent<Image>().sprite = heroUIDic[chooseHeroType].s2Icon;
        S3Tran.GetComponent<Image>().sprite = heroUIDic[chooseHeroType].s3Icon;
    }






    public void ShowPosterButton()
    {
        if (isPosterShow == false)
        {
            isPosterShow = true;
            poster.gameObject.SetActive(true);
        }
        else
        {
            isPosterShow = false;
            poster.gameObject.SetActive(false);
        }
            
    }











    public void OnSkill0ClickButton()
    {
        if(isCircleShow==false||S0Tran.childCount==0)//没有光圈 或 光圈不在该技能上
        {
            isCircleShow = true;
            transform.Find("SkillExplainPanel").gameObject.SetActive(true);
            chooseCircleTran.gameObject.SetActive(true);
            chooseCircleTran.SetParent(S0Tran);
            chooseCircleTran.localPosition = Vector2.zero;

            skillNameT.text = heroUIDic[chooseHeroType].s0Name;
            skillInfoT.text = heroUIDic[chooseHeroType].s0Info;
            skillContentT.text= heroUIDic[chooseHeroType].s0Content;
        }
        else//光圈就在这个技能上
        {
            CloseSkillExplainPanel();
        }
    }

    public void OnSkill1ClickButton()
    {
        if (isCircleShow == false || S1Tran.childCount == 0)//没有光圈 或 光圈不在该技能上
        {
            isCircleShow = true;
            transform.Find("SkillExplainPanel").gameObject.SetActive(true);
            chooseCircleTran.gameObject.SetActive(true);
            chooseCircleTran.SetParent(S1Tran);
            chooseCircleTran.localPosition = Vector2.zero;

            skillNameT.text = heroUIDic[chooseHeroType].s1Name;
            skillInfoT.text = heroUIDic[chooseHeroType].s1Info;
            skillContentT.text = heroUIDic[chooseHeroType].s1Content;
        }
        else//光圈就在这个技能上
        {
            CloseSkillExplainPanel();
        }
    }

    public void OnSkill2ClickButton()
    {
        if (isCircleShow == false || S2Tran.childCount == 0)//没有光圈 或 光圈不在该技能上
        {
            isCircleShow = true;
            transform.Find("SkillExplainPanel").gameObject.SetActive(true);
            chooseCircleTran.gameObject.SetActive(true);
            chooseCircleTran.SetParent(S2Tran);
            chooseCircleTran.localPosition = Vector2.zero;

            skillNameT.text = heroUIDic[chooseHeroType].s2Name;
            skillInfoT.text = heroUIDic[chooseHeroType].s2Info;
            skillContentT.text = heroUIDic[chooseHeroType].s2Content;
        }
        else//光圈就在这个技能上
        {
            CloseSkillExplainPanel();
        }
    }

    public void OnSkill3ClickButton()
    {
        if (isCircleShow == false || S3Tran.childCount == 0)//没有光圈 或 光圈不在该技能上
        {
            isCircleShow = true;
            transform.Find("SkillExplainPanel").gameObject.SetActive(true);
            chooseCircleTran.gameObject.SetActive(true);
            chooseCircleTran.SetParent(S3Tran);
            chooseCircleTran.localPosition = Vector2.zero;

            skillNameT.text = heroUIDic[chooseHeroType].s3Name;
            skillInfoT.text = heroUIDic[chooseHeroType].s3Info;
            skillContentT.text = heroUIDic[chooseHeroType].s3Content;
        }
        else//光圈就在这个技能上
        {
            CloseSkillExplainPanel();
        }
    }










    public void CloseSkillExplainPanel()//点击技能详细描述面板
    {
        transform.Find("SkillExplainPanel").gameObject.SetActive(false);
        //隐藏光圈
        chooseCircleTran.gameObject.SetActive(false);
        isCircleShow = false;
    }



    public void BackButton()
    {
        SceneManager.LoadScene("Start");

    }


    public void StartGame()//最终开始游戏
    {
        PlayerSettingData.Instance.readyToLoad = "JPF Moba";
        SceneManager.LoadScene("Middle");
    }













}
