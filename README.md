# Desktop-Helper

[TOC]

## ChangeList

- 2019/01/22			[Add] 构建了基本的Live 2D桌面应用，添加了图灵机器人聊天，模型跟随鼠标运动的功能
- 2019/05/28            [Add, Fix] 添加了语音输入、情绪识别的功能，将动作与情绪关联了起来

## 目前具有的功能 

1. 接入了图灵机器人API，点击*F7*打开、关闭输入框，按*Enter*发送消息；消息停留5s后会逐渐变淡消失
2. 机器人返回的URL会调用默认浏览器打开，例如：输入关键词“新闻
3. Unity程序背景透明，且可以后台运行，不用怕失去鼠标焦点
4. 增加了语音输入的功能，按住*N*键进行语音输入
5. 增加了情绪识别的功能，根据图灵机器人的回复，展示不同情绪的动作

## TODO

- [ ] 将语音输入的启动方式改为`CTRL+N`

 - [ ] 再细分下表情、动作，在情感指数和动作、表情之间建立一个更加准确的映射关系
 - [ ] 将爬虫模块和Unity这边联系起来

## 文件夹目录结构

```
SimpleLive2D
	|——Assets
	|	  |——Baidu				//百度SDK
	|	  |——Resources			//资源文件夹，包含人物模型文件、音频文件
	|	  |——Scenes				//存放Unity中的scene文件
	|     |——Scripts			//项目的C#代码
	|	  |——UnityPackages		//UnityPackages SDK
	|——README.md				//项目说明
	|——SimpleLive2D.sln			//Visual Studio工程文件
```

## 打开方式

1. 直接打开`SimpleLive2D/Assets/Scenes`下的`SampleScene.unity`，就可以在Unity内打开了
2. 点击`SimpleLive2D.sln`，就可以在Visual Studio下打开项目文件

## FAQ

1. 如果在Unity中出现`Assertion failed: Assertion failed on expression: '!GetMainEditorWindow()'`的错误的话，是老师您的Unity没有登陆，点击Unity的快捷方式启动Unity，然后登陆您的账号(可以微信登陆)，再在Unity客户端内打开该Unity工程就可以了。
2. 貌似由于**Unity版本的原因**，在2017版本的Unity内调用情绪识别的API会报错`TlsException:Invalid certification`