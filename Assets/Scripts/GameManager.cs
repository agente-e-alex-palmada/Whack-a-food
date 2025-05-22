using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEditor;
using Unity.VisualScripting;
using UnityEngine.SocialPlatforms.Impl;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine.Networking;
using System;
using System.Security.Cryptography;

public class GameManager : MonoBehaviour
{
    public bool isGameActive;
    public List<GameObject> targets;
    private float spawnRate = 1.0f;
    private int score = 0;
    private int lives = 0;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI healthText;
    public Button restartButton;
    public GameObject titleScreen;
    public GameObject gameOverScreen;
    public bool isPaused = false;
    public GameObject pauseMenu;
    public GameObject inputField;
    public GameObject nameText;
    public TextMeshProUGUI initialRanking;
    public TextMeshProUGUI finalRanking;
    private string nameScore;


    private List<string> namesInRanking = new List<string>();
    private List<int> scoresInRanking = new List<int>();

    // Datbaase interaction values
    private string secretKey = "mySecretKey";
    public string addScoreURL = "http://localhost/addscore.php?";
    public string highscoreURL = "http://localhost/display.php";

    public IEnumerator GetScores(System.Action<List<string>> onCompleted)
    {
        UnityWebRequest hs_get = UnityWebRequest.Get(highscoreURL);
        yield return hs_get.SendWebRequest();

        if (hs_get.result != UnityWebRequest.Result.Success)
        {
            Debug.Log("There was an error getting the high score: " + hs_get.error);
            onCompleted?.Invoke(null);  // return null if failed
        }
        else
        {
            string dataText = hs_get.downloadHandler.text;
            string[] lines = dataText.Split(new[] { '\n' }, System.StringSplitOptions.RemoveEmptyEntries);

            List<string> entries = new List<string>();

            foreach (string line in lines)
            {
                if (!string.IsNullOrWhiteSpace(line) && line.Contains("_"))
                {
                    entries.Add(line.Trim());
                }
                else
                {
                    Debug.LogWarning("Invalid entry format: " + line);
                }
            }

            onCompleted?.Invoke(entries);
        }
    }

    IEnumerator PostScores(string name, int score)
    {
        string hash = HashInput(name + score + secretKey);
        string post_url = addScoreURL + "name=" + name+ "&score=" + score + "&hash=" + hash;
        Debug.Log(post_url);
        UnityWebRequest hs_post = UnityWebRequest.Post(post_url, hash);
        yield return hs_post.SendWebRequest();
        if (hs_post.error != null) Debug.Log("There was an error posting the high score: " + hs_post.error);
    }

    public string HashInput(string input)
    {
        SHA256Managed hm = new SHA256Managed();
        byte[] hashValue = hm.ComputeHash(System.Text.Encoding.ASCII.GetBytes(input));
        string hash_convert = BitConverter.ToString(hashValue).Replace("-", "").ToLower();
        return hash_convert;
    }

    void Update()
    {
        if (!titleScreen.activeSelf && isGameActive) { 
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
    }

    // Nothing will move
    public void Pause()
    {
        pauseMenu.SetActive(true);

        // The time of the game will be 0, which means nothing will happend during paused screen
        Time.timeScale = 0f;
    }


    // Unpauses the game
    public void Resume()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
    }

    // Starts the game with default values
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
    
    // Reloads the scene
    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // Updates the health value and shows game over sceern if death
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

    // Shows the game over screen
    public void GameOver()
    {
        Debug.Log("Game Over");
        isGameActive = false;
        isPaused = true;
        pauseMenu.SetActive(true);
        gameOverScreen.SetActive(true);
    }
    // Updates the score with a new value
    public void UpdateScore(int scoreToAdd)
    {
        score += scoreToAdd;
        if (score < 0)
        {
            score = 0;
        }
        scoreText.text = "Score: " + score;
    }

    // Spawns a target on screen
    IEnumerator SpawnTarget()
    {
        while (isGameActive)
        {
            yield return new WaitForSeconds(spawnRate);
            int index = UnityEngine.Random.Range(0, targets.Count);
            Instantiate(targets[index]);
        }
    }

    // Gets the name of the user that writed in the input field
    public void ReadInput()
    {
        nameScore = nameText.GetComponent<TextMeshProUGUI>().text;
        Debug.Log("Text has been saved as: \"" + nameScore + "\" with a score of " + score);
        inputField.SetActive(false);
        StartCoroutine(PostScores(nameScore, score));
    }
}

/* To get the data and return

                StartCoroutine(GetScores((List<string> entries) =>
        {
            if (entries != null)
            {
                namesInRanking.Clear();
                scoresInRanking.Clear();

                foreach (string entry in entries)
                {
                    string[] parts = entry.Split('_');
                    if (parts.Length == 2)
                    {
                        namesInRanking.Add(parts[0]);

                        if (int.TryParse(parts[1], out int parsedScore))
                        {
                            scoresInRanking.Add(parsedScore);
                        }
                        else
                        {
                            Debug.LogWarning("Invalid score format: " + parts[1]);
                            scoresInRanking.Add(0);
                        }

                    }
                    else
                    {
                        Debug.LogWarning("Invalid entry: " + entry);
                    }
                }
                for (int i = 0; i < scoresInRanking.Count(); i++)
                {
                    Debug.Log(scoresInRanking[i]);
                }
            }
            else
            {
                Debug.LogWarning("Failed to get scores from server.");
            }
        })); 
*/