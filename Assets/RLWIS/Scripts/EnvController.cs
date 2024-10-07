using UnityEngine;
using Unity.MLAgents;
using OpenCVForUnity.UnityUtils;
using System;

public class EnvController : MonoBehaviour
{
    public GameObject Goal;
    public GameObject Model;
    private PoseCalculator poseCalculator;

    [Space(10)]
    public int targetImgAmount;
    public ChangeTargetMode changeTargetMode;
    [Range(0, 10)] public int changeTargetPeriod;
    [Tooltip("Only for 'Single' mode, start from 0")]
    [Range(1f, 10f)] public float randPosInnerRatio;
    [Range(1f, 10f)] public float randPosOuterRatio;
    public float goalDistance;
    public float failDistance;

    private Transform AreaTrans;
    private Transform GoalTrans;
    private Transform ModelTrans;

    [Header("Directory settings")]
    private int targetImgIndex;

    // Start is called before the first frame update
    void Start()
    {
        poseCalculator = gameObject.GetComponent<PoseCalculator>();
        targetImgIndex = 1;

        LoadTexture();
        poseCalculator.DetectMarkers();

        AreaTrans = gameObject.transform;
        ModelTrans = Model.transform;
        GoalTrans = Goal.transform;

        // areaInitPos = AreaTrans.position;
        // goalInitPos = GoalTrans.position;
        // goalInitRot = GoalTrans.rotation;
    }

    public void MoveModel(float x, float y, float z)
    {
        Vector3 direction = new Vector3(x, y, z);
        Model.transform.Translate(direction, Space.World); // Translate in World coordinates
    }

    public void AreaSetting()
    {
        Matrix4x4 ARM = poseCalculator.ARM_Object;

        // Set Random values
        Vector3 randPos = UnityEngine.Random.onUnitSphere * UnityEngine.Random.Range(goalDistance * randPosInnerRatio, goalDistance * randPosOuterRatio);
        Quaternion randRot = GoalTrans.rotation; // 6DOF 랜덤 Rotation  적용하기 전까지 이대로 사용

        // Set position and rotation of the Goal
        GoalTrans.position = ARUtils.ExtractTranslationFromMatrix(ref ARM);
        GoalTrans.rotation = ARUtils.ExtractRotationFromMatrix(ref ARM);

        // Set position and rotation of the Model
        ModelTrans.position = GoalTrans.position + randPos;
        ModelTrans.rotation = randRot;
    }

    public void LoadTexture()
    {
        Texture2D targetTexture = Resources.Load<Texture2D>("Targets/" + targetImgIndex.ToString());
        poseCalculator.imgTexture = targetTexture;
    }
    public void NextTarget()
    {
        targetImgIndex += 1;
        LoadTexture();
        try
        {
            poseCalculator.DetectMarkers();
        }
        catch (NullReferenceException ex)
        {
            targetImgIndex = 0;
            LoadTexture();
            poseCalculator.DetectMarkers();
        }
    }

    public void PrevTarget()
    {
        targetImgIndex -= 1;
        LoadTexture();
        try
        {
            poseCalculator.DetectMarkers();
        }
        catch (NullReferenceException ex)
        {
            targetImgIndex = targetImgAmount - 1;
            LoadTexture();
            poseCalculator.DetectMarkers();
        }
    }

    public void RandomTarget()
    {
        targetImgIndex = UnityEngine.Random.Range(0, targetImgAmount);
        LoadTexture();
        poseCalculator.DetectMarkers();
    }

    public enum ChangeTargetMode { Sequential, Random, Single }
}
