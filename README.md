# Desktop-Helper

***version：1.3***

## 目前具有的功能

1. 接入了图灵机器人API，点击==**F7**==打开、关闭输入框，按==**回车**==发送消息；消息停留5s后会逐渐变淡消失。
2. 机器人返回的URL会调用默认浏览器打开，例如：输入关键词“新闻。
3. Unity程序背景透明，且可以后台运行，不用怕失去鼠标焦点

## 待实现的功能

1. 后期根据输的入关键词还可以调用电脑中的其他应用。
2. 本来想将机器人的回答做成语音播放，但是Baidu语音合成的API是将文字上传并下载一个音频文件，但是使用Unity动态读取音频文件速度跟不上，我添加了一个5s的循环还是然并卵；想使用C#直接系统调用播放音频文件遇到种种问题……
3. 可以根据机器人的回答调用Baidu的文字情感分析API，返回2个指标(消极指数、积极指数)，然后就可以根据这个情感来设定我们需要展示的动作，但是调用API的时候又遇到问题……接口返回的是GBK编码，我这边一直报错，还没有找到方法。
4. 设定点击模型的身体的部位切换表情和动作，这个或许要研读它的给的SampleApp1。