﻿using UnityEngine;
using System.Collections.Generic;

public class ToggleIngameMenus : MonoBehaviour
{
    private static ToggleIngameMenus instance;

    [SerializeField]
    private GameObject pauseMenu;
    [SerializeField]
    private GameObject optionsMenu;
    [SerializeField]
    private GameObject gameOverMenu;

    //Liste von AudioSources, die auf "mute" gestellt werden sollen, wenn ein Menü aktiv ist
    private static List<AudioSource> audioToDeactivateInMenus = new List<AudioSource>();

    //wird noch früher als Awake aufgerufen, aber nur beim Levelwechsel
    void OnLevelWasLoaded()
    {
        for(var i = 0; i < audioToDeactivateInMenus.Count; ++i)
        {
            if(audioToDeactivateInMenus[i] == null)
            {
                audioToDeactivateInMenus.RemoveAt(i);
                --i;
            }
        }
    }

    void Awake()
    {
        instance = this;

        //Zu Anfang des Spiels deaktiviert sich die Menüs
        //Das GameObject kann auch im Editor schon deaktiviert sein, aber falls es beim Editieren
        //aktiviert bleibt...
        pauseMenu.SetActive(false);
        optionsMenu.SetActive(false);
        gameOverMenu.SetActive(false);
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePauseMenu();
        }
    }

    public void TogglePauseMenu()
    {
        if(gameOverMenu.activeSelf) return; //Wenn Game Over aktiv ist, nicht das Pause-Menü öffnen

        optionsMenu.SetActive(false);

        //GameObject an schalten, wenn aus, und umgekehrt
        pauseMenu.SetActive(!pauseMenu.activeSelf);
        //Pausiert die Welt wenn der Canvas aktiv wird
        Time.timeScale = pauseMenu.activeSelf ? 0 : 1;
        //Ton bestimmer Soundquellen an oder aus schalten
        SetAudioSourcesActive(!pauseMenu.activeSelf);
    }

    public void OpenOptionsMenu()
    {
        optionsMenu.SetActive(true);
        pauseMenu.SetActive(false);
    }

    public static void GameOver()
    {
        Time.timeScale = 0;
        instance.gameOverMenu.SetActive(true);
        SetAudioSourcesActive(false);
    }

    public static void AddAudioSourceForDeactivation(AudioSource audio)
    {
        audioToDeactivateInMenus.Add(audio);
    }

    private static void SetAudioSourcesActive(bool on)
    {
        foreach(var audio in audioToDeactivateInMenus)
        {
            audio.mute = !on;
        }
    }

    public static bool HasOpenMenus()
    {
        return instance.pauseMenu.activeSelf
            || instance.optionsMenu.activeSelf
            || instance.gameOverMenu.activeSelf;
    }
}
