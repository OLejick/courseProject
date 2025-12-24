using System;
using System.Net;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

#nullable enable

public class Authorization : MonoBehaviour
{
    [Header("InputFields")]
    [SerializeField] private TMP_InputField _phoneInputField = null!;
    [SerializeField] private TMP_InputField _passwordInputField = null!;

    [SerializeField] private TextMeshProUGUI _requestText = null!;

    // ÈÑÏĞÀÂËÅÍÎ: Äîáàâëåí setter äëÿ HttpClient
    public static HttpClient? HttpClient { get; set; }

    private void Start()
    {
        // ÑÎÇÄÀÅÌ ÅÄÈÍÛÉ HTTP CLIENT ÅÑËÈ ÅÃÎ ÍÅÒ
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

        _requestText.text = "Âõîä...";

        try
        {
            var json = JsonUtility.ToJson(loginData);
            var content = new StringContent(json, Encoding.UTF8, MediaTypeNames.Application.Json);

            // ÈÑÏÎËÜÇÓÅÌ ÎÁÙÈÉ HTTP CLIENT (COOKIES ÑÎÕĞÀÍßŞÒÑß ÀÂÒÎÌÀÒÈ×ÅÑÊÈ)
            var response = await HttpClient!.PostAsync("client/auth/login", content);

            if (response.StatusCode == HttpStatusCode.Created)
            {
                var responseString = await response.Content.ReadAsStringAsync();
                Debug.Log($"Óñïåøíûé âõîä: {responseString}");

                // ÏÀĞÑÈÌ ÄÀÍÍÛÅ ÏÎËÜÇÎÂÀÒÅËß ÄËß AccountManager
                var userResponse = JsonUtility.FromJson<UserLoginResponse>(responseString);

                if (userResponse != null)
                {
                    // ÑÎÕĞÀÍßÅÌ ÄÀÍÍÛÅ ÏÎËÜÇÎÂÀÒÅËß Â AccountManager
                    AccountManager.Instance.SetUserData(userResponse);
                    _requestText.text = "Óñïåøíûé âõîä!";
                    SceneManager.LoadScene(1);
                }
                else
                {
                    _requestText.text = "Âõîä âûïîëíåí!";
                    SceneManager.LoadScene(1);
                }
            }
            else if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                _requestText.text = "Íåâåğíûé òåëåôîí èëè ïàğîëü";
            }
            else
            {
                _requestText.text = $"Îøèáêà: {response.StatusCode}";
                Debug.LogError($"Îøèáêà âõîäà: {response.StatusCode}");
            }
        }
        catch (Exception ex)
        {
            _requestText.text = "Îøèáêà ñîåäèíåíèÿ";
            Debug.LogError($"Auth error: {ex.Message}");
        }
    }

    private void OnDestroy()
    {
        // ÍÅ ÓÍÈ×ÒÎÆÀÅÌ HttpClient, ÎÍ ÍÓÆÅÍ ÄËß ÄĞÓÃÈÕ ÑÖÅÍ
    }
}

// Êëàññ äëÿ äàííûõ âõîäà
[System.Serializable]
public class LoginByPhoneDto
{
    public string phone = string.Empty;
    public string password = string.Empty;
}