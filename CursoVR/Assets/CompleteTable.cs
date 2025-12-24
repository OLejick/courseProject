using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class CompleteTable : MonoBehaviour
{
    [Header("UI Settings")]
    [SerializeField] private GameObject _resultsUI = null!;
    [SerializeField] private TMPro.TextMeshProUGUI _resultsText = null!;

    private bool _resultsSent = false;

    private void OnTriggerExit(Collider col)
    {
        if (col.TryGetComponent(out Flammable flammable))
        {
            WeldProcess.Instance?.SetCriterion(CriterionName.TableIsClear, true);
        }
    }

    private void OnTriggerEnter(Collider col)
    {
        if (col.TryGetComponent(out Flammable flammable))
        {
            WeldProcess.Instance?.SetCriterion(CriterionName.TableIsClear, false);
        }

        if (col.TryGetComponent(out WeldDetail detail) && !_resultsSent)
        {
            if (WeldProcess.Instance != null && WeldProcess.Instance.DetailIsComplete)
            {
                WeldProcess.Instance.SetCriterion(CriterionName.DetailOnTable, true);
            }

            // Активируем UI и отправляем результаты
            ShowResults();
        }
    }

    private async void ShowResults()
    {
        if (_resultsUI != null)
        {
            _resultsUI.SetActive(true);
        }

        await SendVRResultsAsync();
    }

    private async Task SendVRResultsAsync()
    {
        if (_resultsSent) return;

        if (AccountManager.Instance == null)
        {
            Debug.LogError("AccountManager is null!");
            UpdateResultsText("Ошибка: менеджер аккаунта не найден");
            return;
        }

        var currentTest = AccountManager.Instance.CurrentTest;
        var currentAttemptId = PlayerPrefs.GetInt("CurrentAttemptId", 0);

        if (currentTest == null)
        {
            Debug.LogError("No current test found!");
            UpdateResultsText("Ошибка: тест не найден");
            return;
        }

        if (currentAttemptId == 0)
        {
            Debug.LogError("No attempt ID found!");
            UpdateResultsText("Ошибка: ID попытки не найден");
            return;
        }

        int score = CalculateScore();
        Debug.Log($"Отправка результатов: Test ID: {currentTest.id}, Attempt ID: {currentAttemptId}, Score: {score}");

        try
        {
            var httpClient = Authorization.HttpClient;
            if (httpClient == null)
            {
                Debug.LogError("HTTP Client is null!");
                UpdateResultsText("Ошибка соединения");
                return;
            }

            // ИСПРАВЛЕННАЯ СТРУКТУРА ДАННЫХ
            var resultData = new UpdateEmployerVRExamAttemptDto
            {
                attempt_id = currentAttemptId,
                score = score
            };

            var json = JsonUtility.ToJson(resultData);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            Debug.Log($"Sending POST to: client/employer-vr-exam/{currentTest.id}");
            Debug.Log($"Request JSON: {json}");

            var response = await httpClient.PostAsync($"client/employer-vr-exam/{currentTest.id}", content);

            if (response.IsSuccessStatusCode)
            {
                _resultsSent = true;
                string successMessage = $"✅ Результаты отправлены!\nОценка: {score}/100";
                Debug.Log($"VR results sent successfully! Score: {score}%");
                UpdateResultsText(successMessage);

                PlayerPrefs.SetInt("LastVRScore", score);
                PlayerPrefs.Save();
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                string errorMessage = $"Ошибка: {response.StatusCode}\n{errorContent}";
                Debug.LogError($"Failed to send results: {response.StatusCode}");
                Debug.LogError($"Error response: {errorContent}");
                UpdateResultsText(errorMessage);
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Send results error: {ex.Message}");
            UpdateResultsText("Ошибка соединения");
        }
    }

    private int CalculateScore()
    {
        if (WeldProcess.Instance == null)
        {
            Debug.LogWarning("WeldProcess instance is null");
            return 0;
        }

        try
        {
            // ИСПРАВЛЕННЫЙ РАСЧЕТ БАЛЛОВ - используем свойство Complete критериев
            int completedCount = 0;
            int totalCount = WeldProcess.Instance.Criteria.Count;

            if (totalCount == 0) return 0;

            // ПРАВИЛЬНЫЙ ПОДСЧЕТ ВЫПОЛНЕННЫХ КРИТЕРИЕВ
            foreach (var criterion in WeldProcess.Instance.Criteria)
            {
                if (criterion.Complete) // ПРОСТО используем свойство Complete
                {
                    completedCount++;
                    Debug.Log($"✅ Критерий выполнен: {criterion.Name}");
                }
                else
                {
                    Debug.Log($"❌ Критерий не выполнен: {criterion.Name}");
                }
            }

            int score = Mathf.Clamp((int)((completedCount / (float)totalCount) * 100), 0, 100);
            Debug.Log($"Final Score: {completedCount}/{totalCount} = {score}%");
            return score;
        }
        catch (Exception ex)
        {
            Debug.LogError($"Calculate score error: {ex.Message}");
            return 0;
        }
    }

    private void UpdateResultsText(string message)
    {
        if (_resultsText != null)
        {
            _resultsText.text = message;
        }
    }
}