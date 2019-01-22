using System.Collections.Generic;
using System.IO;
using Baidu.Aip;
using Baidu.Aip.Speech;

public class WordToVoice
{
    //调用接口的认证信息
    const string APP_ID = "15452740";
    const string API_KEY = "lKpbG49cPUaVelXFmzX9WemG";
    const string SECRET_KEY = "cqrilsNCT6Ga5X7GRTDxcrlR4cYfWIho";

    //百度文字转语音接口类
    private static Tts tts = new Tts(API_KEY, SECRET_KEY);

    //参数
    private static Dictionary<string, object> options = new Dictionary<string, object>()
    {
        {"spd", 5}, // 语速
        {"vol", 7}, // 音量
        {"per", 4}  // 发音人，4：情感度丫丫童声
    };

    //文字转语音，将音频文件输出到指定的路径
    public static void Transform(string strWord, string path)
    {
        TtsResponse result = null;
        try
        {
            result = tts.Synthesis(strWord, options);
            if (result.ErrorCode == 0)  // 或 result.Success
            {
                File.WriteAllBytes(path, result.Data);
            }
        }
        catch (AipException exception)
        {
            throw exception;
        }
    }
}
