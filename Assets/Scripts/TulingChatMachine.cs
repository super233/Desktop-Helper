using System;
using System.IO;
using System.Net;
using System.Text;
using Newtonsoft.Json.Linq;

public class TulingChatMachine
{
    //固定格式的JSON
    const string strJson1 = @"{""reqType"":0,""perception"":{""inputText"":{""text"":";
    const string strJson2 = @"}},""userInfo"":{""apiKey"":""fccb54b03b374ecca9f7f43be43ec2e4"",""userId"":""379525""}}";

    //浏览器路径
    const string browserPath = "C:/Program Files (x86)/Google/Chrome/Application/chrome.exe";

    //构造Http Reques，返回JSON的字符串
    private static string ConstructRequest(string input)
    {
        //构造完整的JSON字符串
        string strJsonSend = strJson1 + "\"" + input + "\"" + strJson2;

        //使用Newtonsoft将JOSN字符串序列化
        JObject jObject = JObject.Parse(strJsonSend);

        return jObject.ToString();
    }

    //发送Http Request，返回Http Response的字符串内容
    private static string GetResponse(string strRequest)
    {
        string strResponse = null;

        //初始化Http请求类，初始化Http头部数据
        HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create("http://openapi.tuling123.com/openapi/api/v2");
        webRequest.ContentType = "application/json";
        webRequest.Method = "POST";
        webRequest.ContentLength = Encoding.UTF8.GetByteCount(strRequest);

        //获得输出流，将请求数据发送过去
        using (StreamWriter streamWriter = new StreamWriter(webRequest.GetRequestStream()))
        {
            streamWriter.Write(strRequest);
        }

        //获得输入流，接受响应数据
        using (HttpWebResponse webResponse = (HttpWebResponse)webRequest.GetResponse())
        {
            StreamReader streamReader = new StreamReader(webResponse.GetResponseStream(), System.Text.Encoding.UTF8);
            strResponse = streamReader.ReadToEnd();
        }
        return strResponse;
    }

    //解析Response中的JSON
    private static string GetInformation(string strResponse)
    {
        string ret = "nothing";

        //整体构造成一个大的JSON类 
        JObject jObject = JObject.Parse(strResponse);

        //取出其中的results部分，results部分是一个数组
        JArray jArray = JArray.Parse(jObject["results"].ToString());
        for (int i = 0; i < jArray.Count; i++)
        {
            JObject tempObject = JObject.Parse(jArray[i].ToString());

            //提取新闻的URL
            if (tempObject["resultType"].ToString().Equals("news"))
            {
                JArray jArrayNews = JArray.Parse(JObject.Parse(tempObject["values"].ToString())["news"].ToString());

                //随机选择一个新闻打开
                int index = new Random().Next(0, jArrayNews.Count);
                string url = JObject.Parse(jArrayNews[index].ToString())["detailurl"].ToString();

                OpenWithBroser(url);
            }

            //只输出text的数据
            if (tempObject["resultType"].ToString().Equals("text"))
            {
                ret = JObject.Parse(tempObject["values"].ToString())["text"].ToString();
            }
        }

        return ret;
    }

    //浏览器打开网址
    private static void OpenWithBroser(string url)
    {
        //System.Diagnostics.Process.Start(browserPath, url);
        System.Diagnostics.Process.Start(url);      //使用系统默认浏览器打开
    }

    public static string Chat(string input)
    {
        string strRequest = ConstructRequest(input);
        string strResponse = GetResponse(strRequest);
        string output = GetInformation(strResponse);
        return output;
    }
}
