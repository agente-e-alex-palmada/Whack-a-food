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
    private InteractWithData leaderboard;

    private void Start()
    {
        // Initialize the leaderboard
        leaderboard = new InteractWithData();

        // Load the entries from the leaderboard
        List<InteractWithData.Data> entries = leaderboard.LoadEntries();

        // Sort the entries by score in descending order using LINQ
        entries = entries.OrderByDescending(entry => entry.score).ToList();

        // Initialize an empty string to hold the ranking display
        string rankingText = "Ranking:\n"; // Start with a header

        // Iterate through the entries to build the ranking string
        for (int i = 0; i < entries.Count; i++)
        {
            if (i == 12)
            {
                break;
            }
            // Truncate the name to fit within 8 characters
            string name = entries[i].name.Length > 8 ? entries[i].name.Substring(0, 8) : entries[i].name;

            // Format the score as a string, making sure it fits within 5 characters
            string score = entries[i].score.ToString();
            if (score.Length > 5)
            {
                score = score.Substring(0, 5); // Truncate if score exceeds 5 characters (for very large numbers)
            }

            // Format the entry with a max total length of 13 characters (name + score)
            string formattedEntry = (i + 1) + ". " + name + " - " + score;

            // Ensure the total length doesn't exceed 13 characters
            formattedEntry = formattedEntry.PadRight(13); // Pad the entry to make sure it is exactly 13 characters long

            // Add each entry to the rankingText
            rankingText += formattedEntry + "\n";
        }

        // Set the built ranking text to the TextMeshProUGUI component
        initialRanking.text = rankingText;
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


    // Nothing will mov
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
        // Initialize the leaderboard
        leaderboard = new InteractWithData();

        // Load the entries from the leaderboard
        List<InteractWithData.Data> entries = leaderboard.LoadEntries();

        // Sort the entries by score in descending order using LINQ
        entries = entries.OrderByDescending(entry => entry.score).ToList();

        // Initialize an empty string to hold the ranking display
        string rankingText = "Ranking:\n"; // Start with a header

        // Iterate through the entries to build the ranking string
        for (int i = 0; i < entries.Count; i++)
        {
            if (i == 8)
            {
                break;
            }
            // Truncate the name to fit within 8 characters
            string name = entries[i].name.Length > 8 ? entries[i].name.Substring(0, 8) : entries[i].name;

            // Format the score as a string, making sure it fits within 5 characters
            string score = entries[i].score.ToString();
            if (score.Length > 5)
            {
                score = score.Substring(0, 5); // Truncate if score exceeds 5 characters (for very large numbers)
            }

            // Format the entry with a max total length of 13 characters (name + score)
            string formattedEntry = (i + 1) + ". " + name + " - " + score;

            // Ensure the total length doesn't exceed 13 characters
            formattedEntry = formattedEntry.PadRight(13); // Pad the entry to make sure it is exactly 13 characters long

            // Add each entry to the rankingText
            rankingText += formattedEntry + "\n";
        }

        // Set the built ranking text to the TextMeshProUGUI component
        finalRanking.text = rankingText;
    }

    public void UpdateFinalRanking()
    {
        // Load the entries from the leaderboard
        List<InteractWithData.Data> entries = leaderboard.LoadEntries();

        // Sort the entries by score in descending order using LINQ
        entries = entries.OrderByDescending(entry => entry.score).ToList();

        // Initialize an empty string to hold the ranking display
        string rankingText = "Ranking:\n"; // Start with a header

        // Iterate through the entries to build the ranking string
        for (int i = 0; i < entries.Count; i++)
        {
            if (i == 8)
            {
                break;
            }
            // Truncate the name to fit within 8 characters
            string name = entries[i].name.Length > 8 ? entries[i].name.Substring(0, 8) : entries[i].name;

            // Format the score as a string, making sure it fits within 5 characters
            string score = entries[i].score.ToString();
            if (score.Length > 5)
            {
                score = score.Substring(0, 5); // Truncate if score exceeds 5 characters (for very large numbers)
            }

            // Format the entry with a max total length of 13 characters (name + score)
            string formattedEntry = (i + 1) + ". " + name + " - " + score;

            // Ensure the total length doesn't exceed 13 characters
            formattedEntry = formattedEntry.PadRight(13); // Pad the entry to make sure it is exactly 13 characters long

            // Add each entry to the rankingText
            rankingText += formattedEntry + "\n";
        }

        // Set the built ranking text to the TextMeshProUGUI component
        finalRanking.text = rankingText;
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
            int index = Random.Range(0, targets.Count);
            Instantiate(targets[index]);
        }
    }

    // Gets the name of the user that writed in the input field
    public void ReadInput()
    {
        nameScore = nameText.GetComponent<TextMeshProUGUI>().text;
        if (string.IsNullOrEmpty(nameScore))
        {
            return;  // Do nothing if nameScore is empty
        }
        Debug.Log("Text has been saved as: " + "\"" + nameScore + "\" with a score of  " + score);
        inputField.SetActive(false);
        leaderboard = new InteractWithData();
        leaderboard.SaveData(nameScore, score);
    }

}
