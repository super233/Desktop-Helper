using UnityEngine;
using UnityEngine.EventSystems;
using live2d;
using live2d.framework;
using System;

public class Live2DModel : MonoBehaviour
{
    //模型相关
    public TextAsset mocFile;                           //从Unity内获取的模型文件（moc）
    public Texture2D[] textureFiles;                    //从Unity内获取贴图文件
    private Live2DModelUnity live2DModel;               //模型对象
    private Matrix4x4 live2DCancasPos;                  //用于显示模型的画布

    //物理过程相关
    public TextAsset physicsFile;                       //存储物理过程的json文件
    private L2DPhysics physics;                         //物理过程的对象，可以在Update()中更新模型参数

    //动作相关
    public TextAsset[] mtnFiles;                        //从Unity内获取的动作文件(mtn)
    private Live2DMotion[] motions;                     //动作对象数组
    private L2DMotionManager motionManager = new L2DMotionManager();             //动作管理者对象
    private EyeBlinkMotion eyeBlinkMotion = new EyeBlinkMotion();               //眨眼的动作对象
    private int motionIndex = 0;

    //表情相关
    public TextAsset[] expressionFiles;                 //存储表情文件
    private L2DExpressionMotion[] expressions;          //存储表情对象
    private L2DMotionManager expressionManager = new L2DMotionManager();        //表情管理者对象
    private int expressionIndex = 0;

    //拖拽相关
    private L2DTargetPoint l2DTargetPoint = new L2DTargetPoint();           //用于存储坐标

    //音频相关
    public AudioClip[] audioClips;                      //从Unity获取的音频(mp3)文件
    public AudioSource audioSource;                     //从Unity获得的AudioSource对象，用于控制音频的播放
    private int audioIndex = 0;

    const double PARAM = 1.5;

    // Start is called before the first frame update
    void Start()
    {
        //初始化Live2D环境资源，调用Live2D API之前必须调用
        Live2D.init();

        //加载文件
        LoadModelAndTextures();
        LoadMotion();
        LoadPhysics();
        LoadExpression();

        audioSource = this.GetComponent<AudioSource>();

        //设置运行时的屏幕大小
        //Screen.SetResolution(420, 600, false);
    }

    // Update is called once per frame
    void Update()
    {
        //为模型设置用于显示的画布，后面是2个矩阵相乘
        live2DModel.setMatrix(transform.localToWorldMatrix * live2DCancasPos);

        //设置物理过程，更新模型的参数
        physics.updateParam(live2DModel);

        //设置眨眼动作，设置模型的参数
        eyeBlinkMotion.setParam(live2DModel);

        //按M键切换动作并播放声音
        if (motionManager.isFinished())             //动作完成，切换到默认的动作
        {
            motionManager.startMotionPrio(motions[0], 1);       //默认的动作的优先级为1，数值较高，优先级较大
        }
        else if (Input.GetKeyDown(KeyCode.M))
        {
            motionManager.startMotionPrio(motions[motionIndex], 2);     //新动作的优先级为2
            motionIndex++;

            print("motion index: " + motionIndex + "\n");

            if (motionIndex >= motions.Length)
            {
                motionIndex = 0;
            }


            //播放声音
            audioSource.clip = audioClips[audioIndex];
            audioSource.Play();

            audioIndex++;
            if (audioIndex >= audioClips.Length)
            {
                audioIndex = 0;
            }

        }
        motionManager.updateParam(live2DModel);             //设置了动作后，更新模型的参数

        //表情的动作是一直保持的
        if (Input.GetKeyDown(KeyCode.E))
        {
            expressionManager.startMotion(expressions[expressionIndex]);

            print("expression index: " + expressionIndex + "\n");

            expressionIndex++;
            if (expressionIndex >= expressionFiles.Length)
            {
                expressionIndex = 0;
            }
        }
        expressionManager.updateParam(live2DModel);

        Vector3 mousePos = Input.mousePosition;         //获得鼠标的坐标

        //print("Mouse Position: (" + mousePos.x + ", " + mousePos.y + ")\t" + "Width: " + Screen.width + ", Height: " + Screen.height + "\n");
        
        ////切换动作
        //if (motionManager.isFinished())             //动作完成，切换到默认的动作
        //{
        //    motionManager.startMotionPrio(motions[0], 1);       //默认的动作的优先级为1，数值较高，优先级较大
        //}
        //else if (Input.GetMouseButtonDown(0) && mousePos.x >= 61 * PARAM && mousePos.x <= 140 * PARAM && mousePos.y >= 130 * PARAM && mousePos.y <= 273 * PARAM)
        //{
        //    motionManager.startMotionPrio(motions[motionIndex], 2);     //新动作的优先级为2
        //    motionIndex++;

        //    print("motion index: " + motionIndex + "\n");

        //    if (motionIndex >= motions.Length)
        //    {
        //        motionIndex = 0;
        //    }
        //}
        //motionManager.updateParam(live2DModel);             //设置了动作后，更新模型的参数

        ////切换表情
        //if (Input.GetMouseButtonDown(1) && mousePos.x >= 65 * PARAM && mousePos.x <= 140 * PARAM && mousePos.y >= 300 * PARAM && mousePos.y <= 387 * PARAM)
        //{
        //    expressionManager.startMotion(expressions[expressionIndex]);
        //    expressionManager.updateParam(live2DModel);

        //    print("expression index: " + expressionIndex + "\n");

        //    expressionIndex++;
        //    if (expressionIndex >= expressionFiles.Length)
        //    {
        //        expressionIndex = 0;
        //    }
        //}

        //更新模型参数，使模型随着鼠标运动
        l2DTargetPoint.Set(mousePos.x / Screen.width * 2 - 1, mousePos.y / Screen.height * 2 - 1);      //将鼠标坐标缩放到[-1, 1]，然后存储到l2DTargetPoint中     
        l2DTargetPoint.update();

        //从l2DTargetPoint中取出坐标，用于更新模型的参数
        live2DModel.setParamFloat("PARAM_ANGLE_X", l2DTargetPoint.getX() * 30);
        live2DModel.setParamFloat("PARAM_ANGLE_Y", l2DTargetPoint.getY() * 30);
        live2DModel.setParamFloat("PARAM_BODY_ANGLE_X", l2DTargetPoint.getX() * 10);
        //live2DModel.setParamFloat("PARAM_BODY_ANGLE_Y", l2DTargetPoint.getY() * 10);
        live2DModel.setParamFloat("PARAM_EYE_BALL_X", l2DTargetPoint.getX());
        live2DModel.setParamFloat("PARAM_EYE_BALL_Y", l2DTargetPoint.getY());

        //更新模型的参数，放在Updat()函数后面
        live2DModel.update();


        
    }

    private void OnRenderObject()
    {
        //渲染显示模型
        live2DModel.draw();
    }

    //加载模型文件与贴图文件，并初始化画布
    private void LoadModelAndTextures()
    {
        //读取模型，bytes形式读取
        live2DModel = Live2DModelUnity.loadModel(mocFile.bytes);

        //读取贴图
        for (int i = 0; i < textureFiles.Length; i++)
        {
            live2DModel.setTexture(i, textureFiles[i]);
        }

        //初始化显示的画布，后面的参数一定不要错，😭
        float modelWidth = live2DModel.getCanvasWidth();
        live2DCancasPos = Matrix4x4.Ortho(0, modelWidth, modelWidth, 0, -50, 50);
    }


    //加载动作文件，并完成与动作有关的变量的初始化
    private void LoadMotion()
    {
        //读取动作文件
        motions = new Live2DMotion[mtnFiles.Length];
        for (int i = 0; i < mtnFiles.Length; i++)
        {
            motions[i] = Live2DMotion.loadMotion(mtnFiles[i].bytes);
        }
    }

    //加载表情文件
    private void LoadExpression()
    {
        //读取表情文件
        expressions = new L2DExpressionMotion[expressionFiles.Length];
        for (int i = 0; i < expressionFiles.Length; i++)
        {
            expressions[i] = L2DExpressionMotion.loadJson(expressionFiles[i].bytes);
        }
    }

    //加载物理文件
    private void LoadPhysics()
    {
        physics = L2DPhysics.load(physicsFile.bytes);
    }
}