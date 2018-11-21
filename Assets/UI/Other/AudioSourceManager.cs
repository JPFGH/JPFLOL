using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum MusicName
{
    WZZG,
    WZGG
}






public class AudioSourceManager : MonoBehaviour {

    public AudioSource AS;
    public Dictionary<MusicName, AudioClip> musicDic;





    // Use this for initialization
    void Start () {
        musicDic = new Dictionary<MusicName, AudioClip>();
        AS = Camera.main.GetComponent<AudioSource>();

        LoadAllMusic();


    }
	
	// Update is called once per frame
	void Update () {
        string sceneName = SceneManager.GetActiveScene().name;
        if (AS.isPlaying==false)
        {
            if (sceneName == "ChooseHero")
            {
                AS.PlayOneShot(musicDic[MusicName.WZZG]);
            }
            else if (sceneName == "JPF Moba")
            {
                AS.PlayOneShot(musicDic[MusicName.WZGG]);
            }
        }
	}


    void LoadAllMusic()
    {
        musicDic.Add(MusicName.WZZG, Resources.Load<AudioClip>("Music/WZZG"));
        musicDic.Add(MusicName.WZGG, Resources.Load<AudioClip>("Music/WZGG"));

    }







    
}
