using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class HumanBodyTracking : MonoBehaviour
{

    [SerializeField]
    private Text positionText;

    [SerializeField]
    private GameObject skeletonPrefab;

    [SerializeField]
    [Range(-10.0f, 10.0f)]
    private float skeletonOffsetX = 0;

    [Range(-10.0f, 10.0f)]
    [SerializeField]
    private float skeletonOffsetY = 0;

    [Range(-10.0f, 10.0f)]
    [SerializeField]
    private float skeletonOffsetZ = 0;

    [SerializeField]
    private Text SpeedRigth;

    [SerializeField]
    private Text SpeedLeft;

    [SerializeField]
    public AudioClip audioBit1;

    [SerializeField]
    public AudioClip audioBit2;



    [SerializeField] private ARHumanBodyManager humanBodyManager;

    private Dictionary<TrackableId, HumanBoneController> skeletonTracker = new Dictionary<TrackableId, HumanBoneController>();

    private JointTracker[] jointTrackers;
    private TrailRenderer[] trailRendersTracked;

    private PoseController poseController = new PoseController();

    private double TimeCounter = 0;
    private double TimeMax = 0.5;

    //last jointTrackers positions
    Vector3 lastLeftHandPosition;
    Vector3 lastRightHandPosition;

    Vector3 lastLeftToesPosition;
    Vector3 lastRightToesPosition;


    //new jointTrackers positions
    Vector3 newLeftHandPosition;
    Vector3 newRightHandPosition;

    Vector3 newLeftToesPosition;
    Vector3 newRightToesPosition;

    public ARHumanBodyManager HumanBodyManagers
    {
        get { return humanBodyManager; }
        set { humanBodyManager = value; }
    }

    public GameObject SkeletonPrefab
    {
        get { return skeletonPrefab; }
        set { skeletonPrefab = value; }
    }

    void OnEnable()
    {
        TimeCounter = 0;
        UnityEngine.Debug.Assert(humanBodyManager != null, "Human body manager is required.");
        humanBodyManager.humanBodiesChanged += OnHumanBodiesChanged;
    }

    void OnDisable()
    {
        if (humanBodyManager != null)
            humanBodyManager.humanBodiesChanged -= OnHumanBodiesChanged;
    }
    void OnHumanBodiesChanged(ARHumanBodiesChangedEventArgs eventArgs)
    {
        //To get time of execution
        DateTime start = DateTime.Now;
        positionText.text = string.Empty;

        HumanBoneController humanBoneController;

        foreach (var humanBody in eventArgs.added)
        {
            if (!skeletonTracker.TryGetValue(humanBody.trackableId, out humanBoneController))
            {
                UnityEngine.Debug.Log($"Adding a new skeleton [{humanBody.trackableId}].");
                var newSkeletonGO = Instantiate(skeletonPrefab, humanBody.transform);

                if (jointTrackers == null)
                {
                    jointTrackers = newSkeletonGO.GetComponentsInChildren<JointTracker>();

                    lastLeftHandPosition = new Vector3(0, 0, 0);
                    lastRightHandPosition = new Vector3(0, 0, 0);
                    lastLeftToesPosition = new Vector3(0, 0, 0);
                    lastRightToesPosition = new Vector3(0, 0, 0);

                    newLeftHandPosition = new Vector3(0, 0, 0);
                    newRightHandPosition = new Vector3(0, 0, 0);
                    newLeftToesPosition = new Vector3(0, 0, 0);
                    newRightToesPosition = new Vector3(0, 0, 0);

                }
                if(trailRendersTracked == null)
                {
                    trailRendersTracked = newSkeletonGO.GetComponentsInChildren<TrailRenderer>();
                    UnityEngine.Debug.Log($"Adding cubes tracked [{trailRendersTracked.Count()}].");
                    foreach(var item in trailRendersTracked)
                        UnityEngine.Debug.Log($"Adding cubes tracked [{item.transform.parent.name}].");
                }

                humanBoneController = newSkeletonGO.GetComponent<HumanBoneController>();

                // add an offset just when the human body is added
                humanBoneController.transform.position = humanBoneController.transform.position +
                    new Vector3(skeletonOffsetX, skeletonOffsetY, skeletonOffsetZ);

                skeletonTracker.Add(humanBody.trackableId, humanBoneController);
            }

            humanBoneController.InitializeSkeletonJoints();
            humanBoneController.ApplyBodyPose(humanBody, Vector3.zero);
        }

        foreach (var humanBody in eventArgs.updated)
        {
            if (skeletonTracker.TryGetValue(humanBody.trackableId, out humanBoneController))
            {
                humanBoneController.ApplyBodyPose(humanBody, Vector3.zero);
            }
            if (jointTrackers != null)
            {
                jointTrackers = humanBoneController.GetComponentsInChildren<JointTracker>();
                foreach (JointTracker jointTracker in jointTrackers)
                {
                    //Update new position every (TimeMax - 0.1)
                    if (TimeCounter > TimeMax-0.1)
                    {
                        if (jointTracker.gameObject.transform.parent.name.Equals("LeftHand"))
                        {
                            lastLeftHandPosition = newLeftHandPosition;
                            newLeftHandPosition = jointTracker.gameObject.transform.position;
                        }

                        if (jointTracker.gameObject.transform.parent.name.Equals("RightHand"))
                        {
                            lastRightHandPosition = newRightHandPosition;
                            newRightHandPosition = jointTracker.gameObject.transform.position;
                        }
                        if (jointTracker.gameObject.transform.parent.name.Equals("LeftToesEnd"))
                        {
                            lastLeftToesPosition = newLeftToesPosition;
                            newLeftToesPosition = jointTracker.gameObject.transform.position;
                        }

                        if (jointTracker.gameObject.transform.parent.name.Equals("RightToesEnd"))
                        {
                            lastRightToesPosition = newRightToesPosition;
                            newRightToesPosition = jointTracker.gameObject.transform.position;
                        }
                    }
                }
                if (poseController.ResumePosition(jointTrackers,audioBit1))
                    positionText.text += $"REANUDAR";

                if (poseController.PausePosition(jointTrackers))
                    positionText.text += $"PAUSA";

                if (poseController.FinishPosition(jointTrackers, audioBit2))
                    positionText.text += $"FINALIZAR";

                if (poseController.RestartPosition(jointTrackers))
                    positionText.text += $"REINICIAR";                                
            }
        }

        foreach (var humanBody in eventArgs.removed)
        {
            UnityEngine.Debug.Log($"Removing a skeleton [{humanBody.trackableId}].");
            if (skeletonTracker.TryGetValue(humanBody.trackableId, out humanBoneController))
            {
                Destroy(humanBoneController.gameObject);
                skeletonTracker.Remove(humanBody.trackableId);
            }
        }

        //To get time execution
        DateTime end = DateTime.Now;
        TimeSpan ts = (end - start);
        TimeCounter += ts.TotalSeconds*100;

        //Calculate speed every TimeMax
        if (TimeCounter > TimeMax)
        {
            TimeCounter = 0;
            var speedLeftHand = Math.Round(getJointMovementSpeed(lastLeftHandPosition, newLeftHandPosition) * 10, 3);
            var speedRigthHand = Math.Round(getJointMovementSpeed(lastRightHandPosition, newRightHandPosition) * 10, 3);
            var speedLeftToes = Math.Round(getJointMovementSpeed(lastLeftToesPosition, newLeftToesPosition) * 10, 3);
            var speedRightToes = Math.Round(getJointMovementSpeed(lastRightToesPosition, newRightToesPosition) * 10, 3);

            SpeedLeft.text = string.Empty;
            SpeedLeft.text += $"Velocidad Mano Izquierda : {speedLeftHand}\n";

            SpeedLeft.text += $"Velocidad Pie Izquierdo : {speedLeftToes}\n";

            SpeedRigth.text = string.Empty;
            SpeedRigth.text += $"Velocidad Mano Derecha : {speedRigthHand}\n";

            SpeedRigth.text += $"Velocidad Pie Derecho : {speedRightToes}\n";


            if (trailRendersTracked != null)
                foreach (var cube in trailRendersTracked)
                {
                    double speed = 0;
                    switch (cube.transform.parent.name)
                    {
                        case "LeftHand": speed = speedLeftHand;break;
                        case "RightHand": speed = speedRigthHand; break;
                        case "LeftToesEnd": speed = speedLeftToes; break;
                        case "RightToesEnd": speed = speedRightToes; break;
                    }
                    if(0< speed && speed <= 5)
                    {
                        cube.material.color = Color.red;
                    }
                    else if (5 < speed && speed <= 10)
                    {
                        cube.material.color = Color.green;
                    }
                    else if (10 < speed && speed <= 15)
                    {
                        cube.material.color = Color.yellow;
                    }
                    else if (15 < speed && speed <= 20)
                    {
                        cube.material.color = Color.blue;
                    }
                    else if (20 < speed)
                    {
                        cube.material.color = Color.magenta;
                    }
                }
        }
    }

    double getJointMovementSpeed(Vector3 lastPosition, Vector3 newPostion)
    {
        var lastx = (float) Math.Round(lastPosition.x * 10,2);
        var lasty = (float)Math.Round(lastPosition.y * 10, 2);
        var lastz = (float)Math.Round(lastPosition.z * 10, 2);
        var newx = (float)Math.Round(newPostion.x * 10, 2); ;
        var newy = (float)Math.Round(newPostion.y * 10, 2); ;
        var newz = (float)Math.Round(newPostion.z * 10, 2); ;

        double distance = Math.Sqrt(Math.Pow(newx - lastx, 2)+ Math.Pow(newy - lasty, 2)+ Math.Pow(newz - lastz, 2));

        return distance/TimeMax;
    }
}
