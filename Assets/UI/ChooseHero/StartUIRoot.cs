using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StartUIRoot : MonoBehaviour
{

    public Text userInput;

    //1980下滑动速度
    float smoothing = 20;
    float actualSmoothing;


    //1980下最大偏移
    const float YOffset = 135;
    const float XOffset = 240;

    float actualYOffset;
    float actualXOffset;



    public Vector3 lastFrameA;//上一帧的加速度计
    public Image BG;



    // Use this for initialization
    void Start()
    {
        lastFrameA = Input.acceleration;


        actualYOffset = YOffset * ((float)Screen.height / 1080);
        actualXOffset = XOffset * ((float)Screen.height / 1080);

        actualSmoothing = smoothing * ((float)Screen.height / 1080);

    }

    // Update is called once per frame
    void Update()
    {
        

        //Debug.Log(Input.acceleration);

        //正对用户情况下
        //if (Input.acceleration.y - lastFrameA.y < -0.01f)//手机开始前倾
        //{
        //    //图片下移
        //    if (Mathf.Abs(BG.rectTransform.localPosition.y) <= actualYOffset - actualSmoothing)
        //    {
        //        BG.rectTransform.localPosition = new Vector2(BG.rectTransform.localPosition.x, BG.rectTransform.localPosition.y - actualSmoothing);
        //    }
        //}
        //else if (Input.acceleration.y - lastFrameA.y > 0.01f)//手机开始后倾
        //{
        //    //图片下移
        //    if (Mathf.Abs(BG.rectTransform.localPosition.y) <= actualYOffset - actualSmoothing)
        //    {
        //        BG.rectTransform.localPosition = new Vector2(BG.rectTransform.localPosition.x, BG.rectTransform.localPosition.y + actualSmoothing);
        //    }
        //}

        //if (Input.acceleration.x - lastFrameA.x < -0.01f)//手机开始左倾
        //{
        //    //图片左移
        //    if (Mathf.Abs(BG.rectTransform.localPosition.x) <= actualXOffset - actualSmoothing)
        //    {
        //        BG.rectTransform.localPosition = new Vector2(BG.rectTransform.localPosition.x - actualSmoothing, BG.rectTransform.localPosition.y);
        //    }
        //}
        //else if (Input.acceleration.y - lastFrameA.y > 0.01f)//手机开始右倾
        //{
        //    //图片右移
        //    if (Mathf.Abs(BG.rectTransform.localPosition.x) <= actualXOffset - actualSmoothing)
        //    {
        //        BG.rectTransform.localPosition = new Vector2(BG.rectTransform.localPosition.x + actualSmoothing, BG.rectTransform.localPosition.y);
        //    }
        //}







        lastFrameA = Input.acceleration;
    }

    /// <summary>
    /// 进入游戏按钮
    /// </summary>
    public void GotoGame()
    {
        Debug.Log(userInput.text);
        if(userInput.text=="")
        {
            PlayerSettingData.Instance.PlayerName = "Boy Next Door";
        }
        else
        {
            PlayerSettingData.Instance.PlayerName = userInput.text;
        }

        SceneManager.LoadScene("Middle");
        //以下代码在切换过去执行
        PlayerSettingData.Instance.readyToLoad = "ChooseHero";




    }








}
