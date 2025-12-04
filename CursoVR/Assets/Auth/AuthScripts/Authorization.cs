using System;
using System.Net;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class Authorization : MonoBehaviour
{
    [Header("InputFields")]
    [SerializeField] private TMP_InputField _phoneInputField = null!;
    [SerializeField] private TMP_InputField _passwordInputField = null!;

    [SerializeField] private TextMeshProUGUI _requestText = null!;
    public static HttpClient? HttpClient { get; set; }

    private void Start()
    {
        if (HttpClient == null)
        {
            HttpClient = new HttpClient()
            {
                BaseAddress = new Uri("https://api.prof-testium.ru/"),
            };
            Debug.Log("Global HttpClient created for cookie-based auth");

        }
    }

    public async void Auth()
    {
        await AuthAsync();
    }

    public async Task AuthAsync()
    {
        var loginData = new LoginByPhoneDto
        {
            phone = _phoneInputField.text,
            password = _passwordInputField.text
        };
        _requestText.text = "Вход...";
        try
        {
            var json = JsonUtility.ToJson(loginData);
            var content = new StringContent(json, Encoding.UTF8, MediaTypeNames.Application.Json);

            var response = await HttpClient!.PutAsync("client/auth/login", content);

            if (response.StatusCode == HttpStatusCode.Created)
            {
                var reponseString = await response.Content.ReadAsStringAsync();
                Debug.Log($"Успешный вход: {reponseString}");
                var userReponse = JsonUtility.FromJson<UserLoginResponse>(reponseString);

                if (userReponse != null)
                {
                    AccountManager.Instance.SetUserData(userReponse);
                    _requestText.text = "Успешный вход!";
                    SceneManager.LoadScene(1);
                }
                else
                {
                    _requestText.text = "Вход выполнен!";
                    SceneManager.LoadScene(1);
                }
            }else if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                _requestText.text = "Неверный телефон или пароль";
            }
            else
            {
                _requestText.text = $"Ошибка: {response.StatusCode}";
                Debug.LogError($"Ошибка входа: {response.StatusCode}");
            }
        }
        catch (Exception ex)
        {
            _requestText.text = "Ошибка соединения";
            Debug.LogError($"Auth error: {ex.Message}");
        }
    }
    private void OnDestroy()
    {
        // не уничтожаем
    }
}

