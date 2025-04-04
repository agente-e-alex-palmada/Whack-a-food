using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEditor;

public class GameManager : MonoBehaviour
{
    public bool isGameActive;
    public List<GameObject> targets;
    private float spawnRate = 1.0f;
    private int score = 0;
    private int lives = 0;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI gameOverText;
    public TextMeshProUGUI healthText;
    public Button restartButton;
    public GameObject titleScreen;
    public bool isPaused = false;
    public GameObject pauseMenu;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                Resume();
                isPaused = false;
            }
            else { 
                Pause();
                isPaused=true;
            }
        }
    }

    public void Pause()
    {
        pauseMenu.SetActive(true);

        // The time of the game will be 0, which means nothing will happend during paused screen
        Time.timeScale = 0f;
    }

    public void Resume()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
    }

    public void StartGame(int difficulty)
    {
        spawnRate /= difficulty;
        StartCoroutine(SpawnTarget());
        UpdateScore(0);
        UpdateHealth(3);
        isGameActive = true;
        titleScreen.gameObject.SetActive(false);
        scoreText.gameObject.SetActive(true);
        healthText.gameObject.SetActive(true);
    }
    
    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void UpdateHealth(int healthToAdd)
    {
        lives += healthToAdd;
        if (lives <= 0)
        {
            lives = 0;
            GameOver();
        }
        healthText.text = "HP: "+ lives;
    }

    public void GameOver()
    {
        Debug.Log("Game Over");
        isGameActive = false;
        gameOverText.gameObject.SetActive(true);
        restartButton.gameObject.SetActive(true);
    }

    public void UpdateScore(int scoreToAdd)
    {
        score += scoreToAdd;
        if (score < 0)
        {
            score = 0;
        }
        scoreText.text = "Score: " + score;
    }
    IEnumerator SpawnTarget()
    {
        while (isGameActive)
        {
            yield return new WaitForSeconds(spawnRate);
            int index = Random.Range(0, targets.Count);
            Instantiate(targets[index]);
        }
    }
}
