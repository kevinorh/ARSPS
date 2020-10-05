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
    private GameObject objeto1;
    [SerializeField]
    private GameObject objeto2;
    [SerializeField]
    private GameObject objeto3;
    [SerializeField]
    private GameObject objeto4;
    [SerializeField]
    private GameObject objeto5;
    [SerializeField]
    private GameObject objeto6;
    [SerializeField]
    private GameObject objeto7;
    [SerializeField]
    private GameObject objeto8;

    [SerializeField]
    Sound[] sounds;


    [SerializeField] private ARHumanBodyManager humanBodyManager;

    private Dictionary<TrackableId, HumanBoneController> skeletonTracker = new Dictionary<TrackableId, HumanBoneController>();

    private JointTracker[] jointTrackers;

    private PoseController poseController = new PoseController();

    private double TimeCounter = 0;
    private double TimeMax = 0.5;

    DateTime positionDetectionTime;
    bool positionDetected = false;


    //last jointTrackers positions
    Vector3 lastLeftHandPosition;
    Vector3 lastRightHandPosition;
    //Instantiated AR
    List<GameObject> ARObjects = new List<GameObject>();

    //new jointTrackers positions
    Vector3 newLeftHandPosition;
    Vector3 newRightHandPosition;

    Vector3 rightForearm;
    Vector3 leftForearm;
    Vector3 rightShoulder1;
    Vector3 leftShoulder1;


    //Colors for tracked joints
    Color leftHandColor = Color.red;
    Color rightHandColor = Color.red;

    //AR States
    ARState figuresGeneration = ARState.Detener;
    //Posiciones
    Posicion currentPosition = Posicion.Ninguna;

    int maxObjectsInScreen = 3000;

    int eliminados = 0;

    private SoundManager soundManager;
    string currentSound;

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

    public GameObject ARObjetToBeCreated
    {
        get { return objeto1; }
        set { objeto1 = value; }
    }

    void OnEnable()
    {
        soundManager = SoundManager.instace;
        if (soundManager == null)
            Debug.LogError("SoundManager not found in the scene");

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
            //Keep position detected message for 1 sec
            if (positionDetected && positionDetectionTime.AddSeconds(1) < DateTime.Now)
            {
                positionText.text = string.Empty;
                positionDetected = false;
            }
            if (positionDetected && !currentPosition.Equals(Posicion.Ninguna))
                positionText.text = $"{currentPosition}";

            HumanBoneController humanBoneController;
            //Add detected joints
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
                        newLeftHandPosition = new Vector3(0, 0, 0);
                        newRightHandPosition = new Vector3(0, 0, 0);
                        rightForearm = new Vector3(0, 0, 0);
                        rightShoulder1 = new Vector3(0, 0, 0);
                        leftForearm = new Vector3(0, 0, 0);
                        leftShoulder1 = new Vector3(0, 0, 0);

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
            //update detected joints position
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
                        //Instance ARObjects for RightHand
                        if(figuresGeneration.Equals(ARState.Continuar))
                        {
                            if (jointTracker.gameObject.transform.parent.name.Equals("RightHand"))
                            {
                                var nuevaEsfera = Instantiate(ARObjetToBeCreated, jointTracker.gameObject.transform.position, Quaternion.identity);
                                nuevaEsfera.transform.GetComponent<Renderer>().material.color = rightHandColor;
                                ARObjects.Add(nuevaEsfera);

                                UnityEngine.Debug.Log($"#ARObjects: [{ARObjects.Count()}].");
                                UnityEngine.Debug.Log($"#eliminados: [{eliminados}].");
                                //Destroy first ARObjects to keep maxObjectsInScreen
                                if (ARObjects.Count() - eliminados >= maxObjectsInScreen)
                                {
                                    UnityEngine.Debug.Log($"CUMPLE CONDICIÓN.");
                                    DestroyARObject(eliminados);
                                }
                            }
                            //Instance ARObjects for LeftHand
                            if (jointTracker.gameObject.transform.parent.name.Equals("LeftHand"))
                            {
                                var nuevaEsfera = Instantiate(ARObjetToBeCreated, jointTracker.gameObject.transform.position, Quaternion.identity);
                                nuevaEsfera.transform.GetComponent<Renderer>().material.color = leftHandColor;
                                ARObjects.Add(nuevaEsfera);

                                UnityEngine.Debug.Log($"#ARObjects: [{ARObjects.Count()}].");
                                UnityEngine.Debug.Log($"#eliminados: [{eliminados}].");
                                //Destroy first ARObjects to keep maxObjectsInScreen
                                if (ARObjects.Count() - eliminados >= maxObjectsInScreen)
                                {
                                    UnityEngine.Debug.Log($"CUMPLE CONDICIÓN.");
                                    DestroyARObject(eliminados);
                                }
                            }
                            UnityEngine.Debug.Log($"ARObjects [{ARObjects.Count()}].");
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
                            if (jointTracker.gameObject.transform.parent.name.Equals("RightForearm"))
                                rightForearm = jointTracker.gameObject.transform.position;

                            if (jointTracker.gameObject.transform.parent.name.Equals("LeftForearm"))
                                leftForearm = jointTracker.gameObject.transform.position;

                            if (jointTracker.gameObject.transform.parent.name.Equals("RightShoulder1"))
                                rightShoulder1 = jointTracker.gameObject.transform.position;

                            if (jointTracker.gameObject.transform.parent.name.Equals("LeftShoulder1"))
                                leftShoulder1 = jointTracker.gameObject.transform.position;
                           /*
                           SpeedRigth.text = string.Empty;
                           SpeedRigth.text += $"Hombro Derecho : ({rightShoulder1.x},{rightShoulder1.y},{rightShoulder1.z})\n";
                           SpeedRigth.text += $"Codo Derecho : ({rightForearm.x},{rightForearm.y},{rightForearm.z})\n";
                           SpeedRigth.text += $"Mano Derecho : ({newRightHandPosition.x},{newRightHandPosition.y},{newRightHandPosition.z})\n";
                           SpeedRigth.text += $"RF 1 : ({rightForearm.x},{rightShoulder1.y},{rightForearm.z})\n";
                           SpeedRigth.text += $"RF 2 : ({rightForearm.x},{newRightHandPosition.y},{rightForearm.z})\n";

                           SpeedLeft.text = string.Empty;
                           SpeedLeft.text += $"Hombro Izquierdo : ({leftShoulder1.x},{leftShoulder1.y},{leftShoulder1.z})\n";
                           SpeedLeft.text += $"Codo Izquierdo : ({leftForearm.x},{leftForearm.y},{leftForearm.z})\n";
                           SpeedLeft.text += $"Mano Izquierdo : ({newLeftHandPosition.x},{newLeftHandPosition.y},{newLeftHandPosition.z})\n";
                           SpeedLeft.text += $"RF 1 : ({leftForearm.x},{leftShoulder1.y},{leftForearm.z})\n";
                           SpeedLeft.text += $"RF 2 : ({leftForearm.x},{newLeftHandPosition.y},{leftForearm.z})\n";


                           errorText.text = string.Empty;

                           double hombroCodoD = poseController.CalcularAnguloHombroCodoDerecha(rightShoulder1, rightForearm);
                           errorText.text += $"Angulo hombro-codo D : {hombroCodoD}\n";
                           double codoManoD = poseController.CalcularAnguloCodoManoDerecha(rightForearm, newRightHandPosition, hombroCodoD);
                           errorText.text += $"Angulo codo-mano D : {codoManoD}\n\n";

                           double hombroCodoI = poseController.CalcularAnguloHombroCodoIzquierda(leftShoulder1, leftForearm);
                           errorText.text += $"Angulo hombro-codo I : {hombroCodoI}\n";
                           double codoManoI = poseController.CalcularAnguloCodoManoIzquierda(leftForearm, newLeftHandPosition, hombroCodoI);
                           errorText.text += $"Angulo codo-mano I : {codoManoI}\n";
                           */
                            currentPosition = poseController.IdentificarPosicion(rightShoulder1, rightForearm, newRightHandPosition, leftShoulder1, leftForearm, newLeftHandPosition);                            
                        }
                    }
                    //Cannot detect new position after 1 sec if a position different to Ninguna has been detected
                    if (!positionDetected)
                    {
                        /*
                        if (!currentPosition.Equals(Posicion.Ninguna))
                            positionText.text = $"{currentPosition}";
                        */
                        if (currentPosition == Posicion.Sonido1 &&
                            currentSound != currentPosition.ToString())
                        {
                            UnityEngine.Debug.Log($"Posicion Identificada {Posicion.Sonido1}");
                            positionDetectionTime = DateTime.Now;
                            positionDetected = true;

                            ARObjetToBeCreated = objeto1;
                            figuresGeneration = ARState.Continuar;
                            currentSound = currentPosition.ToString();

                            soundManager.PlaySound(Posicion.Sonido1.ToString());
                        }

                        else if (currentPosition == Posicion.Sonido2 &&
                            currentSound != currentPosition.ToString())
                        {
                            UnityEngine.Debug.Log($"Posicion Identificada {Posicion.Sonido2}");
                            positionDetectionTime = DateTime.Now;
                            positionDetected = true;

                            ARObjetToBeCreated = objeto2;
                            figuresGeneration = ARState.Continuar;
                            currentSound = currentPosition.ToString();
                            soundManager.PlaySound(Posicion.Sonido2.ToString());
                        }

                        else if (currentPosition == Posicion.Sonido3 &&
                            currentSound != currentPosition.ToString())
                        {
                            UnityEngine.Debug.Log($"Posicion Identificada {Posicion.Sonido3}");
                            positionDetectionTime = DateTime.Now;
                            positionDetected = true;

                            ARObjetToBeCreated = objeto3;
                            figuresGeneration = ARState.Continuar;
                            currentSound = currentPosition.ToString();
                            soundManager.PlaySound(Posicion.Sonido3.ToString());

                        }

                        else if (currentPosition == Posicion.Sonido4 &&
                            currentSound != currentPosition.ToString())
                        {
                            UnityEngine.Debug.Log($"Posicion Identificada {Posicion.Sonido4}");
                            positionDetectionTime = DateTime.Now;
                            positionDetected = true;

                            ARObjetToBeCreated = objeto4;
                            figuresGeneration = ARState.Continuar;
                            currentSound = currentPosition.ToString();
                            soundManager.PlaySound(Posicion.Sonido4.ToString());
                        }

                        else if (currentPosition == Posicion.Sonido5 &&
                            currentSound != currentPosition.ToString())
                        {
                            UnityEngine.Debug.Log($"Posicion Identificada {Posicion.Sonido5}");

                            positionDetectionTime = DateTime.Now;
                            positionDetected = true;

                            ARObjetToBeCreated = objeto5;
                            figuresGeneration = ARState.Continuar;
                            currentSound = currentPosition.ToString();
                            soundManager.PlaySound(Posicion.Sonido5.ToString());
                        }

                        else if (currentPosition == Posicion.Sonido6 &&
                            currentSound != currentPosition.ToString())
                        {
                            UnityEngine.Debug.Log($"Posicion Identificada {Posicion.Sonido6}");
                            positionDetectionTime = DateTime.Now;
                            positionDetected = true;

                            ARObjetToBeCreated = objeto6;
                            figuresGeneration = ARState.Continuar;
                            currentSound = currentPosition.ToString();

                            soundManager.PlaySound(Posicion.Sonido6.ToString());
                        }

                        else if (currentPosition == Posicion.Sonido7 &&
                            currentSound != currentPosition.ToString())
                        {
                            UnityEngine.Debug.Log($"Posicion Identificada {Posicion.Sonido7}");
                            positionDetectionTime = DateTime.Now;
                            positionDetected = true;

                            ARObjetToBeCreated = objeto7;
                            figuresGeneration = ARState.Continuar;
                            currentSound = currentPosition.ToString();
                            soundManager.PlaySound(Posicion.Sonido7.ToString());
                        }

                        else if (currentPosition == Posicion.Sonido8 &&
                            currentSound != currentPosition.ToString())
                        {
                            UnityEngine.Debug.Log($"Posicion Identificada {Posicion.Sonido8}");
                            positionDetectionTime = DateTime.Now;
                            positionDetected = true;

                            ARObjetToBeCreated = objeto8;
                            figuresGeneration = ARState.Continuar;
                            currentSound = currentPosition.ToString();
                            soundManager.PlaySound(Posicion.Sonido8.ToString());
                        }

                        else if (currentPosition == Posicion.SubirVolumen)
                        {
                            positionDetectionTime = DateTime.Now;
                            positionDetected = true;

                            soundManager.IncreaseVolume(currentSound);
                        }

                        else if (currentPosition == Posicion.BajarVolumen)
                        {
                            positionDetectionTime = DateTime.Now;
                            positionDetected = true;

                            soundManager.DecreaseVolume(currentSound);
                        }

                        else if (currentPosition == Posicion.Acelerar)
                        {
                            positionDetectionTime = DateTime.Now;
                            positionDetected = true;

                            soundManager.IncreasePitch(currentSound);
                        }

                        else if (currentPosition == Posicion.Ralentizar)
                        {
                            positionDetectionTime = DateTime.Now;
                            positionDetected = true;
                            soundManager.DecreasePitch(currentSound);
                        }

                        else if (currentPosition == Posicion.Finalizar)
                        {
                            positionDetectionTime = DateTime.Now;
                            positionDetected = true;
                            figuresGeneration = ARState.Detener;
                        }

                        else if (currentPosition == Posicion.Reiniciar)
                        {
                            positionDetectionTime = DateTime.Now;
                            positionDetected = true;

                            figuresGeneration = ARState.Detener;

                            foreach (var obj in ARObjects)
                                Destroy(obj);

                            soundManager.StopSounds();
                        }

                        UnityEngine.Debug.Log($"CurrentSound: {currentSound}");
                    }
                }

            }
            //Remove joints not detected
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

                if (jointTrackers != null)
                {
                    //Define color for left and right hand AR Figures
                    foreach (var cube in jointTrackers)
                    {
                        double speed = 0;
                        Color newColor = Color.red;

                        switch (cube.transform.parent.name)
                        {
                            case "LeftHand": speed = speedLeftHand; break;
                            case "RightHand": speed = speedRigthHand; break;
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
                        }
                    }
                }
            }
        }
        catch (Exception e)
        {
            UnityEngine.Debug.LogError($"Error al capturar los movimientos: [{e}].");
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

        UnityEngine.Debug.Log($"Distancia tipo1: [{distance}].");
        UnityEngine.Debug.Log($"Distancia tipo1: [{v3distance}].");

        return distance / TimeMax;
    }
    void DestroyARObject(int indexToBeDestroyed) {
        UnityEngine.Debug.Log($"To Be Destroyed {indexToBeDestroyed}");
        if (ARObjects[indexToBeDestroyed] != null)
        {
            UnityEngine.Debug.Log($"Destroy ARObjects[{indexToBeDestroyed}]");
            Destroy(ARObjects[indexToBeDestroyed]);
            eliminados++;
        }
    }

}
