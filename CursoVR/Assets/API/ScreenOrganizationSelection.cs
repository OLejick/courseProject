using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class OrganizationSelection : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Transform _organizationContainer = null!;
    [SerializeField] private GameObject _organizationButtonPrefab = null!;
    [SerializeField] private TextMeshProUGUI _statusText = null!;
    [SerializeField] private Button _backButton = null!;
    [SerializeField] private Button _refreshButton = null!;

    private List<GameObject> _organizationButtons = new List<GameObject>();
    private HttpClient _httpClient = null!;

    private void Start()
    {
        Debug.Log("OrganizationSelection Start called");

        if (AccountManager.Instance == null)
        {
            Debug.LogError("AccountManager.Instance is NULL!");
            _statusText.text = "Ошибка: AccountManager не найден. Вернитесь к авторизации.";
            return;
        }

        if (AccountManager.Instance.UserData == null)
        {
            Debug.LogError("UserData is NULL!");
            _statusText.text = "Ошибка: Данные пользователя не найдены. Вернитесь к авторизации.";
            return;
        }

        InitializeHttpClient();
        _backButton.onClick.AddListener(Logout);
        _refreshButton.onClick.AddListener(LoadOrganizations);

        LoadOrganizations();
    }

    private void InitializeHttpClient()
    {
        Debug.Log("Initializing HttpClient...");

        _httpClient = Authorization.HttpClient;

        if (_httpClient == null)
        {
            Debug.LogError("Global HttpClient is null! Creating new one...");
            _httpClient = new HttpClient()
            {
                BaseAddress = new Uri("https://api.prof-testium.ru/"),
            };
        }

        Debug.Log("Using global HttpClient with cookies");
    }

    private async void LoadOrganizations()
    {
        _statusText.text = "Загрузка организаций...";

        if (AccountManager.Instance == null || AccountManager.Instance.UserData == null)
        {
            _statusText.text = "Ошибка: нет авторизации. Вернитесь к входу.";
            return;
        }

        try
        {
            // ПРОБУЕМ ПОЛУЧИТЬ АКТУАЛЬНЫЕ ДАННЫЕ ИЗ API
            var organizations = await GetUserOrganizationsFromAPI();

            if (organizations != null && organizations.Count > 0)
            {
                DisplayOrganizations(organizations);
                _statusText.text = $"Выберите организацию ({organizations.Count} доступно):";
            }
            else
            {
                // ЕСЛИ API НЕ ВЕРНУЛ ДАННЫЕ, ИСПОЛЬЗУЕМ ДАННЫЕ ИЗ АВТОРИЗАЦИИ
                var organizationsFromAuth = GetOrganizationsFromUserData();
                if (organizationsFromAuth.Count > 0)
                {
                    DisplayOrganizations(organizationsFromAuth);
                    _statusText.text = $"Выберите организацию ({organizationsFromAuth.Count} доступно):";
                }
                else
                {
                    _statusText.text = "Нет доступных организаций";
                }
            }
        }
        catch (System.Exception ex)
        {
            _statusText.text = "Ошибка загрузки организаций";
            Debug.LogError($"Load organizations error: {ex.Message}");

            // Fallback на данные из авторизации
            var organizationsFromAuth = GetOrganizationsFromUserData();
            if (organizationsFromAuth.Count > 0)
            {
                DisplayOrganizations(organizationsFromAuth);
                _statusText.text = $"Выберите организацию ({organizationsFromAuth.Count} доступно):";
            }
        }
    }

    private async Task<List<Organization>> GetUserOrganizationsFromAPI()
    {
        if (_httpClient == null)
        {
            Debug.LogError("HTTP Client is null");
            return new List<Organization>();
        }

        try
        {
            var response = await _httpClient.GetAsync("client/auth/check-auth-global");

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                Debug.Log($"API Organizations response: {json}");

                // ПРОБУЕМ РАЗНЫЕ ФОРМАТЫ ОТВЕТА
                try
                {
                    // Формат 1: UserGlobalProfile (основной формат из check-auth-global)
                    var globalProfile = JsonUtility.FromJson<UserGlobalProfile>(json);
                    if (globalProfile != null && globalProfile.employers != null && globalProfile.employers.Count > 0)
                    {
                        Debug.Log($"Found {globalProfile.employers.Count} employers in global profile");
                        return ConvertEmployersToOrganizations(globalProfile.employers);
                    }
                }
                catch (Exception ex1)
                {
                    Debug.LogWarning($"Failed to parse as UserGlobalProfile: {ex1.Message}");
                }

                try
                {
                    // Формат 2: UserLoginResponse (альтернативный формат)
                    var userResponse = JsonUtility.FromJson<UserLoginResponse>(json);
                    if (userResponse != null && userResponse.employers != null && userResponse.employers.Count > 0)
                    {
                        Debug.Log($"Found {userResponse.employers.Count} employers in user response");
                        return ConvertEmployersToOrganizations(userResponse.employers);
                    }
                }
                catch (Exception ex2)
                {
                    Debug.LogWarning($"Failed to parse as UserLoginResponse: {ex2.Message}");
                }

                Debug.LogWarning("Could not parse organizations from API response");
                return new List<Organization>();
            }
            else
            {
                Debug.LogError($"Failed to get organizations from API: {response.StatusCode}");
                return new List<Organization>();
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Get organizations API error: {ex.Message}");
            return new List<Organization>();
        }
    }

    private List<Organization> GetOrganizationsFromUserData()
    {
        if (AccountManager.Instance.UserData?.employers != null && AccountManager.Instance.UserData.employers.Count > 0)
        {
            Debug.Log($"Using {AccountManager.Instance.UserData.employers.Count} organizations from UserData");
            return ConvertEmployersToOrganizations(AccountManager.Instance.UserData.employers);
        }

        Debug.LogWarning("No organizations found in UserData");
        return new List<Organization>();
    }

    // КОНВЕРТИРУЕМ EMPLOYERS В ORGANIZATIONS
    private List<Organization> ConvertEmployersToOrganizations(List<Employer> employers)
    {
        var organizations = new List<Organization>();
        foreach (var employer in employers)
        {
            if (employer.organization != null)
            {
                organizations.Add(new Organization
                {
                    id = employer.organization.id,
                    name = employer.organization.name,
                    description = $"Роль: {employer.role?.name ?? "Не указана"}"
                });
            }
        }
        return organizations;
    }

    private void DisplayOrganizations(List<Organization> organizations)
    {
        foreach (var button in _organizationButtons)
        {
            Destroy(button);
        }
        _organizationButtons.Clear();

        foreach (var org in organizations)
        {
            var buttonObj = Instantiate(_organizationButtonPrefab, _organizationContainer);
            var button = buttonObj.GetComponent<Button>();
            var text = buttonObj.GetComponentInChildren<TextMeshProUGUI>();

            text.text = $"{org.name}\n<size=70%>{org.description}</size>";

            button.onClick.AddListener(() => SelectOrganization(org));

            _organizationButtons.Add(buttonObj);
        }
    }

    private async void SelectOrganization(Organization organization)
    {
        _statusText.text = $"Вход в {organization.name}...";

        try
        {
            var success = await LoginToOrganization(organization.id);

            if (success)
            {
                AccountManager.Instance.SetCurrentOrganization(organization);
                _statusText.text = $"Успешный вход в {organization.name}!";
                Debug.Log($"Successfully logged into organization: {organization.name} (ID: {organization.id})");

                await Task.Delay(1000);
                SceneManager.LoadScene(2);
            }
            else
            {
                _statusText.text = "Ошибка входа в организацию";
            }
        }
        catch (System.Exception ex)
        {
            _statusText.text = "Ошибка соединения";
            Debug.LogError($"Select organization error: {ex.Message}");
        }
    }

    private async Task<bool> LoginToOrganization(int organizationId)
    {
        if (_httpClient == null)
        {
            Debug.LogError("HTTP Client is null");
            return false;
        }

        // НАХОДИМ ПРАВИЛЬНЫЙ employer_id ДЛЯ ВЫБРАННОЙ ОРГАНИЗАЦИИ
        int employerId = FindEmployerIdForOrganization(organizationId);

        if (employerId == 0)
        {
            Debug.LogError($"Employer not found for organization {organizationId}");
            _statusText.text = "Ошибка: не найден сотрудник для организации";
            return false;
        }

        var loginData = new LoginOrganizationDto
        {
            employer_id = employerId,
            organization_id = organizationId
        };

        var json = JsonUtility.ToJson(loginData);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        Debug.Log($"Sending organization login: {json}");

        try
        {
            var response = await _httpClient.PostAsync("client/auth/login-organization", content);

            if (response.IsSuccessStatusCode)
            {
                Debug.Log($"Successfully logged into organization {organizationId} with employer {employerId}");
                return true;
            }
            else
            {
                Debug.LogError($"Failed to login to organization: {response.StatusCode}");
                var errorContent = await response.Content.ReadAsStringAsync();
                Debug.LogError($"Error response: {errorContent}");
                return false;
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Login request error: {ex.Message}");
            return false;
        }
    }

    private int FindEmployerIdForOrganization(int organizationId)
    {
        // ПРОБУЕМ НАЙТИ В АКТУАЛЬНЫХ ДАННЫХ
        if (AccountManager.Instance.UserData?.employers != null)
        {
            var employer = AccountManager.Instance.UserData.employers
                .FirstOrDefault(e => e.organization?.id == organizationId);

            if (employer != null)
            {
                Debug.Log($"Found employer ID: {employer.id} for organization: {organizationId}");
                return employer.id;
            }
        }

        Debug.LogWarning($"Employer not found for organization {organizationId}");
        return 0;
    }

    private void Logout()
    {
        AccountManager.Instance?.ClearSelection();
        Authorization.HttpClient?.Dispose();
        Authorization.HttpClient = null;
        SceneManager.LoadScene(0);
    }

    private void OnDestroy()
    {
        // НЕ УНИЧТОЖАЕМ HttpClient, ОН ОБЩИЙ
    }
}