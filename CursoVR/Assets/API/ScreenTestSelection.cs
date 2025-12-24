using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ScreenTestSelection : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Transform _testContainer = null!;
    [SerializeField] private GameObject _testButtonPrefab = null!;
    [SerializeField] private TextMeshProUGUI _statusText = null!;
    [SerializeField] private Button _backButton = null!;
    [SerializeField] private Button _refreshButton = null!;

    [Header("VR Test Settings")]
    [SerializeField] private string _vrTestSceneName = "Weld";

    private List<GameObject> _testButtons = new List<GameObject>();
    private HttpClient _httpClient = null!;

    private void Start()
    {
        Debug.Log($"VR Scene name in Start: '{_vrTestSceneName}'"); // ДОБАВЬТЕ ЭТУ СТРОКУ

        InitializeHttpClient();
        _backButton.onClick.AddListener(GoBackToOrganizations);
        _refreshButton.onClick.AddListener(LoadVRAssignments);

        LoadVRAssignments();
    }

    private void InitializeHttpClient()
    {
        _httpClient = Authorization.HttpClient;
        if (_httpClient == null)
        {
            _httpClient = new HttpClient() { BaseAddress = new Uri("https://api.prof-testium.ru/") };
        }
    }

    private async void LoadVRAssignments()
    {
        _statusText.text = "Загрузка VR тестов...";

        try
        {
            var assignments = await GetVRAssignmentsFromAPI();

            if (assignments != null && assignments.Count > 0)
            {
                DisplayVRAssignments(assignments);
                _statusText.text = $"Доступно VR тестов: {assignments.Count}";
            }
            else
            {
                _statusText.text = "Нет назначенных VR тестов";
            }
        }
        catch (Exception ex)
        {
            _statusText.text = "Ошибка загрузки тестов";
            Debug.LogError($"Load VR assignments error: {ex.Message}");
        }
    }

    private async Task<List<VrExam>> GetVRAssignmentsFromAPI()
    {
        try
        {
            var response1 = await _httpClient.GetAsync("client/vr_exam/employers");

            if (response1.IsSuccessStatusCode)
            {
                var json = await response1.Content.ReadAsStringAsync();
                Debug.Log($"VR exams from employers: {json}");
                return ParseVrExams(json);
            }

            // Остальные endpoints...
            return new List<VrExam>();
        }
        catch (Exception ex)
        {
            Debug.LogError($"API call error: {ex.Message}");
            return new List<VrExam>();
        }
    }

    private List<VrExam> ParseVrExams(string json)
    {
        try
        {
            var wrappedResponse = JsonUtility.FromJson<VrExamDataResponse>(json);
            if (wrappedResponse != null && wrappedResponse.data != null && wrappedResponse.data.Count > 0)
            {
                Debug.Log($"Found {wrappedResponse.data.Count} VR exams in data response");
                return wrappedResponse.data;
            }

            Debug.LogWarning("No VR exams found in parsed response");
            return new List<VrExam>();
        }
        catch (Exception ex)
        {
            Debug.LogError($"Parse VR exams error: {ex.Message}");
            return new List<VrExam>();
        }
    }

    private void DisplayVRAssignments(List<VrExam> assignments)
    {
        foreach (var button in _testButtons)
        {
            if (button != null) Destroy(button);
        }
        _testButtons.Clear();

        foreach (var assignment in assignments)
        {
            var buttonObj = Instantiate(_testButtonPrefab, _testContainer);
            var button = buttonObj.GetComponent<Button>();
            var text = buttonObj.GetComponentInChildren<TextMeshProUGUI>();

            string buttonText = $"{assignment.name}";
            if (!string.IsNullOrEmpty(assignment.description))
            {
                buttonText += $"\n<size=60%>{assignment.description}</size>";
            }
            text.text = buttonText;

            button.onClick.AddListener(() => StartVRTest(assignment));
            _testButtons.Add(buttonObj);
        }
    }
    private async void StartVRTest(VrExam test)
    {
        _statusText.text = $"Запуск теста: {test.name}...";
        Debug.Log($"Starting VR test: {test.name} (ID: {test.id})");

        try
        {
            var attemptInfo = await StartVRAttempt(test.id);
            Debug.Log($"After StartVRAttempt - attemptInfo: {attemptInfo != null}");

            // ПРОВЕРЯЕМ РЕЗУЛЬТАТ И НЕ ПЕРЕХОДИМ ПРИ ОШИБКАХ
            if (attemptInfo == null)
            {
                _statusText.text = "❌ Ошибка начала теста. Сервер не ответил.";
                Debug.LogError("attemptInfo is NULL - not loading VR scene");
                return;
            }

            if (attemptInfo.attempt_id <= 0)
            {
                _statusText.text = "❌ Не удалось начать тест. Обратитесь к администратору.";
                Debug.LogError($"Invalid attempt_id: {attemptInfo.attempt_id} - not loading VR scene");
                return;
            }

            // ЕСЛИ attempt_id > 0 - ТЕСТ УСПЕШНО НАЧАТ, ПЕРЕХОДИМ В VR
            Debug.Log($"Attempt ID is valid: {attemptInfo.attempt_id}");

            AccountManager.Instance.SetCurrentTest(test);
            PlayerPrefs.SetInt("CurrentAttemptId", attemptInfo.attempt_id);
            PlayerPrefs.SetInt("CurrentTestId", test.id);
            PlayerPrefs.Save();

            Debug.Log($"Data saved - Attempt: {attemptInfo.attempt_id}, Test: {test.id}");

            _statusText.text = "✅ Тест начат! Переход в VR...";
            await Task.Delay(1000);

            LoadVRScene("Weld");
        }
        catch (Exception ex)
        {
            Debug.LogError($"Start VR test error: {ex.Message}");
            _statusText.text = "❌ Ошибка соединения. Проверьте интернет.";
            // НЕ ПЕРЕХОДИМ ПРИ ОШИБКАХ
        }
    }

    private async Task<VrAttemptResponse> StartVRAttempt(int testId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"client/employer-vr-exam/{testId}");
            Debug.Log($"Start attempt - Test ID: {testId}, Status: {response.StatusCode}");

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                Debug.Log($"Start attempt SUCCESS response: {json}");

                var attemptResponse = JsonUtility.FromJson<VrAttemptResponse>(json);
                if (attemptResponse != null && attemptResponse.attempt_id > 0)
                {
                    return attemptResponse;
                }

                var alternativeResponse = JsonUtility.FromJson<AlternativeAttemptResponse>(json);
                if (alternativeResponse != null && alternativeResponse.id > 0)
                {
                    return new VrAttemptResponse { attempt_id = alternativeResponse.id };
                }

                // НЕ ВОЗВРАЩАЕМ MOCK ID - ВОЗВРАЩАЕМ NULL ПРИ ОШИБКЕ ПАРСИНГА
                Debug.LogError("Could not parse valid attempt ID from response");
                return null;
            }
            else
            {
                // НЕ ВОЗВРАЩАЕМ MOCK ID ПРИ ОШИБКАХ API
                var errorContent = await response.Content.ReadAsStringAsync();
                Debug.LogError($"Start attempt FAILED - Status: {response.StatusCode}, Error: {errorContent}");

                // Показываем понятное сообщение об ошибке
                if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
                {
                    _statusText.text = "❌ Тест недоступен (истек срок или превышены попытки)";
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    _statusText.text = "❌ Тест не найден";
                }
                else
                {
                    _statusText.text = $"❌ Ошибка сервера: {response.StatusCode}";
                }

                return null; // ВОЗВРАЩАЕМ NULL ПРИ ОШИБКЕ
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"StartVRAttempt exception: {ex.Message}");
            _statusText.text = "❌ Ошибка сети. Проверьте подключение.";
            return null; // ВОЗВРАЩАЕМ NULL ПРИ ИСКЛЮЧЕНИИ
        }
    }

    private void LoadVRScene(string sceneName = null)
    {
        string targetScene = sceneName ?? _vrTestSceneName;
        Debug.Log($"LoadVRScene called - Scene name: '{targetScene}'");

        if (string.IsNullOrEmpty(targetScene))
        {
            Debug.LogError("Scene name is not set!");
            return;
        }

        try
        {
            if (Application.CanStreamedLevelBeLoaded(targetScene))
            {
                Debug.Log($"Loading scene: '{targetScene}'");
                SceneManager.LoadScene(targetScene);
            }
            else
            {
                Debug.LogError($"Scene '{targetScene}' does not exist in build settings!");

                Debug.Log("Available scenes:");
                for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
                {
                    string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
                    string availableScene = System.IO.Path.GetFileNameWithoutExtension(scenePath);
                    Debug.Log($"Scene {i}: {availableScene}");
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to load scene: {ex.Message}");
        }
    }

    private void GoBackToOrganizations()
    {
        SceneManager.LoadScene("TestSelection");
    }
}