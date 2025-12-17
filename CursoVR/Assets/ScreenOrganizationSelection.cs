using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScreenOrganizationSelection : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Transform _organizationContrainer = null!;
    [SerializeField] private GameObject _organizationButtonPrefab = null!;
    [SerializeField] private TextMeshProUGUI _statusText = null!;
    [SerializeField] private Button _backButton = null!;
    [SerializeField] private Button _refreshButton = null!;

    private List<GameObject> _organizationButtons = new List<GameObject>();
    private HttpClient _httpClient = null!;

    private void Start()
    {
        if (AccountManager.Instance == null)
        {
            Debug.LogError("AccountManager.Instance is NULL!");
            _statusText.text = "Ошибка";
        }
    }
}
