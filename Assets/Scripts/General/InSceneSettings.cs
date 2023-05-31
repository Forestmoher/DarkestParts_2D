using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.InputSystem;


public class InSceneSettings : MonoBehaviour
{
    //[SerializeField] GameObject settingPanel;
    //[SerializeField] GameManager gameManager;
    //[SerializeField] Toggle fullscreenToggle;
    //bool SettingsOpen = false;

    //readonly string fullscreenKey = "fullscreen";
    

    //private void Start() {
    //    if(PlayerPrefs.GetInt(fullscreenKey, 1) == 1) {
    //        fullscreenToggle.isOn = true;
    //    } else {
    //        fullscreenToggle.isOn = false;
    //    }
    //    ToggleFullscreen();
    //}

    //public void ToggleSettingsPanel() {
    //    if (gameManager.gameEnded) return;
    //    gameManager.audioManager.PlaySound("Switch_Click");
    //    //if (levelManager.respawning || levelManager.dialogueManager.inConversation || levelManager.gameEnd) return;
    //    if (!SettingsOpen) {
    //        gameManager.inGame = false;
    //        SettingsOpen = true;
    //        settingPanel.SetActive(true);
    //        Time.timeScale = 0;
    //    } else {
    //        if(gameManager.gameStarted) gameManager.inGame = true;
    //        SettingsOpen = false;
    //        settingPanel.SetActive(false);
    //        Time.timeScale = 1f;
    //    }
    //}

    //private void Update() {
    //    if (Keyboard.current.escapeKey.wasPressedThisFrame) {
    //        ToggleSettingsPanel();
    //    }
    //    //if (Input.GetKeyDown(KeyCode.R)) {
    //    //    RestartScene();
    //    //}
    //}

    //public async void RestartScene() {
    //    gameManager.audioManager.PlaySound("Switch_Click");
    //    gameManager.audioManager.startRightAway = true;
    //    //if (levelManager.respawning || levelManager.gameEnd) return;
    //    //levelManager.respawning = true;
    //    Time.timeScale = 1f;
    //    //levelManager.circleTransition.CloseBlackScreen();
    //    //levelManager.progressManager.firstTimeAtMenu = false;
    //    //await Task.Delay(1000);
    //    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    //}

    //public async void ReturnToMenu() {
    //    gameManager.audioManager.PlaySound("Switch_Click");
    //    //if (levelManager.respawning || levelManager.gameEnd) return;
    //    //levelManager.respawning = true;
    //    Time.timeScale = 1f;
    //    //levelManager.circleTransition.CloseBlackScreen();
    //    //await Task.Delay(1000);
    //    //SceneManager.LoadScene(0);
    //    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    //} 

    //public void ToggleFullscreen() {
    //    gameManager.audioManager.PlaySound("Switch_Click");
    //    if (fullscreenToggle.isOn) {
    //        Screen.fullScreen = true;
    //        PlayerPrefs.SetInt(fullscreenKey, 1);
    //    } else {
    //        Screen.fullScreen = false;
    //        PlayerPrefs.SetInt(fullscreenKey, 0);
    //    }
    //}

    //public void QuitGame() {
    //    Application.Quit();
    //}
}
