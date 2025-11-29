using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ResultsUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _criteriaCount;
    [SerializeField] private TextMeshProUGUI _dropReason;

    [SerializeField] private CriterionElement _criteriaPrefab;

    [SerializeField] private Transform _criteriaList;

    private void OnEnable()
    {
        if (!WeldProcess.Instance.GetCriterion(CriterionName.TableIsClear).Complete)
        {
            WeldProcess.Instance.DropResults();
            _dropReason.text = "Не соблюдена пожарная безопасность!";
            WeldProcess.Instance.DropReason = "Не соблюдена пожарная безопасность";
        }

        foreach (var criterion in WeldProcess.Instance.Criteria)
        {
            var c = Instantiate(_criteriaPrefab, _criteriaList);
            c.Text.text = criterion.Description;
            c.Toggle.isOn = criterion.Complete;
        }

        _criteriaCount.text = $"Набрано баллов: {WeldProcess.Instance.GetCompletedCriteriaCount()}";

        WeldProcess.Instance.SendResult();
    }

    public void Exit()
    {
        Application.Quit();
    }
}
