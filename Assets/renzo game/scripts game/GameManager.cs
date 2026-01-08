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
    public NumberTile[] allTiles; // assign in inspector
    public TMP_Text feedbackText;
    public TMP_Text mathProblemText;
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
        ShowMathProblem();
    }

    NumberTile FindTile(int number)
    {
        return System.Array.Find(allTiles, t => t.number == number);
    }

    public void OnTileClicked(int number)
    {
        if (isShowingSequence) return;

        playerInput.Add(number);
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
        string problem = string.Join(" + ", sequence) + " = ?";
        mathProblemText.text = problem;
    }

    public void SubmitAnswer()
    {
        if (!SequenceCorrect())
        {
            ShowFeedback("Wrong order! Try again.");
            ResetGame();
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
            ShowFeedback("Correct! 🎉 Level up!");
            sequenceLength++;
            level++;
            StartNextRound();
        }
        else
        {
            ShowFeedback("Math answer is wrong. Try again.");
            ResetGame();
        }
    }

    void ResetGame()
    {
        sequenceLength = 3;
        level = 1;
        StartNextRound();
    }

    bool SequenceCorrect()
    {
        if (playerInput.Count != sequence.Count) return false;

        for (int i = 0; i < sequence.Count; i++)
        {
            if (playerInput[i] != sequence[i])
                return false;
        }

        return true;
    }

    void ShowFeedback(string message)
    {
        feedbackText.text = message;
    }

    void StartNextRound()
    {
        GenerateSequence();
        StartCoroutine(PlaySequence());
        answerInput.text = "";
        feedbackText.text = "";
    }
}