using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.IO;

public class Inter_Audio_Word : MonoBehaviour {
    public static string init_audio(string my_appid,string session_begin_params,byte[] audio_data)
    {
        //Debug.Log(Application.internetReachability);
        //先登录
        int res = MscDLL.MSPLogin(null, null, my_appid);//用户名，密码，登陆信息，前两个为空
        string result;
        if (res!=(int)MscDLL.Errors.MSP_SUCCESS)
        {
            //说明登录失败
            Debug.Log("登录失败！");
            //return false;
        }
        Debug.Log("登陆成功！");
        //开始识别，包括开启会话、写入音频，获取结果、结束会话
        result = audio_iat(audio_data, session_begin_params);
        //结束登录，每次登录之后必须结束，如果没有结束下一次登录就会失败
        int ret_end = MscDLL.MSPLogout();
        if((int)MscDLL.Errors.MSP_SUCCESS != ret_end)
        {
            Debug.Log("Logout failed");
        }
        return result;
    }
    public static string audio_iat(byte[] audio_data, string session_begin_params)
    {
        string getData = null;//返回的文字
        int result = 0;
        int result_get_result=0;
        int rslt_status = 0;
        uint audio_len = (uint)audio_data.Length;
        MscDLL.RecogStatus recogStatus = 0;
        MscDLL.RecogStatus recogStatusGetResult = 0;
        MscDLL.AudioStatus audioStatus = MscDLL.AudioStatus.MSP_AUDIO_SAMPLE_FIRST;
        MscDLL.EpStatus epStatus = 0;
        IntPtr rec_result;
        IntPtr rec_get_result;
        //开启会话
        IntPtr sessionID =  MscDLL.QISRSessionBegin(null, session_begin_params,ref result);
        if (0 != result)
        {
            Debug.Log("SessionBegin failed");
        }
        //File.WriteAllBytes(@"d:\audio_bytes1.txt", audio_data);

        //循环将需要识别的audio_data发送给接口
        while (MscDLL.AudioStatus.MSP_AUDIO_SAMPLE_LAST != audioStatus)
        {
            //Debug.Log("AudioWrite");
            
            result = MscDLL.QISRAudioWrite(sessionID, audio_data, audio_len, audioStatus, ref epStatus, ref recogStatus);
            //MSP_SUCCESS 为 0;result = 0代表写入成功
            if (result != 0)
            {
                Debug.Log("Audio Write Error");
                break;
            }
            //Debug.Log("recogStatus:"+recogStatus);
            //如果识别成功则读取
            //if (MscDLL.RecogStatus.MSP_REC_STATUS_SUCCESS == recogStatus)
            //{
            //    rec_get_result = MscDLL.QISRGetResult(sessionID, ref recogStatusGetResult, 5000, ref result_get_result);
            //    Debug.Log(rec_get_result);
            //}


            //检测到音频后端点，停止写入
            if (MscDLL.EpStatus.MSP_EP_AFTER_SPEECH == epStatus)
            {
                audioStatus = MscDLL.AudioStatus.MSP_AUDIO_SAMPLE_LAST;
                Debug.Log("音频输入到达结束点");
                break;
            }
            else
                audioStatus = MscDLL.AudioStatus.MSP_AUDIO_SAMPLE_CONTINUE;

            //Thread.Sleep(1000);
        }
        recogStatus = 0;
        //MscDLL.RecogStatus.MSP_REC_STATUS_COMPLETE = 5

        //循环读取识别的结果
        while(MscDLL.RecogStatus.MSP_REC_STATUS_COMPLETE != recogStatus)
        {
            rec_result = MscDLL.QISRGetResult(sessionID,ref recogStatus, 5000, ref result);
            //Debug.Log("GetResult:" + recogStatus + result);
            //MSP_SUCCESS 为 0;
            if (result != 0)
            {
                Debug.Log("GetResult failed:"+result.ToString());
                break;
            }
            if(null != rec_result)
            {
                //intptr 转string
                //每次识别的结果保存在getdata中
                getData += Marshal.PtrToStringAnsi(rec_result);
                continue;
            }
            //避免浪费资源
            Thread.Sleep(200);
        }
        Debug.Log(getData);
        //结束会话
        int ret_end = MscDLL.QISRSessionEnd(sessionID, "normal end");
        if(0 != ret_end)
        {
            Debug.Log("sessionEnd failed");
        }
        return getData;
    }

}
