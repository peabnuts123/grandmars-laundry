using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    // Singleton
    public static GameManager _instance;

    // Public members
    public GameObject[] laundryItems;
    public GameObject[] lifeIndicators;
    public AudioClip dealHandSound;
    public AudioClip levelFailedSound;
    public LevelEndDialog levelEndDialog;
    public GameObject pauseDialog;
    public GameObject gameOverDialog;

    // Private members
    private IList<LaundryItem> currentDeck;
    private Text pointsDisplay;
    private Text remainingDisplay;
    private Text levelDisplay;
    private Text timerDisplay;
    private AudioSource audioSource;
    private AudioSource musicPlayer;

    // Private State
    private float totalPoints;
    private float levelPoints;
    private int levelNumber;
    private float timeLeft;
    private bool isSuspended;
    private int numLives;

    public GameManager()
    {
        // Singleton
        if (GameManager._instance == null)
        {
            GameManager._instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    void OnEnable()
    {
        currentDeck = new List<LaundryItem>();
        pointsDisplay = GameObject.FindGameObjectWithTag("PointsDisplay").GetComponent<Text>();
        remainingDisplay = GameObject.FindGameObjectWithTag("RemainingDisplay").GetComponent<Text>();
        levelDisplay = GameObject.FindGameObjectWithTag("LevelDisplay").GetComponent<Text>();
        timerDisplay = GameObject.FindGameObjectWithTag("TimerDisplay").GetComponent<Text>();
        audioSource = GetComponent<AudioSource>();
        musicPlayer = GameObject.FindGameObjectWithTag("MusicPlayer").GetComponent<AudioSource>();
    }

    void Start()
    {
        this.levelNumber = 1;
        this.numLives = lifeIndicators.Length;
        StartNewLevel();
    }

    void StartNewLevel()
    {
        SetGameSuspended(false);

        this.levelPoints = 0;

        int deckSize = 10 + (Mathf.RoundToInt(Mathf.Pow(1.4F, this.levelNumber)) - 1);
        DealNewDeck(deckSize);
        timeLeft = 60;

        // Initialise PlayerHand handler
        PlayerHand._instance.Init();
        ClothesLine._instance.Init();

        UpdateDisplays();
    }

    void Update()
    {
        bool escapePressed = Input.GetKeyDown(KeyCode.Escape);

        if (!isSuspended)
        {
            int originalTimeLeft = (int)(this.timeLeft);
            this.timeLeft -= Time.deltaTime;
            int newTimeLeft = (int)(this.timeLeft);

            if (newTimeLeft != originalTimeLeft)
            {
                UpdateDisplays();
            }

            if (this.timeLeft < 0)
            {
                UpdateDisplays();
                LevelFail();
            }

            if (escapePressed)
            {
                PauseGame();
            }
        }
        else
        {
            if (escapePressed)
            {
                UnpauseGame();
            }
        }
    }

    public LaundryItem RequestLaundryItemFromDeck()
    {
        if (this.currentDeck.Count == 0)
        {
            throw new InvalidOperationException("Deck is empty");
        }

        int popIndex = 0;
        LaundryItem returnValue = this.currentDeck[popIndex];
        this.currentDeck.RemoveAt(popIndex);
        return returnValue;
    }

    void DealNewDeck(int size)
    {
        this.currentDeck.Clear();
        for (int i = 0; i < size; i++)
        {
            LaundryItem laundryItem = Utility.GetRandomLaundryItem();
            this.currentDeck.Add(laundryItem);
        }
    }

    public void BankItems(IEnumerable<Laundry> laundryObjects)
    {
        foreach (Laundry laundry in laundryObjects)
        {
            this.levelPoints += laundry.pointValue;
        }

        UpdateDisplays();
        PlayerHand._instance.Deal();

        PlayAudioClip(this.dealHandSound);

        if (AreNoCardsLeft())
        {
            LevelSuccess();
        }
    }

    public void UpdateDisplays()
    {
        pointsDisplay.text = "Pocket Money: $" + (this.totalPoints + this.levelPoints).ToString("F");
        remainingDisplay.text = "Laundry Remaining: " + GetTotalNumberOfCardsInPlay();
        levelDisplay.text = "Level: " + levelNumber;
        timerDisplay.text = "" + (int)(this.timeLeft + 1F);
    }

    public int GetTotalNumberOfCardsInPlay()
    {
        return this.currentDeck.Count + PlayerHand._instance.GetNumberOfCardsInHand();
    }

    public bool AreNoCardsLeft()
    {
        return GetTotalNumberOfCardsInPlay() == 0;
    }

    public bool IsDeckEmpty()
    {
        return this.currentDeck.Count == 0;
    }

    public void PlayAudioClip(AudioClip clip)
    {
        PlayAudioClip(clip, 0);
    }

    public void PlayAudioClipWithRandomPitch(AudioClip clip)
    {
        PlayAudioClip(clip, 0.03F);
    }

    public void PlayAudioClip(AudioClip clip, float randomnessFactor)
    {
        float pitchOffset = Random.Range(-randomnessFactor, randomnessFactor);
        this.audioSource.pitch = 1 + pitchOffset;
        this.audioSource.PlayOneShot(clip);
    }

    public void LevelSuccess()
    {
        LevelEnd();
        ShowLevelEndDialog(true);
    }

    public void LevelFail()
    {
        PlayAudioClip(levelFailedSound);

        OffsetLives(-1);
        if (numLives < 0)
        {
            GameOver();
            return;
        }

        LevelEnd();
        ShowLevelEndDialog(false);
    }

    void OffsetLives(int offset)
    {
        this.numLives += offset;
        for (int i = 0; i < this.lifeIndicators.Length; i++)
        {
            this.lifeIndicators[i].SetActive(i < this.numLives);
        }
    }

    public void LevelEnd()
    {
        if (this.levelNumber == 1)
        {
            ToggleHelp._instance.HideHelpOverlay();
        }

        SetGameSuspended(true);
    }

    void ShowLevelEndDialog(bool isSuccess)
    {
        levelEndDialog.gameObject.SetActive(true);
        levelEndDialog.isSuccess = isSuccess;
    }

    void PauseGame()
    {
        pauseDialog.SetActive(true);
        SetGameSuspended(true);
    }

    public void UnpauseGame()
    {
        pauseDialog.SetActive(false);
        SetGameSuspended(false);
    }

    void GameOver()
    {
        gameOverDialog.SetActive(true);
        SetGameSuspended(true);
    }

    public void TransitionToNextLevel(bool isSuccess)
    {
        if (isSuccess)
        {
            this.totalPoints += this.levelPoints;
            this.levelNumber++;
        }

        StartNewLevel();
    }

    void SetGameSuspended(bool isSuspended)
    {
        ClothesLine._instance.isSuspended = isSuspended;
        PlayerHand._instance.isSuspended = isSuspended;
        this.isSuspended = isSuspended;
    }
}
