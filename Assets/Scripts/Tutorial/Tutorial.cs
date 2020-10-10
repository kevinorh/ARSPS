using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Tutorial : MonoBehaviour
{
    [SerializeField]
    private GameObject flujo1Button;
    [SerializeField]
    private GameObject sonido1Button;
    [SerializeField]
    private GameObject sonido2Button;
    [SerializeField]
    private GameObject sonido3Button;
    [SerializeField]
    private GameObject sonido4Button;
    [SerializeField]
    private GameObject sonido5Button;
    [SerializeField]
    private GameObject sonido6Button;
    [SerializeField]
    private GameObject sonido7Button;
    [SerializeField]
    private GameObject sonido8Button;
    [SerializeField]
    private GameObject flujo2Button;
    [SerializeField]
    private GameObject subirVolButton;
    [SerializeField]
    private GameObject bajarVolButton;
    [SerializeField]
    private GameObject subirVelButton;
    [SerializeField]
    private GameObject bajarVelButton;
    [SerializeField]
    private GameObject flujo3Button;
    [SerializeField]
    private GameObject finalizarButton;
    [SerializeField]
    private GameObject reiniciarButton;
    [SerializeField]
    private GameObject coloresButton;

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        flujo1Button = GameObject.Find("Flujo1");
        sonido1Button = GameObject.Find("Sonido1");
        sonido2Button = GameObject.Find("Sonido2");
        sonido3Button = GameObject.Find("Sonido3");
        sonido4Button = GameObject.Find("Sonido4");       
        sonido5Button = GameObject.Find("Sonido5");
        sonido6Button = GameObject.Find("Sonido6");
        sonido7Button = GameObject.Find("Sonido7");
        sonido8Button = GameObject.Find("Sonido8");
        flujo2Button = GameObject.Find("Flujo2");
        subirVolButton= GameObject.Find("SubirVolumen");
        bajarVolButton= GameObject.Find("BajarVolumen");
        subirVelButton= GameObject.Find("SubirVelocidad");
        bajarVelButton = GameObject.Find("BajarVelocidad");
        flujo3Button = GameObject.Find("Flujo3");
        finalizarButton= GameObject.Find("Finalizar");
        reiniciarButton = GameObject.Find("Reiniciar");
        coloresButton = GameObject.Find("Colores");

        flujo1Button.SetActive(true);
        sonido1Button.SetActive(false);
        sonido2Button.SetActive(false);
        sonido3Button.SetActive(false);
        sonido4Button.SetActive(false);
        sonido5Button.SetActive(false);
        sonido6Button.SetActive(false);
        sonido7Button.SetActive(false);
        sonido8Button.SetActive(false);
        flujo2Button.SetActive(false);
        subirVolButton.SetActive(false);
        bajarVolButton.SetActive(false);
        subirVelButton.SetActive(false);
        bajarVelButton.SetActive(false);
        flujo3Button.SetActive(false);
        finalizarButton.SetActive(false);
        reiniciarButton.SetActive(false);
        coloresButton.SetActive(false);
    }

    public void Click1()
    {
        flujo1Button.SetActive(false);
        sonido1Button.SetActive(true);
    }

    public void Click2()
    {
        sonido1Button.SetActive(false);
        sonido2Button.SetActive(true);
    }
    public void Click3()
    {
        sonido2Button.SetActive(false);
        sonido3Button.SetActive(true);
    }
    public void Click4()
    {
        sonido3Button.SetActive(false);
        sonido4Button.SetActive(true);
    }
    public void Click5()
    {
        sonido4Button.SetActive(false);
        sonido5Button.SetActive(true);
    }
    public void Click6()
    {
        sonido5Button.SetActive(false);
        sonido6Button.SetActive(true);
    }
    public void Click7()
    {
        sonido6Button.SetActive(false);
        sonido7Button.SetActive(true);
    }
    public void Click8()
    {
        sonido7Button.SetActive(false);
        sonido8Button.SetActive(true);
    }
    public void Click9()
    {
        sonido8Button.SetActive(false);
        flujo2Button.SetActive(true);
    }
    public void Click10()
    {
        flujo2Button.SetActive(false);
        subirVolButton.SetActive(true);
    }
    public void Click11()
    {
        subirVolButton.SetActive(false);
        bajarVolButton.SetActive(true);
    }
    public void Click12()
    {
        bajarVolButton.SetActive(false);
        subirVelButton.SetActive(true);
    }
    public void Click13()
    {
        subirVelButton.SetActive(false);
        bajarVelButton.SetActive(true);
    }
    public void Click14()
    {
        bajarVelButton.SetActive(false);
        flujo3Button.SetActive(true);
    }
    public void Click15()
    {
        flujo3Button.SetActive(false);
        finalizarButton.SetActive(true);
    }
    public void Click16()
    {
        finalizarButton.SetActive(false);
        reiniciarButton.SetActive(true);
    }
    public void Click17()
    {
        reiniciarButton.SetActive(false);
        coloresButton.SetActive(true);
    }
    public void StartBodyTracking()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
