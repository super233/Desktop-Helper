using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;


public class ChatWithTuling : MonoBehaviour
{
    // 情感指数相关的变量
    public static float rate = 0;   // 积极指数 - 消极指数的值
    public static bool flag = false;    // 表示是否有数值产生

    //UI类
    public Text text;
    public InputField inputField;

    //GameObject类，在Unity中，文件是一样的，只是UI类方便取数据，GameObject类方便设置可见性
    public GameObject textObject;
    public GameObject inputFiledObject;

    //计时器
    private float timePassed = 0;
    const float T_TOTAL = 10;
    const float T_BEGIN = 5;

    //音频相关
    private AudioSource audioSource;        //控制音频播放的类
    private int index = 0;

    // Start is called before the first frame update
    void Start()
    {
        //首先设置不可见
        textObject.SetActive(false);
        inputFiledObject.SetActive(false);

        //从Unity获得音频控制器
        audioSource = this.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        //按F7键打开与关闭输入框
        if (Input.GetKeyDown(KeyCode.F7))
        {
            inputFiledObject.SetActive(!inputFiledObject.activeSelf);
        }

        //当输入框打开时，回车发送消息
        if (inputFiledObject.activeSelf == true && (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)))
        {
            string output = TulingChatMachine.Chat(inputField.text);        //发送消息，并接受消息

            // 获取情绪识别的结果，并计算rate值
            Dictionary<string, float> recogRet = EmotionRecognition.Recognition(output);
            rate = recogRet["optimistic"] - recogRet["pessimistic"];
            flag = true;

            ////语音转文字，存储语音
            //if (inputField.text != null)
            //{
            //    try
            //    {
            //        WordToVoice.Transform(output, Application.dataPath + "/Resources/Sounds/test" + index + ".mp3");

            //        long time = System.DateTime.Now.Ticks;
            //        while(true)
            //        {
            //            long timeTemp = System.DateTime.Now.Ticks;
            //            if ((timeTemp - time) / 10000000 > 5)
            //            {
            //                break;
            //            }
            //        }

            //        LoadAndPlayAudio(index);     //播放语音
            //        index++;
            //    }
            //    catch (AipException exception)
            //    {
            //        print("调用失败Message: " + exception.Message + ", Code: " + exception.Code + "\n");
            //        print("Num: " + index + "\n");

            //    }
            //}

            inputField.text = null;         //输入框清空

            textObject.SetActive(true);         //显示文字
            timePassed = 0;         //计时器清0
            text.color = new UnityEngine.Color(1, 1, 1, 1);         //字体颜色设为初始状态
            text.text = output;         //显示收到的消息
        }

        //如果文字被显示出来了，计时，修改字体不可见度
        if (textObject.activeSelf == true)
        {
            timePassed += Time.deltaTime;

            //过了一段时间后才开始变色
            if (timePassed >= T_BEGIN)
            {
                text.color = new Color(1, 1, 1, 1 - (timePassed - T_BEGIN) / (T_TOTAL - T_BEGIN));
            }
        }

        //时间到了，计时器清0，文字取消显示
        if (timePassed >= T_TOTAL)
        {
            timePassed = 0;
            textObject.SetActive(false);
        }

    }

    private void LoadAndPlayAudio(int num)
    {
        //加载音频文件，使用Resources.Load的文件必须在Rexources文件夹下，且不加后缀
        AudioClip audioClip = Resources.Load<AudioClip>("Sounds/test" + num);
        audioSource.clip = audioClip;

        //播放
        audioSource.Play();
        print("Play The test" + num + ".mp3\n");
    }
}
