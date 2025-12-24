#nullable enable

using System;
using System.Collections.Generic;
using UnityEngine;

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
    [SerializeField]
    private List<Criterion> _criteria = new List<Criterion> {
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
    public static WeldProcess? Instance;
    public string? DropReason;
    public bool Running;
    public float TimeProcess;

    private void Awake()
    {
        Instance = this;
        Running = true;
    }
    public void SendResult()
    {
        // Просто запускаем отправку результатов через CompleteTable
        // или вызываем событие для других скриптов
        Debug.Log("SendResult called - расчет баллов завершен");

        // Можно вызвать событие, если нужно уведомить другие скрипты
        OnResultsSent?.Invoke(CalculateScore(), GetCompletedCriteriaCount(), Criteria.Count);
    }

    // Событие для уведомления о завершении (если нужно)
    public event Action<int, int, int>? OnResultsSent;

    private void FixedUpdate()
    {
        if (Running)
        {
            TimeProcess += Time.fixedDeltaTime;
        }
    }

    // ТОЛЬКО РАСЧЕТ БАЛЛОВ И ЛОГИКА КРИТЕРИЕВ
    public int CalculateScore()
    {
        if (_criteria.Count == 0) return 0;
        int completedCount = GetCompletedCriteriaCount();
        return (int)((completedCount / (float)_criteria.Count) * 100);
    }

    public Criterion? GetCriterion(CriterionName name)
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
            if (criterion.Name == criterionName)
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
        DropReason = null;
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
            Description = description;
            Complete = false;
        }
    }
}

#nullable restore