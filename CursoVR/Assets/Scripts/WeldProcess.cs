using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SocialPlatforms.Impl;

public enum CriterionName
{
    WeldDress,
    LegDress,
    Gloves,
    Mask,
    Rug,
    WireChecked,
    Grounding, 
    WireLengthSetted,
    SwitchOn,
    ElectrodeInHolder,
    SwitchOff,
    CindersRemoved,
    DetailOnTable,
    TableIsClear
}

public class WeldProcess : MonoBehaviour
{
    [SerializeField] private List<Criterion> _criteria = new List<Criterion> { 
        new Criterion(CriterionName.WeldDress, "Сварочный костюм"),
        new Criterion(CriterionName.LegDress, "Спец. обувь"),
        new Criterion(CriterionName.Gloves, "Перчатки"),
        new Criterion(CriterionName.Mask, "Маска"),
        new Criterion(CriterionName.Rug, "Коврик"),
        new Criterion(CriterionName.WireChecked, "Целостность провода"),
        new Criterion(CriterionName.Grounding, "Заземление"),
        new Criterion(CriterionName.WireLengthSetted, "Длина шнура"),
        new Criterion(CriterionName.SwitchOn, "Включение сварочного аппарата"),
        new Criterion(CriterionName.ElectrodeInHolder, "Установка электрода в держак"),
        new Criterion(CriterionName.SwitchOff, "Выключение сварочного аппарата"),
        new Criterion(CriterionName.CindersRemoved, "Огарки выброшены"),
        new Criterion(CriterionName.DetailOnTable, "Готовая деталь"),
        new Criterion(CriterionName.TableIsClear, "Соблюдение пожарной безопасности")
    };

    public List<Criterion> Criteria => _criteria;

    public bool DetailIsComplete;

    public static WeldProcess Instance;

    public string DropReason;

    public bool Running;
    public float TimeProcess;

    private void Awake()
    {
        Instance = this;
        Running = true;
    }

    private void FixedUpdate()
    {
        if (Running)
        {
            TimeProcess += Time.fixedDeltaTime;
        }
    }

    public Criterion GetCriterion(CriterionName name)
    {
        foreach (var criterion in _criteria)
        {
            if (criterion.Name == name)
            {
                return criterion;
            }
        }

        return null;
    }

    public void SetCriterion(CriterionName criterionName, bool complete)
    {
        foreach (var criterion in _criteria)
        {
            if(criterion.Name == criterionName)
            {
                criterion.Complete = complete;
                break;
            }
        }
    }

    public int GetCompletedCriteriaCount()
    {
        int count = 0;

        foreach (var criterion in _criteria)
        {
            if (criterion.Complete)
            {
                count++;
            }
        }

        return count;
    }

    public void DropResults()
    {
        foreach (var criterion in _criteria)
        {
            criterion.Complete = false;
        }
    }

    public async void SendResult()
    {
        Running = false;
        await SendResultsAsync();
    }

    async Task SendResultsAsync() // отправка результатов на сервер с использованием токена
    {
        using var client = new HttpClient()
        {
            BaseAddress = new Uri("https://0435-176-28-64-201.ngrok-free.app/api/"), // адрес сервера
        };
        client.DefaultRequestHeaders.Add("ngrok-skip-browser-warning", "69420");

        client.DefaultRequestHeaders.Add("Authorization", AccountManager.Instance.Token.tokenPair.accessToken);//установка токена в header

        var sessonResult = new SessionResult
        {
            score = GetCompletedCriteriaCount(),
            maxScore = _criteria.Count,
            duration = new TimeSpan(0, (int)TimeProcess / 60, (int)TimeProcess % 60),
            mark = GetCompletedCriteriaCount() / _criteria.Count * 10,
            descriptionEvaluationReason = DropReason,
            isSuccessful = false
        };

        var sessionResultString = JsonUtility.ToJson(sessonResult);
        var sessionResultContent = new StringContent(sessionResultString, Encoding.UTF8, MediaTypeNames.Application.Json);

        var responseCreatesession = await client.PostAsync("session/Сварщики", sessionResultContent);
        string? sessionId = null;
        if(responseCreatesession.StatusCode == HttpStatusCode.OK)
        {
            var sessionResultBodyString = await responseCreatesession.Content.ReadAsStringAsync();
            var sessionResultBody = JsonUtility.FromJson<SessionResultBody>(sessionResultBodyString);
            print(sessionResultBody.score);
            print(sessionResultBody.maxScore);
        }
    }
}

[Serializable]
public class SessionResult
{
    public TimeSpan duration;
    public float score;
    public float maxScore;
    public bool isSuccessful;
    public float mark;
    public string descriptionEvaluationReason;
}

[Serializable]
public class SessionResultBody
{
    public string id;
    public string date;
    public string duration;
    public float score;
    public float maxScore;
    public bool isSuccessful;
    public float mark;
    public string descriptionEvaluationReason;
    public string? urlRecordingFile;
}


[Serializable]
public class Criterion
{
    public CriterionName Name;
    public string Description;
    public bool Complete;

    public Criterion(CriterionName name, string description)
    {
        Name = name;
        Complete = false;
        Description = description;
    }
}
