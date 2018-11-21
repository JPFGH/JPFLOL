using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public enum NumShowType//伤害字体显示样式
{
    NormalHurtNum,//普通伤害
    ViolentHurtNum,//暴击伤害
    RecoverHPNum,//恢复生命
}

public enum NumShowDir//数值显示方向
{
    //回复生命
    Null,
    //伤害类型
    Left,
    Right,
}


public class NumShow : MonoBehaviour
{

    public ObjectsPoolType nowPool;//当前使用了池的类型
    //这个在做预制体时手动选择
    public Text numT;
    public Image fire;

    //1920:1080下的普通伤害数字位置信息
    Vector3 normalStartOffsetL = new Vector3(-40, 50, 0);//数字出现在左侧的理想起始位置
    Vector3 normalStartOffsetR = new Vector3(40, 50, 0);//数字出现在右侧的理想起始位置
    Vector3 actualNormalStartOffsetL;
    Vector3 actualNormalStartOffsetR;
    Vector3 normalEndOffsetL = new Vector3(-100, 0, 0);//左侧数字出现点到消失在左侧终点的偏移
    Vector3 actualNormalEndOffsetL;

    //1920:1080下的暴击伤害数字位置信息
    Vector3 vioStartL = new Vector3(-40, 50, 0);//数字左侧最小开始位置
    Vector3 vioStartR = new Vector3(40, 50, 0);//数字右侧最小开始位置
    Vector3 vioStopOffsetL = new Vector3(-60, 60, 0);//左侧数字从从最小位置到正常大小位置的偏移
    Vector3 vioStopOffsetR = new Vector3(60, 60, 0);//右侧数字从从最小位置到正常大小位置的偏移
    Vector3 vioDisOffset = new Vector3(0, -50, 0);//暴击数字消失的终点
    Vector3 actualVioStartL;
    Vector3 actualVioStartR;
    Vector3 actualVioStopOffsetL;
    Vector3 actualVioStopOffsetR;
    Vector3 actualVioDisOffset;

    //1920:1080下的恢复数字位置信息
    Vector3 recoverStartOffset = new Vector3(0, 20, 0);
    Vector3 actualRecoverStartOffset;

    //1920:1080随机偏移数值
    float random = 15;




    void Start()
    {
        //一些坐标的转化不能放在这，因为第一次播动画时不会执行到

    }


    /// <summary>
    /// 初始化数值显示动画信息
    /// </summary>
    /// <param name="type"></param>
    /// <param name="num"></param>
    /// <param name="unitPos"></param>
    /// <param name="dir"></param>
    public void InitFlutterAnimation(NumShowType type, int num, Vector3 unitPos, NumShowDir dir)
    {
        //实际普通伤害数字位置计算
        actualNormalStartOffsetL = new Vector3(normalStartOffsetL.x * ((float)Screen.height / 1080), normalStartOffsetL.y * ((float)Screen.height / 1080), 0);
        actualNormalStartOffsetR = new Vector3(normalStartOffsetR.x * ((float)Screen.height / 1080), normalStartOffsetR.y * ((float)Screen.height / 1080), 0);
        actualNormalEndOffsetL = new Vector3(normalEndOffsetL.x * ((float)Screen.height / 1080), 0, 0);
        //实际暴击伤害数字位置计算
        actualVioStartL = new Vector3(vioStartL.x * ((float)Screen.height / 1080), vioStartL.y * ((float)Screen.height / 1080), 0);
        actualVioStartR = new Vector3(vioStartR.x * ((float)Screen.height / 1080), vioStartR.y * ((float)Screen.height / 1080), 0);
        actualVioStopOffsetL = new Vector3(vioStopOffsetL.x * ((float)Screen.height / 1080), vioStopOffsetL.y * ((float)Screen.height / 1080), 0);
        actualVioStopOffsetR = new Vector3(vioStopOffsetR.x * ((float)Screen.height / 1080), vioStopOffsetR.y * ((float)Screen.height / 1080), 0);
        actualVioDisOffset = new Vector3(0, vioDisOffset.y * ((float)Screen.height / 1080), 0);

        //实际恢复数字位置计算
        actualRecoverStartOffset = new Vector3(0 , recoverStartOffset.y* ((float)Screen.height / 1080), 0);


        switch (type)
        {
            case NumShowType.NormalHurtNum:
                numT.text = num.ToString();
                numT.color = Color.red;
                //1920:1080字体初始大小
                numT.fontSize = 34;
                //控制位移缩放，和透明度的动画
                NormalFly(numT, unitPos, dir);

                break;
            case NumShowType.ViolentHurtNum:
                numT.text = num.ToString();
                numT.color = Color.red;
                //1920:1080字体初始大小
                numT.fontSize = 8;
                //控制位移缩放，和透明度的动画
                ViolentFly(numT, unitPos, dir);

                break;
            case NumShowType.RecoverHPNum:
                numT.text = "+" + num.ToString();
                numT.color = Color.green;
                //1920:1080字体初始大小
                numT.fontSize = 44;
                //控制位移缩放，和透明度的动画
                RecoverFly(numT, unitPos);

                break;
            default:
                break;
        }
    }
    //Sequence.Append构建缓动序列，同时Join方法支持并行缓动。利用这个特性，可以实现ugui—Text的飘字缓动效果。
    //Append是在序列的末端插入一个Tweener，如果前面的Tweener都执行完了，就执行这个Tweener。
    //Join也是在序列末端插入一个Tweener，不同的是，这个Tweener将与前一个非Join加进来的Tweener并行执行。


    void NormalFly(Graphic graphic, Vector3 unitPos, NumShowDir dir)
    {
        Vector3 randomOffsetV3 = RandomOffset();


        RectTransform rt = graphic.rectTransform;
        Color c = graphic.color;
        c.a = 1;
        graphic.color = c;//一开始透明度为1
        Sequence mySequence = DOTween.Sequence();
        mySequence.OnComplete(NumShowEnqueue);//动画序列结束时进池

        if(dir==NumShowDir.Left)//数字出现在左边，消失在左边
        {
            rt.position = Camera.main.WorldToScreenPoint(unitPos) + actualNormalStartOffsetL;//起始位置在左边
            Vector3 targetPos = rt.position + actualNormalEndOffsetL + randomOffsetV3;
            float jumpPower = 70f;//1920下跳跃力
            float actualJumpPower = jumpPower * ((float)Screen.height / 1080);
            Sequence posChange = rt.DOJump(targetPos, actualJumpPower, 1, 1.3f);//终点位置在左边
            mySequence.Append(posChange);
        }
        else
        {
            rt.position = Camera.main.WorldToScreenPoint(unitPos) + actualNormalStartOffsetR;//起始位置在右边
            Vector3 targetPos = rt.position - actualNormalEndOffsetL + randomOffsetV3;
            float jumpPower = 70f;//1920下跳跃力
            float actualJumpPower = jumpPower * ((float)Screen.height / 1080);
            Sequence posChange = rt.DOJump(targetPos, actualJumpPower, 1, 1.3f);//终点位置在右边
            mySequence.Append(posChange);
        }
        Tweener scaleChange = DOTween.To(() => numT.fontSize, x => numT.fontSize = x, 16, 1.3f);
        Tweener alphaChange = graphic.DOColor(new Color(c.r, c.g, c.b, 0), 1f);
        mySequence.Join(scaleChange);
        mySequence.Join(alphaChange);

    }


    void ViolentFly(Graphic graphic, Vector3 unitPos, NumShowDir dir)
    {
        Vector3 randomOffsetV3 = RandomOffset();

        RectTransform rt = graphic.rectTransform;
        Color c = graphic.color;
        c.a = 1;
        graphic.color = c;//一开始透明度为1
        Sequence mySequence = DOTween.Sequence();
        mySequence.OnComplete(NumShowEnqueue);//动画序列结束时进池


        //公共脉动部分
        Tweener scaleChangeSToB = DOTween.To(() => numT.fontSize, x => numT.fontSize = x, 56, 0.5f);//脉动大
        scaleChangeSToB.SetEase(Ease.OutCubic);
        Tweener scaleChangeBToS = DOTween.To(() => numT.fontSize, x => numT.fontSize = x, 46, 0.4f);//脉动小
        scaleChangeBToS.SetEase(Ease.InCubic);
        scaleChangeBToS.OnComplete(FireHide);
        if (dir == NumShowDir.Left)//数字出现在左边，消失在左边
        {
            rt.position = Camera.main.WorldToScreenPoint(unitPos) + actualVioStartL;//起始位置在左边
            Tweener move1 = rt.DOMove(rt.position + actualVioStopOffsetL + randomOffsetV3, 0.1f);//由小到正常的位置变化
            Tweener scaleChange1 = DOTween.To(() => numT.fontSize, x => numT.fontSize = x, 46, 0.1f);//由小到正常的大小变化
            scaleChange1.SetEase(Ease.InOutQuad);
            move1.OnComplete(FireShow);
            //
            Tweener move2 = rt.DOMove(rt.position + actualVioStopOffsetL + actualVioDisOffset + randomOffsetV3, 0.2f);//由正常的位置到消失的位置变化
            Tweener scaleChange2 = DOTween.To(() => numT.fontSize, x => numT.fontSize = x, 14, 0.2f);//由正常到小的大小变化
            Tweener alphaChange = graphic.DOColor(new Color(c.r, c.g, c.b, 0), 0.2f);
            alphaChange.SetEase(Ease.OutCubic);

            mySequence.Append(move1);
            mySequence.Join(scaleChange1);
            mySequence.Append(scaleChangeSToB);
            mySequence.Append(scaleChangeBToS);
            mySequence.Append(move2);
            mySequence.Join(scaleChange2);
            mySequence.Join(alphaChange);
        }
        else
        {
            rt.position = Camera.main.WorldToScreenPoint(unitPos) + actualVioStartR;//起始位置在右边
            Tweener move1 = rt.DOMove(rt.position + actualVioStopOffsetR + randomOffsetV3, 0.1f);//由小到正常的位置变化
            Tweener scaleChange1 = DOTween.To(() => numT.fontSize, x => numT.fontSize = x, 46, 0.1f);//由小到正常的大小变化
            scaleChange1.SetEase(Ease.InOutQuad);
            move1.OnComplete(FireShow);
            //
            Tweener move2 = rt.DOMove(rt.position + actualVioStopOffsetR + actualVioDisOffset + randomOffsetV3, 0.2f);//由正常的位置到消失的位置变化
            Tweener scaleChange2 = DOTween.To(() => numT.fontSize, x => numT.fontSize = x, 14, 0.2f);//由正常到小的大小变化
            Tweener alphaChange = graphic.DOColor(new Color(c.r, c.g, c.b, 0), 0.2f);
            alphaChange.SetEase(Ease.OutCubic);

            mySequence.Append(move1);
            mySequence.Join(scaleChange1);
            mySequence.Append(scaleChangeSToB);
            mySequence.Append(scaleChangeBToS);
            mySequence.Append(move2);
            mySequence.Join(scaleChange2);
            mySequence.Join(alphaChange);
        }

    }

    void RecoverFly(Graphic graphic, Vector3 unitPos)
    {
        RectTransform rt = graphic.rectTransform;
        Color c = graphic.color;
        c.a = 1;
        graphic.color = c;//一开始透明度为1
        Sequence mySequence = DOTween.Sequence();
        mySequence.OnComplete(NumShowEnqueue);//动画序列结束时进池

        rt.position = Camera.main.WorldToScreenPoint(unitPos) + actualRecoverStartOffset;//起始位置在人物上面一点

        float jumpPower = 90f;//1920下跳跃力
        float actualJumpPower = jumpPower * ((float)Screen.height / 1080);
        Sequence posChange = rt.DOJump((rt.position + 2* actualRecoverStartOffset), actualJumpPower, 1, 2f);//终点位置在起始位置上面一点
        mySequence.Append(posChange);

        Tweener scaleChange = DOTween.To(() => numT.fontSize, x => numT.fontSize = x, 32, 2f);
        Tweener alphaChange = graphic.DOColor(new Color(c.r, c.g, c.b, 0), 2f);
        alphaChange.SetEase(Ease.InQuart);
        mySequence.Join(scaleChange);
        mySequence.Join(alphaChange);
    }

    /// <summary>
    /// 产生一个普通，暴击伤害数字位置的随机偏移
    /// </summary>
    /// <returns></returns>
    Vector3 RandomOffset()
    {
        //1920下的随机偏移数值
        float randomX = Random.Range(-random, random);
        float randomY = Random.Range(-random, random);
        //实际
        float actualX = randomX * ((float)Screen.height / 1080);
        float actualY = randomY * ((float)Screen.height / 1080);

        Vector3 randomOffset = new Vector3(actualX, actualY, 0);
        return randomOffset;
    }






    void NumShowEnqueue()//进池
    {
        ObjectsPool.Instance.EnqueueInstance(nowPool, gameObject);
    }

    void FireShow()
    {
        fire.gameObject.SetActive(true);
    }

    void FireHide()
    {
        fire.gameObject.SetActive(false);
    }




}
