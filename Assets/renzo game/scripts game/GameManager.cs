using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class GameManager : MonoBehaviour
{
    public List<int> sequence = new List<int>();     // correct order
    public List<int> playerInput = new List<int>();  // what player clicks
    public int sequenceLength = 3;                   // increases over time
    public bool isShowingSequence;
    public bool isWaitingForMathAnswer = false;      // waiting for math answer
    public NumberTile[] allTiles; // assign in inspector
    public TMP_Text feedbackText;
    public TMP_Text mathProblemText;
    public TMP_Text levelText;                       // NEW: display level
    public TMP_InputField answerInput;
    public int level = 1;

    void Start()
    {
        StartNextRound();
    }

    void GenerateSequence()
    {
        sequence.Clear();
        HashSet<int> usedNumbers = new HashSet<int>();

        while (sequence.Count < sequenceLength)
        {
            int randomNumber = Random.Range(1, 10); // 1–9
            if (!usedNumbers.Contains(randomNumber))
            {
                sequence.Add(randomNumber);
                usedNumbers.Add(randomNumber);
            }
        }
    }

    IEnumerator PlaySequence()
    {
        isShowingSequence = true;
        playerInput.Clear();
        isWaitingForMathAnswer = false;

        yield return new WaitForSeconds(1f);

        foreach (int num in sequence)
        {
            NumberTile tile = FindTile(num);
            if (tile != null)
            {
                tile.Highlight();
                yield return new WaitForSeconds(0.7f);
            }
        }

        isShowingSequence = false;
        ShowFeedback("Click the tiles in the order you remember!");
    }

    NumberTile FindTile(int number)
    {
        return System.Array.Find(allTiles, t => t.number == number);
    }

    public void OnTileClicked(int number)
    {
        if (isShowingSequence) return;
        if (isWaitingForMathAnswer) return;  // Don't click tiles while doing math

        playerInput.Add(number);

        // Check if player clicked wrong tile
        if (playerInput[playerInput.Count - 1] != sequence[playerInput.Count - 1])
        {
            ShowFeedback("Wrong! Game Over.");
            ResetGame();
            return;
        }

        // Check if sequence is complete and correct
        if (playerInput.Count == sequence.Count)
        {
            ShowMathProblem();
        }
    }

    int CalculateCorrectAnswer()
    {
        int result = sequence[0];

        for (int i = 1; i < sequence.Count; i++)
        {
            result += sequence[i]; // start simple (addition)
        }

        return result;
    }

    void ShowMathProblem()
    {
        isWaitingForMathAnswer = true;
        string problem = string.Join(" + ", sequence) + " = ?";
        if (mathProblemText != null)
        {
            mathProblemText.text = problem;
        }
        ShowFeedback("Now solve the math problem!");
    }

    public void SubmitAnswer()
    {
        if (!isWaitingForMathAnswer)
        {
            ShowFeedback("First, click the tiles in order!");
            return;
        }

        int playerAnswer;
        if (!int.TryParse(answerInput.text, out playerAnswer))
        {
            ShowFeedback("Please enter a number.");
            return;
        }

        int correctAnswer = CalculateCorrectAnswer();

        if (playerAnswer == correctAnswer)
        {
            ShowFeedback("Correct! Level up!");
            sequenceLength++;
            level++;
            StartNextRound();
        }
        else
        {
            ShowFeedback("Math answer is wrong. Game Over.");
            ResetGame();
        }
    }

    void ResetGame()
    {
        sequenceLength = 3;
        level = 1;
        StartCoroutine(DelayedNextRound());
    }

    IEnumerator DelayedNextRound()
    {
        yield return new WaitForSeconds(2f);
        StartNextRound();
    }

    void ShowFeedback(string message)
    {
        if (feedbackText != null)
        {
            feedbackText.text = message;
        }
        else
        {
            Debug.LogWarning("Feedback Text is not assigned!");
        }
    }

    void UpdateLevelDisplay()
    {
        if (levelText != null)
        {
            levelText.text = "Level: " + level;
        }
    }

    void StartNextRound()
    {
        UpdateLevelDisplay();
        GenerateSequence();
        StartCoroutine(PlaySequence());
        answerInput.text = "";
    }
}