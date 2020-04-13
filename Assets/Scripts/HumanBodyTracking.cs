using System.Collections;
using System.Collections.Generic;
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
    private Slider curveTimeSlider;
    [SerializeField]
    private Slider minVertexDistanceSliderSlider;


    [SerializeField] private ARHumanBodyManager humanBodyManager;

    private Dictionary<TrackableId, HumanBoneController> skeletonTracker = new Dictionary<TrackableId, HumanBoneController>();

    private TrailRenderer trailRenderer;

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
    private void Awake()
    {
        curveTimeSlider.onValueChanged.AddListener(OnCurveTimeSlidersChanged);
        minVertexDistanceSliderSlider.onValueChanged.AddListener(OnMinVertexDistnaceSlidersChanged);
    }
    void OnEnable()
    {
        Debug.Assert(humanBodyManager != null, "Human body manager is required.");
        humanBodyManager.humanBodiesChanged += OnHumanBodiesChanged;
    }

    void OnDisable()
    {
        if (humanBodyManager != null)
            humanBodyManager.humanBodiesChanged -= OnHumanBodiesChanged;
    }
    void OnCurveTimeSlidersChanged(float value)
    {
        if (trailRenderer != null)
        {
            trailRenderer.time = curveTimeSlider.value;
        }
    }
    void OnMinVertexDistnaceSlidersChanged(float value)
    {
        if (trailRenderer != null)
        {
            trailRenderer.minVertexDistance = minVertexDistanceSliderSlider.value;
        }
    }
    void ApplyChangesToCurve(HumanBoneController humanBoneController)
    {
        if(curveTimeSlider!=null && minVertexDistanceSliderSlider != null)
        {
            trailRenderer = humanBoneController.GetComponentInChildren<TrailRenderer>();
            trailRenderer.minVertexDistance = minVertexDistanceSliderSlider.value;
            trailRenderer.time = curveTimeSlider.value;
        }
        else
        {
            Debug.LogError("Curve Time Slider and Min Vertex Distance Slider need to be set");
        }
    }

    void OnHumanBodiesChanged(ARHumanBodiesChangedEventArgs eventArgs)
    {
        HumanBoneController humanBoneController;

        foreach (var humanBody in eventArgs.added)
        {
            if (!skeletonTracker.TryGetValue(humanBody.trackableId, out humanBoneController))
            {
                Debug.Log($"Adding a new skeleton [{humanBody.trackableId}].");
                var newSkeletonGO = Instantiate(skeletonPrefab, humanBody.transform);

                //apply trall render options
                ApplyChangesToCurve(humanBoneController);

                humanBoneController = newSkeletonGO.GetComponent<HumanBoneController>();

                // add an offset just when the human body is added
                humanBoneController.transform.position = humanBoneController.transform.position +
                    new Vector3(skeletonOffsetX, skeletonOffsetY, skeletonOffsetZ);

                skeletonTracker.Add(humanBody.trackableId, humanBoneController);
            }

            humanBoneController.InitializeSkeletonJoints();
            humanBoneController.ApplyBodyPose(humanBody, Vector3.zero);

            HumanBodyTrackerUI.Instance.humanBodyText.text = $"{this.gameObject.name} {humanBody.name} Position: {humanBody.transform.position}\n" +
            $"LocalPosition: {humanBody.transform.localPosition}";

            HumanBodyTrackerUI.Instance.humanBoneControllerText.text = $"{this.gameObject.name} {humanBoneController.name} Position: {humanBoneController.transform.position}\n" +
            $"LocalPosition: {humanBoneController.transform.localPosition}";
        }

        foreach (var humanBody in eventArgs.updated)
        {
            if (skeletonTracker.TryGetValue(humanBody.trackableId, out humanBoneController))
            {
                humanBoneController.ApplyBodyPose(humanBody, Vector3.zero);
            }
            //apply trall render options
            ApplyChangesToCurve(humanBoneController);

            HumanBodyTrackerUI.Instance.humanBodyText.text = $"{this.gameObject.name} {humanBody.name} Position: {humanBody.transform.position}\n" +
            $"LocalPosition: {humanBody.transform.localPosition}";

            HumanBodyTrackerUI.Instance.humanBoneControllerText.text = $"{this.gameObject.name} {humanBoneController.name} Position: {humanBoneController.transform.position}\n" +
            $"LocalPosition: {humanBoneController.transform.localPosition}";
        }

        foreach (var humanBody in eventArgs.removed)
        {
            Debug.Log($"Removing a skeleton [{humanBody.trackableId}].");
            if (skeletonTracker.TryGetValue(humanBody.trackableId, out humanBoneController))
            {
                Destroy(humanBoneController.gameObject);
                skeletonTracker.Remove(humanBody.trackableId);
            }
        }

        HumanBodyTrackerUI.Instance.humanBodyTrackerText.text = $"{this.gameObject.name} Position: {this.gameObject.transform.position}\n" +
            $"LocalPosition: {this.gameObject.transform.localPosition}";
    }
}
