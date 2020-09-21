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
    private Text errorText;

    [SerializeField]
    private GameObject skeletonPrefab;

    [SerializeField]
    private GameObject esfera;

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

    [SerializeField]
    public AudioClip audioBit3;

    [SerializeField] private ARHumanBodyManager humanBodyManager;

    private Dictionary<TrackableId, HumanBoneController> skeletonTracker = new Dictionary<TrackableId, HumanBoneController>();

    private JointTracker[] jointTrackers;
    private TrailRenderer[] trailRendersTracked;

    private PoseController poseController = new PoseController();

    private double TimeCounter = 0;
    private double TimeMax = 0.5;

    DateTime positionDetectionTime;
    bool positionDetected = false;

    //last jointTrackers positions
    Vector3 lastLeftHandPosition;
    Vector3 lastRightHandPosition;

    Vector3 lastLeftToesPosition;
    Vector3 lastRightToesPosition;
    //Instantiated AR
    List<GameObject> ARObjects = new List<GameObject>();

    //new jointTrackers positions
    Vector3 newLeftHandPosition;
    Vector3 newRightHandPosition;

    Vector3 newLeftToesPosition;
    Vector3 newRightToesPosition;

    Vector3 codoDerecho;
    Vector3 codoIzquierdo;
    Vector3 hombroDerecho;
    Vector3 hombroIzquierdo;


    //Colors for tracked joints
    Color leftHandColor = Color.red;
    Color rightHandColor = Color.red;
    Color leftToesColor = Color.red;
    Color rightToesColor = Color.red;

    //AR States
    ARState CurrentState = ARState.Pausa;

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

    public GameObject Esfera
    {
        get { return esfera; }
        set { esfera = value; }
    }

    void OnEnable()
    {
        TimeCounter = 0;
        UnityEngine.Debug.Assert(humanBodyManager != null, "Human body manager is required.");
        positionDetectionTime = DateTime.Now;
        humanBodyManager.humanBodiesChanged += OnHumanBodiesChanged;
    }

    void OnDisable()
    {
        if (humanBodyManager != null)
            humanBodyManager.humanBodiesChanged -= OnHumanBodiesChanged;
    }
    void OnHumanBodiesChanged(ARHumanBodiesChangedEventArgs eventArgs)
    {
        try
        {
            //To get time of execution
            DateTime start = DateTime.Now;

            if (positionDetected && positionDetectionTime.AddSeconds(5) < DateTime.Now)
            {
                positionText.text = string.Empty;
                positionDetected = false;
            }

            HumanBoneController humanBoneController;

            foreach (var humanBody in eventArgs.added)
            {
                if (!skeletonTracker.TryGetValue(humanBody.trackableId, out humanBoneController))
                {
                    UnityEngine.Debug.Log($"Adding a new skeleton [{humanBody.trackableId}].");
                    errorText.text = string.Empty;

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

                        codoDerecho = new Vector3(0, 0, 0);
                        hombroDerecho = new Vector3(0, 0, 0);
                        codoIzquierdo = new Vector3(0, 0, 0);
                        hombroIzquierdo = new Vector3(0, 0, 0);

                    }
                    if (trailRendersTracked == null)
                    {
                        trailRendersTracked = newSkeletonGO.GetComponentsInChildren<TrailRenderer>();
                        UnityEngine.Debug.Log($"Adding cubes tracked [{trailRendersTracked.Count()}].");
                        foreach (var item in trailRendersTracked)
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
                    SpeedRigth.text = string.Empty;
                    foreach (JointTracker jointTracker in jointTrackers)
                    {
                        SpeedRigth.text += jointTracker.gameObject.transform.parent.name + " - ";
                        //Instanciar objetos
                        if (CurrentState.Equals(ARState.Reanudar))
                        {
                            if (jointTracker.gameObject.transform.parent.name.Equals("RightHand"))
                            {
                                var nuevaEsfera = Instantiate(Esfera, jointTracker.gameObject.transform.position, Quaternion.identity);
                                nuevaEsfera.transform.GetComponent<Renderer>().material.color = rightHandColor;
                                ARObjects.Add(nuevaEsfera);
                            }
                            if (jointTracker.gameObject.transform.parent.name.Equals("LeftHand"))
                            {
                                var nuevaEsfera = Instantiate(Esfera, jointTracker.gameObject.transform.position, Quaternion.identity);
                                nuevaEsfera.transform.GetComponent<Renderer>().material.color = leftHandColor;
                                ARObjects.Add(nuevaEsfera);
                            }
                        }
                        //Update new position every (TimeMax - 0.1)
                        if (TimeCounter > TimeMax - 0.1)
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
                            if (jointTracker.gameObject.transform.parent.name.Equals("RightForearm"))
                                codoDerecho = jointTracker.gameObject.transform.position;

                            if (jointTracker.gameObject.transform.parent.name.Equals("LeftForearm"))
                                codoIzquierdo = jointTracker.gameObject.transform.position;

                            if (jointTracker.gameObject.transform.parent.name.Equals("RightShoulder1"))
                                hombroDerecho = jointTracker.gameObject.transform.position;

                            if (jointTracker.gameObject.transform.parent.name.Equals("LeftShoulder1"))
                                hombroIzquierdo = jointTracker.gameObject.transform.position;

                            poseController.ActualizarCoordenadas(hombroDerecho, codoDerecho, newRightHandPosition, hombroIzquierdo, codoIzquierdo, newLeftHandPosition);


                            SpeedLeft.text = string.Empty;
                            SpeedLeft.text += $"Codo Derecho : {codoDerecho}\n";
                            SpeedLeft.text += $"Hombro Derecho : {hombroDerecho}\n";
                            SpeedLeft.text += $"Mano Derecho : {newRightHandPosition}\n";

                            var hombroCodoD = poseController.CalcularAnguloHombroCodoDerecha(hombroDerecho, codoDerecho);
                            var hombroCodoI = poseController.CalcularAnguloHombroCodoIzquierda(hombroIzquierdo, codoIzquierdo);

                            var codoManoI = poseController.CalcularAnguloCodoManoIzquierda(codoIzquierdo, newLeftHandPosition, hombroCodoI);
                            var codoManoD = poseController.CalcularAnguloCodoManoDerecha(codoDerecho, newRightHandPosition, hombroCodoD);

                            errorText.text = string.Empty;

                            errorText.text += $"Angulo hombro-codo D : {hombroCodoD.ToString()}\n";
                            errorText.text += $"Angulo hombro-codo I : {hombroCodoI.ToString()}\n";

                            errorText.text += $"Angulo codo-mano D : {codoManoD.ToString()}\n";
                            errorText.text += $"Angulo codo-mano I : {codoManoI.ToString()}\n";
                        }
                    }
                    if (!positionDetected)
                    {
                        if (CurrentState == ARState.Pausa && poseController.ResumePosition(jointTrackers, audioBit1))
                        {
                            positionDetectionTime = DateTime.Now;
                            positionDetected = true;

                            positionText.text += $"REANUDAR";
                            CurrentState = ARState.Reanudar;
                            SoundController.Instance.PlayMusic(audioBit1, 0.5f, 1);
                        }
                        if (CurrentState == ARState.Reanudar && poseController.PausePosition(jointTrackers))
                        {
                            positionDetectionTime = DateTime.Now;
                            positionDetected = true;

                            positionText.text += $"PAUSA";
                            CurrentState = ARState.Pausa;
                            SoundController.Instance.PlayMusic(audioBit2, 0.5f, 1);
                        }

                        if ((CurrentState == ARState.Reanudar || CurrentState == ARState.Pausa) &&
                            poseController.FinishPosition(jointTrackers, audioBit2))
                        {
                            positionDetectionTime = DateTime.Now;
                            positionDetected = true;

                            positionText.text += $"FINALIZAR";
                            CurrentState = ARState.Finalizar;

                            SoundController.Instance.PlayMusic(audioBit3, 0.5f, 1);
                        }
                        if (CurrentState == ARState.Finalizar && poseController.RestartPosition(jointTrackers))
                        {
                            positionDetectionTime = DateTime.Now;
                            positionDetected = true;

                            positionText.text += $"REINICIAR";
                            CurrentState = ARState.Reiniciar;
                            foreach (var obj in ARObjects)
                                Destroy(obj);
                        }
                    }
                }
            }

            foreach (var humanBody in eventArgs.removed)
            {

                errorText.text = "No fue posible capturar los movimientos";
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
            TimeCounter += ts.TotalSeconds * 100;

            //Calculate speed every TimeMax
            if (TimeCounter > TimeMax)
            {
                TimeCounter = 0;
                var speedLeftHand = Math.Round(getJointMovementSpeed(lastLeftHandPosition, newLeftHandPosition) * 10, 3);
                var speedRigthHand = Math.Round(getJointMovementSpeed(lastRightHandPosition, newRightHandPosition) * 10, 3);
                var speedLeftToes = Math.Round(getJointMovementSpeed(lastLeftToesPosition, newLeftToesPosition) * 10, 3);
                var speedRightToes = Math.Round(getJointMovementSpeed(lastRightToesPosition, newRightToesPosition) * 10, 3);
                /*
                SpeedLeft.text = string.Empty;
                SpeedLeft.text += $"Velocidad Mano Izquierda : {speedLeftHand}\n";

                SpeedLeft.text += $"Velocidad Pie Izquierdo : {speedLeftToes}\n";

                SpeedRigth.text = string.Empty;
                SpeedRigth.text += $"Velocidad Mano Derecha : {speedRigthHand}\n";

                SpeedRigth.text += $"Velocidad Pie Derecho : {speedRightToes}\n";
                */

                if (trailRendersTracked != null)
                {
                    foreach (var cube in trailRendersTracked)
                    {
                        double speed = 0;
                        Color newColor = Color.red;

                        switch (cube.transform.parent.name)
                        {
                            case "LeftHand": speed = speedLeftHand; break;
                            case "RightHand": speed = speedRigthHand; break;
                            case "LeftToesEnd": speed = speedLeftToes; break;
                            case "RightToesEnd": speed = speedRightToes; break;
                        }

                        float Cred, Cgreen, Cblue;

                        if (0 < speed && speed <= 15)
                        {
                            Cred = (float)(10 * speed);
                            Cgreen = (float)(17 * speed);
                            Cblue = (float)(255 - (10 * speed));
                        }
                        else if (15 < speed && speed <= 25.5)
                        {
                            Cred = (float)(10 * speed);
                            Cgreen = (float)(510 - (20 * speed));
                            Cblue = (float)(255 - (10 * speed));
                        }
                        else
                        {
                            Cred = 255;
                            Cgreen = 0;
                            Cblue = 0;
                        }
                        newColor = new Color(Cred / 255f, Cgreen / 255f, Cblue / 255f, 0.5f);

                        switch (cube.transform.parent.name)
                        {
                            case "LeftHand": leftHandColor = newColor; break;
                            case "RightHand": rightHandColor = newColor; break;
                            case "LeftToesEnd": leftToesColor = newColor; break;
                            case "RightToesEnd": rightToesColor = newColor; break;
                        }
                    }
                }
            }
        }
        catch (Exception e)
        {
            UnityEngine.Debug.Log($"Error al capturar los movimientos: [{e}].");
            errorText.text = "No fue posible capturar los movimientos";
        }
    }

    double getJointMovementSpeed(Vector3 lastPosition, Vector3 newPostion)
    {

        var v3distance = Vector3.Distance(lastPosition, newPostion);

        var lastx = (float)Math.Round(lastPosition.x * 10, 2);
        var lasty = (float)Math.Round(lastPosition.y * 10, 2);
        var lastz = (float)Math.Round(lastPosition.z * 10, 2);
        var newx = (float)Math.Round(newPostion.x * 10, 2); ;
        var newy = (float)Math.Round(newPostion.y * 10, 2); ;
        var newz = (float)Math.Round(newPostion.z * 10, 2); ;

        double distance = Math.Sqrt(Math.Pow(newx - lastx, 2) + Math.Pow(newy - lasty, 2) + Math.Pow(newz - lastz, 2));
        //errorText.text = $"v3Distance: {v3distance} - calcDistance: {distance}";

        return distance / TimeMax;
    }
}
