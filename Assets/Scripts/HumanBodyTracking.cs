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
    private Text PositionRigth;

    [SerializeField]
    private Text SpeedLeft;

    [SerializeField]
    private Text PositionLeft;

    [SerializeField] private ARHumanBodyManager humanBodyManager;

    private Dictionary<TrackableId, HumanBoneController> skeletonTracker = new Dictionary<TrackableId, HumanBoneController>();

    private JointTracker[] jointTrackers;

    private PoseController poseController = new PoseController();

    private double TimeCounter = 0; 

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

        SpeedRigth.text = string.Empty;
        SpeedLeft.text = string.Empty;

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
                    foreach(JointTracker jointTracker in jointTrackers)
                    {
                        SpeedLeft.text += $" Bone: {jointTracker.gameObject.transform.parent.name} Position: {jointTracker.gameObject.transform.position} \n";
                        //SpeedLeft.text += $" Bone: {jointTracker.gameObject.transform.parent.name} Local Position: {jointTracker.gameObject.transform.localPosition} \n";
                        //calculate speed
                        
                        //SpeedRigth.text += $" Bone: {jointTracker.gameObject.transform.parent.name} Local Position: {jointTracker.gameObject.transform.localPosition} \n";
                    }
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
                //get last jointTrackers positions
                Vector3 lastLeftHandPosition = new Vector3(0, 0, 0);
                Vector3 lastRightHandPosition = new Vector3(0, 0, 0);

                foreach (JointTracker jointTracker in jointTrackers)
                {
                    if (jointTracker.gameObject.transform.parent.name.Equals("LeftHand"))
                        lastLeftHandPosition = jointTracker.gameObject.transform.position;

                    if (jointTracker.gameObject.transform.parent.name.Equals("RightHand"))
                        lastRightHandPosition = jointTracker.gameObject.transform.position;
                }
                //get new jointTrackers positions
                Vector3 newLeftHandPosition = new Vector3(0, 0, 0);
                Vector3 newRightHandPosition = new Vector3(0, 0, 0);

                jointTrackers = humanBoneController.GetComponentsInChildren<JointTracker>();
                foreach (JointTracker jointTracker in jointTrackers)
                {
                    SpeedLeft.text += $" Bone: {jointTracker.gameObject.transform.parent.name} Position: {jointTracker.gameObject.transform.position} \n";
                    //SpeedLeft.text += $" Bone: {jointTracker.gameObject.transform.parent.name} Local Position: {jointTracker.gameObject.transform.localPosition} \n";

                    if (jointTracker.gameObject.transform.parent.name.Equals("LeftHand"))
                    {
                        newLeftHandPosition = jointTracker.gameObject.transform.position;
                        //The world space position of the transform
                        /*
                        newLeftHandPosition = jointTracker.gameObject.transform.position;
                        SpeedLeft.text += $"X: {newLeftHandPosition.x}\n";
                        SpeedLeft.text += $"Y: {newLeftHandPosition.y}\n";
                        SpeedLeft.text += $"Z: {newLeftHandPosition.z}\n";
                        //SpeedLeft.text += $"Velocidad(I): {getJointMovementSpeed(lastLeftHandPosition, newLeftHandPosition)}";*/
                    }

                    if (jointTracker.gameObject.transform.parent.name.Equals("RightHand"))
                    {
                        newRightHandPosition = jointTracker.gameObject.transform.position;
                        /*SpeedRigth.text += $"X: {newRightHandPosition.x}\n";
                        SpeedRigth.text += $"Y: {newRightHandPosition.y}\n";
                        SpeedRigth.text += $"Z: {newRightHandPosition.z}\n";
                        //SpeedRigth.text += $"Velocidad(D): {getJointMovementSpeed(lastRightHandPosition, newRightHandPosition)}";*/
                    }
                        
                }

                SpeedRigth.text += $"Resumen: {poseController.ResumePosition(jointTrackers)}\n";
                SpeedRigth.text += $"Pausa: {poseController.PausePosition(jointTrackers)}\n";

                var speed = getJointMovementSpeed(lastLeftHandPosition, newLeftHandPosition);
                //SpeedRigth.text += $"Velocidad Izquierda: {speed}";
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
        /*SpeedRigth.text += $"Tiempo Inicio: {start:MM/dd/yyyy hh:mm:ss.fff tt}\n";
        SpeedRigth.text += $"Tiempo Fin: {end:MM/dd/yyyy hh:mm:ss.fff tt}\n";*/
        TimeCounter += ts.TotalSeconds*100;

        if (TimeCounter > 1)
            TimeCounter = 0;
        //SpeedLeft.text += $"Tiempo: {ts.TotalMilliseconds} ms\n";
        //SpeedLeft.text += $"Tiempo Ejecutando: {TimeCounter} sec\n";
       
    }

    double getJointMovementSpeed(Vector3 lastPosition, Vector3 newPostion)
    {
        //var speed = Mathf.Pow((int)(newPostion.x*10) - (int)(lastPosition.x*10), 2) + Mathf.Pow((int)(newPostion.y*10) - (int)(lastPosition.y*10), 2) + Mathf.Pow((int)(newPostion.z*10) - (int)(lastPosition.z*10), 2);
        var lastx = (float) Math.Round(lastPosition.x * 10,2);
        var lasty = (float)Math.Round(lastPosition.y * 10, 2);
        var lastz = (float)Math.Round(lastPosition.z * 10, 2);
        var newx = (float)Math.Round(newPostion.x * 10, 2); ;
        var newy = (float)Math.Round(newPostion.y * 10, 2); ;
        var newz = (float)Math.Round(newPostion.z * 10, 2); ;
        //var distance = Math.Sqrt(Math.Pow(newx - lastx, 2)+ Math.Pow(newy - lasty, 2)+ Math.Pow(newz - lastz, 2));
        var distance = newx - lastx;
        //return Mathf.Sqrt(speed);
        return distance;
    }
}
