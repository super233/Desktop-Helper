using System;
using System.Collections.Generic;
using Baidu.Aip.Nlp;
using System.Text;
using Newtonsoft.Json.Linq;
using UnityEngine.UI;
using UnityEngine;

// 对文字进行情感识别
public class EmotionRecognition
{
    // 配置Baidu API的接口信息
    private static readonly string APP_ID = "15454811";
    private static readonly string API_KEY = "b40EHjAtBMaI7I38ZxZHxVcu";
    private static readonly string SECRET_KEY = "1vsZzpYpAAmbgTaa7vi4cyf7pHvq5W92";

    private static Nlp client = new Baidu.Aip.Nlp.Nlp(API_KEY, SECRET_KEY);

    //static void Main(string[] args)
    //{
    //    //Encoding.RegisterProvider(Encoding.CodePagesEncodingProvider.Instance);

    //    Dictionary<string, float> ret = Recognition("你好");

    //    foreach (KeyValuePair<string, float> item in ret)
    //    {
    //        Console.WriteLine(item.Key + ": " + item.Value);
    //    }
    //}

    // 传入一个文本，返回3种情感的指数
    public static Dictionary<string, float> Recognition(string text)
    {
        Dictionary<string, float> ret = new Dictionary<string, float>();

        // 调用对话情绪识别接口，可能会抛出网络等异常，请使用try/catch捕获
        JObject result = client.Emotion(text);

        JArray jArray = JArray.Parse(result["items"].ToString());
        for(int i = 0; i < jArray.Count; i++)
        {
            JObject tempObject = JObject.Parse(jArray[i].ToString());
            ret[tempObject["label"].ToString()] = float.Parse(tempObject["prob"].ToString());
        }

        Debug.Log(text);
        //Display(ret);

        return ret;
    }

    // 打印情感分析的结果
    public static void Display(Dictionary<string, float> dict)
    {
        string strDisplay = "情感指数\n";

        foreach (KeyValuePair<string, float> item in dict)
        {
            strDisplay += item.Key + ": " + item.Value.ToString() + ", ";
        }

        Debug.Log(strDisplay);
    }
}