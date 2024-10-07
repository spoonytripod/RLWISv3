using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class AgentController : Agent
{
    public GameObject env;
    public GameObject goal;
    public GameObject model;

    private EnvController envController;

    private float preDist;
    private Transform modelTrans;
    private Transform goalTrans;

    private float goalDistance;
    private float failDistance;

    public override void Initialize()
    {
        envController = env.GetComponent<EnvController>();

        goalDistance = envController.goalDistance;
        failDistance = envController.failDistance;

        modelTrans = model.transform;
        goalTrans = goal.transform;

        Academy.Instance.AgentPreStep += WaitTimeInference;
    }

    public override void OnEpisodeBegin()
    {
        envController.AreaSetting();

        preDist = Vector3.Magnitude(goalTrans.position - modelTrans.position);
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // sensor.AddObservation(modelTrans.position - goalTrans.position);
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        AddReward(-0.01f);

        var actions = actionBuffers.ContinuousActions;

        float moveX = Mathf.Clamp(actions[0], -1, 1f);
        float moveY = Mathf.Clamp(actions[1], -1, 1f);
        float moveZ = Mathf.Clamp(actions[2], -1, 1f);

        envController.MoveModel(moveX, moveY, moveZ);

        float distance = Vector3.Magnitude(goalTrans.position - modelTrans.position);

        if (distance <= goalDistance) // Terminal state 0 : Goal
        {
            SetReward(1f);
            EndEpisode();
        }
        else if (distance > failDistance)  // Terminal state 0 : Fail (Out of area)
        {
            SetReward(-1f);
            EndEpisode();
        }
        else
        {
            float reward = preDist - distance;
            SetReward(reward);
            preDist = distance;
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActionsOut = actionsOut.ContinuousActions;

        continuousActionsOut[0] = Input.GetAxis("Horizontal"); // X-axis
        continuousActionsOut[1] = Input.GetAxis("Mouse ScrollWheel"); // Y-axis
        continuousActionsOut[2] = Input.GetAxis("Vertical"); // Z-axis

        if (Input.GetKey(KeyCode.PageUp))
        {
            envController.NextTarget();
            envController.AreaSetting();
        }
        if (Input.GetKey(KeyCode.PageDown))
        {
            envController.PrevTarget();
            envController.AreaSetting();
        }


    }

    public float DecisionWaitingTime = 5f;
    float m_currentTime = 0f;

    public void WaitTimeInference(int action)
    {
        if (Academy.Instance.IsCommunicatorOn)
        {
            RequestDecision();
        }
        else
        {
            if (m_currentTime >= DecisionWaitingTime)
            {
                m_currentTime = 0f;
                RequestDecision();
            }
            else
            {
                m_currentTime += Time.fixedDeltaTime;
            }
        }
    }
}
