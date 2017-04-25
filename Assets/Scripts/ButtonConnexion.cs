using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ButtonConnexion : MonoBehaviour {

    public GameObject gameObjectBouton;

    public GameObject gameObjectInputField;
    public GameObject gameObjectInputFieldIp;

    public GameObject gameObjectCamera;

    Button button;
    AudioSource buttonAudioSource;

    Camera camera;
    AudioSource cameraAudioSource;

    InputField inputField;
    InputField inputFieldIp;

    

    // Use this for initialization
    void Start () {
        gameObjectBouton = GameObject.Find("Button");

        gameObjectInputField = GameObject.Find("InputField");
        gameObjectInputFieldIp = GameObject.Find("Ip");

        gameObjectCamera = GameObject.Find("Main Camera");

        button = gameObjectBouton.GetComponent<Button>();
        buttonAudioSource = gameObjectBouton.GetComponent<AudioSource>();

        camera = gameObjectCamera.GetComponent<Camera>();
        cameraAudioSource = gameObjectCamera.GetComponent<AudioSource>();

        inputField = gameObjectInputField.GetComponent<InputField>();
        inputFieldIp = gameObjectInputFieldIp.GetComponent<InputField>();

        button.onClick.AddListener(ClickOnButton);

        if(PlayerPrefs.HasKey("userName"))
        {
            if(PlayerPrefs.GetString("userName") != "")
            {
                inputField.text = PlayerPrefs.GetString("userName");
            }
        }

        if (PlayerPrefs.HasKey("serverIp"))
        {
            if (PlayerPrefs.GetString("serverIp") != "")
            {
                inputFieldIp.text = PlayerPrefs.GetString("serverIp");
            }
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    void ClickOnButton ()
    {
        if(inputField.text != "" && inputFieldIp.text != "")
        {
            PlayerPrefs.SetString("userName", inputField.text);
            PlayerPrefs.SetString("serverIp", inputFieldIp.text);
            PlayerPrefs.Save();
            SceneManager.LoadScene("JeuOnline");
        }
        else
        {
            buttonAudioSource.Stop();
            cameraAudioSource.Play();
        }
    }
}
