using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Tutorial : MonoBehaviour
{
    [SerializeField]
    private GameObject resumeButton;
    [SerializeField]
    private GameObject pauseButton;
    [SerializeField]
    private GameObject finishButton;
    [SerializeField]
    private GameObject deleteButton;

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        resumeButton = GameObject.Find("ResumeButton");
        pauseButton = GameObject.Find("PauseButton");
        finishButton = GameObject.Find("FinishButton");
        deleteButton = GameObject.Find("DeleteButton");

        resumeButton.SetActive(true);
        pauseButton.SetActive(false);
        finishButton.SetActive(false);
        deleteButton.SetActive(false);

    }


    public void ShowPauseButton()
    {
        resumeButton.SetActive(false);
        pauseButton.SetActive(true);
    }

    public void ShowFinishButton()
    {
        pauseButton.SetActive(false);
        finishButton.SetActive(true);
    }

    public void ShowDeleteButton()
    {
        finishButton.SetActive(false);
        deleteButton.SetActive(true);
    }
    public void StartBodyTracking()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
