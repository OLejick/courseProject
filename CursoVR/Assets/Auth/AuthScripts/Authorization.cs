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
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public enum UserRole
{
    Common,
    Organization,
    Admin
}

public class Authorization : MonoBehaviour
{
    [Header("InputFields")]
    [SerializeField] private TMP_InputField _usernameInputField;
    [SerializeField] private TMP_InputField _passwordInputField;

    [SerializeField] private TextMeshProUGUI _requestText;

    public async void Auth()
    {
        await AuthAsync();
    }

    public async Task AuthAsync()
    {
        Student student = new Student(_usernameInputField.text, _passwordInputField.text);

        _requestText.text = "";

        using var client = new HttpClient()
        {
            BaseAddress = new Uri("https://0435-176-28-64-201.ngrok-free.app/api/"),
        };
        client.DefaultRequestHeaders.Add("ngrok-skip-browser-warning", "69420");

        var s = JsonUtility.ToJson(student);
        var content = new StringContent(s, Encoding.UTF8, MediaTypeNames.Application.Json);
        var response = await client.PostAsync("signin", content);
        TokenBody? tokenBody = null;
        if (response.StatusCode == HttpStatusCode.OK)
        {
            var tokenBodyString = await response.Content.ReadAsStringAsync();
            tokenBody = JsonUtility.FromJson<TokenBody>(tokenBodyString);

            AccountManager.Instance.SetToken(tokenBody);

            SceneManager.LoadScene(1);
        }
        else
        {
            _requestText.text = "Неверные данные";
        }
    }
}

public class Student
{
    public string email;
    public string password;

    public Student(string name, string pass)
    {
        email = name;
        password = pass;
    }
}

