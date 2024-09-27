using UnityEngine;
using Unity.MLAgents;

public class EnvController : MonoBehaviour
{
    public GameObject Goal;
    public GameObject Model;

    private Transform AreaTrans;
    private Transform GoalTrans;
    private Transform ModelTrans;

    private Vector3 areaInitPos;
    private Vector3 goalInitPos;
    private Quaternion goalInitRot;

    // Start is called before the first frame update
    void Start()
    {
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

        ModelTrans.position = areaInitPos + new Vector3(Random.Range(-5f, 5f), Random.Range(-5f, 5f), Random.Range(-5f, 5f));
    }
}
