using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using UnityEngine;

#nullable enable

public class AccountManager : MonoBehaviour
{
    public TokenBody? Token { get; private set; }
    public Organization? CurrentOrganization { get; private set; }
    public VrExam? CurrentTest { get; private set; }
    public UserLoginResponse? UserData { get; private set; } // ДОБАВЛЕНО

    public static AccountManager Instance = null!;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetToken(TokenBody token)
    {
        Token = token;
        Debug.Log($"Token set: {token.tokenPair.accessToken}, Role: {token.role}");
    }

    // ДОБАВЛЕНО: метод для сохранения данных пользователя
    public void SetUserData(UserLoginResponse userData)
    {
        UserData = userData;
        Debug.Log($"User data set: {userData.name} {userData.surname}, Organizations: {userData.employers?.Count ?? 0}");
    }

    public void SetCurrentOrganization(Organization organization)
    {
        CurrentOrganization = organization;
        Debug.Log($"Organization set: {organization.name} (ID: {organization.id})");
    }

    public void SetCurrentTest(VrExam test)
    {
        CurrentTest = test;
        Debug.Log($"Test set: {test.name} (ID: {test.id})");
    }

    public void ClearSelection()
    {
        CurrentOrganization = null;
        CurrentTest = null;
        Debug.Log("Selection cleared");
    }
}

[Serializable]
public class TokenPair
{
    public string accessToken = string.Empty;
    public string refreshToken = string.Empty;
}

[Serializable]
public class TokenBody
{
    public TokenPair tokenPair = new TokenPair();
    public UserRole role;
}

public enum UserRole
{
    Common,
    Organization,
    Admin
}

// ДОБАВЛЕНО: классы для данных пользователя
[Serializable]
public class UserLoginResponse
{
    public int id;
    public string phone = string.Empty;
    public string name = string.Empty;
    public string surname = string.Empty;
    public string patroname = string.Empty;
    public string birthday = string.Empty;
    public bool is_superadmin;
    public string created_at = string.Empty;
    public string updated_at = string.Empty;
    public List<Employer> employers = new List<Employer>();
}
[Serializable]
public class UserGlobalProfile
{
    public int id;
    public string phone = string.Empty;
    public string name = string.Empty;
    public string surname = string.Empty;
    public string patroname = string.Empty;
    public string birthday = string.Empty;
    public bool is_superadmin;
    public string created_at = string.Empty;
    public string updated_at = string.Empty;
    public List<Employer> employers = new List<Employer>();
}


[Serializable]
public class Employer
{
    public int id;
    public Organization organization = new Organization();
    public Role role = new Role();
    public Position? position;
    public string email = string.Empty;
}

[Serializable]
public class Organization
{
    public int id;
    public string name = string.Empty;
    public string description = string.Empty;
}

[Serializable]
public class Role
{
    public string name = string.Empty;
}

[Serializable]
public class Position
{
    public string name = string.Empty;
}

[Serializable]
public class VrExam
{
    public int id;
    public string name = string.Empty;
    public string description = string.Empty;
    public string start_date = string.Empty;
    public string end_date = string.Empty;
}
// Добавьте эти классы в любой из ваших существующих файлов, например в ScreenTestSelection.cs

[Serializable]
public class VrExamResponse
{
    public List<VrExam> exams = new List<VrExam>();
}

[Serializable]
public class VrAttemptResponse
{
    public int attempt_id;
    public string created_at = string.Empty;
    public string updated_at = string.Empty;
}

[Serializable]
public class AlternativeAttemptResponse
{
    public int id;
    public string created_at = string.Empty;
    public string updated_at = string.Empty;
}

[Serializable]
public class ComplexAttemptResponse
{
    public AttemptData data = new AttemptData();
}

[Serializable]
public class AttemptData
{
    public int attempt_id;
    public string created_at = string.Empty;
    public string updated_at = string.Empty;
}
[Serializable]
public class LoginOrganizationDto
{
    public int employer_id;
    public int organization_id;
}
// ДОБАВЬТЕ ЭТИ КЛАССЫ В AccountManager.cs
[Serializable]
public class VrExamDataResponse
{
    public List<VrExam> data = new List<VrExam>();
    public int count;
}
//// КЛАССЫ ДЛЯ ПАРСИНГА ОТВЕТОВ API
[Serializable]
public class VrExamListResponse
{
    public List<VrExam> data = new List<VrExam>();
    public int count;
}
[Serializable]
public class UpdateEmployerVRExamAttemptDto
{
    public int attempt_id;
    public int score;
}