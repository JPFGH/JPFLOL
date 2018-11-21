using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MiddleUIRoot : MonoBehaviour {

    public Image fillImage;
    public Text progressT;

    // Use this for initialization
    void Start () {
        progressT.text = "0%";

        StartCoroutine("LoadScene", PlayerSettingData.Instance.readyToLoad);
    }
	
	// Update is called once per frame
	void Update () {
		
	}


    IEnumerator LoadScene(string sceneName)
    {
        int displayProgress = 0;
        int toProgress = 0;
        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);
        op.allowSceneActivation = false;
        while (op.progress < 0.9f)
        {
            toProgress = (int)op.progress * 100;
            while (displayProgress < toProgress)
            {
                ++displayProgress;
                SetLoadingPercentage(displayProgress);
                yield return new WaitForEndOfFrame();
            }
        }

        toProgress = 100;
        while (displayProgress < toProgress)
        {
            ++displayProgress;
            SetLoadingPercentage(displayProgress);
            yield return new WaitForEndOfFrame();
        }
        op.allowSceneActivation = true;
    }

    void SetLoadingPercentage(int num)
    {
        progressT.text=string.Format("{0}%",num);
        fillImage.fillAmount = (float)num / 100;
    }





}
