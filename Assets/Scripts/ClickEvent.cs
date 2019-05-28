using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Threading;
using System.IO;

public class ClickEvent : MonoBehaviour
{
    public InputField inputFiled;
    //public Button btn;
    public byte[] audioData;
    public float[] samples;
    private AudioSource audio; //存储录制的音频
    //public Text text;
    private int frequency = 16000; //采样率
    public string rec_result = null;//存储识别结果
    public bool is_get_result = false;//是否获得结果
    // Use this for initialization
    private const string app_id = "appid = 5ce8a516";
    private const string session_begin_params = "sub = iat, domain = iat, language = zh_cn, accent = mandarin, sample_rate = 16000, result_type = plain, result_encoding = utf-8";

    void Start()
    {
        //btn = this.GetComponent<Button>();
        //btn.onClick.AddListener(OnClick);
        audio = this.GetComponent<AudioSource>();
        //text = GameObject.Find("ChatText").GetComponent<Text>(); //GetComponent<Text>();

    }
    void Update()
    {
        //判断是否按下按键
        AlwaysClick();
        //如果已经返回结果，则修改文本框中内容
        if (is_get_result == true)
        {
            inputFiled.text = rec_result;
            //设置为false等待下一次语音输入
            is_get_result = false;
            rec_result = null;
        }
    }
    private void AlwaysClick()
    {
        if (Input.GetKeyDown("n"))
        {
            //如果当前麦克风没有记录，则开始记录
            if (!Microphone.IsRecording(null))
            {
                StartRecord();
            }
        }
        //n抬起则停止记录
        if (Input.GetKeyUp("n"))
        {
            StopRecord();
        }

        //Debug.Log("test thread");
    }
    //private void OnClick()
    //{
    //    Debug.Log("Test button down");
    //}
    private void StartRecord()
    {
        //停止可能存在的上一次记录然后开始记录，30为最大时长，null代表默认麦克风设备
        audio.Stop();
        audio.clip = Microphone.Start(null, false, 30, frequency);
        //Debug.Log(Microphone.GetPosition(null));
    }
    private void StopRecord()
    {
        //如果默认麦克风正在记录，则停止保存在audio.clip中
        if (Microphone.IsRecording(null))
        {
            Debug.Log(Microphone.IsRecording(null));
            Microphone.End(null);

            //AudioSource.PlayClipAtPoint(audio.clip, new Vector3(-109,-163,0));

            Debug.Log("语音识别启动");

            if (audio.clip == null)
            {
                Debug.Log("clip is null");
            }
            samples = new float[audio.clip.samples];
            //获取语音输入data
            audio.clip.GetData(samples, 0);

            //启动线程
            Thread thread = new Thread(new ThreadStart(ThreadFunction));
            thread.Start();
            //isSuccess = Inter_Audio_Word.init_audio(app_id, session_begin_params, audioData);
            /*if (isSuccess)
                Debug.Log("登录成功");
            else
                Debug.Log("登录失败");*/
            //Inter_Audio_Word.audio_iat()
        }

    }
    public void ThreadFunction()
    {
        //Float2Byte用于将获取的float数据转换成byte供科大讯飞读数据
        audioData = Float2Byte();
        //返回识别结果
        rec_result = Inter_Audio_Word.init_audio(app_id, session_begin_params, audioData);
        if (null != rec_result)
        {
            is_get_result = true;
        }
    }
    public byte[] Float2Byte()
    {
        //print(samples);
        short[] intData = new short[samples.Length];

        byte[] bytesData = new byte[samples.Length * 2];
        int rescaleFactor = 32767;

        for (int i = 0; i < samples.Length; i++)
        {
            intData[i] = (short)(samples[i] * rescaleFactor);
            byte[] byteArr = new byte[2];
            byteArr = BitConverter.GetBytes(intData[i]);
            byteArr.CopyTo(bytesData, i * 2);
        }
        return bytesData;
    }
}
