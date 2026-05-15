using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct MissionResultRecord
{
    public float timeSeconds;
    public float maxSpeedKmh;
    public int maxForwardGear;
}

[Serializable]
public class MissionResultsData
{
    public List<MissionResultRecord> results = new List<MissionResultRecord>();
}

// Stores the last 5 mission attempts in PlayerPrefs.
public static class MissionResultsStorage
{
    private const string ResultsKey = "MissionResults_LastFive";
    private const int MaxResults = 5;

    public static MissionResultRecord[] LoadResults()
    {
        string json = PlayerPrefs.GetString(ResultsKey, string.Empty);
        if (string.IsNullOrEmpty(json))
        {
            return Array.Empty<MissionResultRecord>();
        }

        MissionResultsData data = JsonUtility.FromJson<MissionResultsData>(json);
        if (data == null || data.results == null)
        {
            return Array.Empty<MissionResultRecord>();
        }

        return data.results.ToArray();
    }

    public static void AddResult(MissionResultRecord result)
    {
        MissionResultsData data = LoadData();
        data.results.Insert(0, result);
        if (data.results.Count > MaxResults)
        {
            data.results.RemoveRange(MaxResults, data.results.Count - MaxResults);
        }

        SaveData(data);
    }

    public static int GetBestResultIndex(MissionResultRecord[] results)
    {
        if (results == null || results.Length == 0)
        {
            return -1;
        }

        int bestIndex = 0;
        float bestTime = results[0].timeSeconds;
        for (int i = 1; i < results.Length; i++)
        {
            if (results[i].timeSeconds < bestTime)
            {
                bestTime = results[i].timeSeconds;
                bestIndex = i;
            }
        }

        return bestIndex;
    }

    private static MissionResultsData LoadData()
    {
        string json = PlayerPrefs.GetString(ResultsKey, string.Empty);
        if (string.IsNullOrEmpty(json))
        {
            return new MissionResultsData();
        }

        MissionResultsData data = JsonUtility.FromJson<MissionResultsData>(json);
        return data ?? new MissionResultsData();
    }

    private static void SaveData(MissionResultsData data)
    {
        string json = JsonUtility.ToJson(data);
        PlayerPrefs.SetString(ResultsKey, json);
        PlayerPrefs.Save();
    }
}
