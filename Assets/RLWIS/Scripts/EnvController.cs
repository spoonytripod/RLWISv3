using UnityEngine;
using Unity.MLAgents;

public class EnvController : MonoBehaviour
{
    public GameObject Goal;
    public GameObject Model;
    private PoseCalculator poseCalculator;

    [Space(10)]
    [Range(1f, 10f)] public float randPosInnerRatio;
    [Range(1f, 10f)] public float randPosOuterRatio;
    public float goalDistance;
    public float failDistance;

    private Transform AreaTrans;
    private Transform GoalTrans;
    private Transform ModelTrans;

    private Vector3 areaInitPos;
    private Vector3 goalInitPos;
    private Quaternion goalInitRot;

    // Start is called before the first frame update
    void Start()
    {
        poseCalculator = gameObject.GetComponent<PoseCalculator>();
        poseCalculator.DetectMarkers();

        AreaTrans = gameObject.transform;
        ModelTrans = Model.transform;
        GoalTrans = Goal.transform;

        areaInitPos = AreaTrans.position;
        goalInitPos = GoalTrans.position;
        goalInitRot = GoalTrans.rotation;
    }

    public void MoveModel(float x, float y, float z)
    {
        Vector3 direction = new Vector3(x, y, z);
        Model.transform.Translate(direction);
    }

    public void AreaSetting()
    {
        GoalTrans.position = goalInitPos;
        GoalTrans.rotation = goalInitRot;

        Vector3 randPos = Random.onUnitSphere * Random.Range(goalDistance * randPosInnerRatio, goalDistance * randPosOuterRatio);
        ModelTrans.position = areaInitPos + randPos;
    }
}
