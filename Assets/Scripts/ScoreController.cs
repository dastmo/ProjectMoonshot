using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreController : MonoBehaviour
{
    public int SecondsRemaining { get; set; } = 0;
    public float DebrisMassCollected { get; set; } = 0f;
    public float DebrisMassRemaining { get; set; } = 0f;

    public float DebrisCollectedPercentage { get; set; } = 0f;
    public int DebrisScore { get; set; } = 0;

    private Dictionary<string, int> ScoreLetterGrades = new Dictionary<string, int>();

    private int _finalScore = 0;
    private string _finalLetterGrade = "F";
    public int FinalScore { get => _finalScore; }
    public string FinalLetterGrade { get => _finalLetterGrade; }

    public Action ScoreCalculated;

    private void Awake()
    {
        PopulateLetterGradesDictionary();
        GameController.GameEnded += OnGameEnded;
    }

    private void PopulateLetterGradesDictionary()
    {
        ScoreLetterGrades.Add("F", 0);
        ScoreLetterGrades.Add("D-", 1);
        ScoreLetterGrades.Add("D", 250000);
        ScoreLetterGrades.Add("D+", 500000);
        ScoreLetterGrades.Add("C-", 750000);
        ScoreLetterGrades.Add("C", 1000000);
        ScoreLetterGrades.Add("C+", 1250000);
        ScoreLetterGrades.Add("B-", 1500000);
        ScoreLetterGrades.Add("B", 1750000);
        ScoreLetterGrades.Add("B+", 2000000);
        ScoreLetterGrades.Add("A-", 2300000);
        ScoreLetterGrades.Add("A", 2600000);
        ScoreLetterGrades.Add("A+", 2900000);
        ScoreLetterGrades.Add("S", 3200000);
    }

    private void OnGameEnded(int secondsRemaining, float debrisCollected, float debrisRemaining)
    {
        SecondsRemaining = secondsRemaining;
        DebrisMassCollected = debrisCollected;
        DebrisMassRemaining = debrisRemaining;
        DebrisCollectedPercentage = DebrisMassCollected / (DebrisMassCollected + DebrisMassRemaining);

        _finalScore = CalculateScore();
        _finalLetterGrade = GetLetterGrade(_finalScore);

        ScoreCalculated?.Invoke();
    }

    private int CalculateScore()
    {
        int score = 0;

        int debrisPercent = Mathf.RoundToInt(DebrisCollectedPercentage * 100);
        DebrisScore = debrisPercent * 20000;
        score += DebrisScore;

        score += SecondsRemaining * 1000;

        return score;
    }

    private string GetLetterGrade(int score)
    {
        int lastScoreFloorEvaluated = 0;
        string letterGrade = "F";

        foreach (var item in ScoreLetterGrades)
        {
            if (item.Value > lastScoreFloorEvaluated && score >= item.Value)
            {
                lastScoreFloorEvaluated = item.Value;
                letterGrade = item.Key;
            }
        }

        return letterGrade;
    }

    private void OnDestroy()
    {
        GameController.GameEnded -= OnGameEnded;
    }
}
