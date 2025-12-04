using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class AccountManager : MonoBehaviour
{
    public TokenBody? Token { get; private set; }
    public Organization? CurrentOrganization { get; private set; }
    public VrExam? CurrentText { get; private set; }
    public UserLoginResponse? UserData { get; private set; }

    public static AccountManager Instance = null;

    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(this);
    }

    public void SetToken(TokenBody token)
    {
        if (Token == null)
        {
            Token = token;
        }
    }
    public void SetUserData(UserLoginResponse userData)
    {
        UserData = userData;
        Debug.Log($"User data set: {userData.name} {userData.surname}, Organizations: {userData.employers?.Count ?? 0}");

    }
}
[System.Serializable]
public class LoginByPhoneDto
{
    public string phone = string.Empty;
    public string password = string.Empty;
}

[System.Serializable]
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

public enum UserRole
{
    Common,
    Organization,
    Admin
}
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
public class Employer
{
    public int id;
    public Organization organization = new Organization();
    public Role role = new Role();
    public Position? position;
    public string email = string.Empty;
}

[Serializable]
public class Role
{
    public string name = string.Empty;
}
[Serializable]
public class Postition
{
    public string name = string.Empty;
}

[Serializable]
public class VrExam
{
    public int id;
    public string name = string.Empty;
    public string description = string.Empty;
    public string srart_date = string.Empty;
    public string end_date = string.Empty;
}

[Serializable]
public class Organization
{
    public int id;
    public string name = string.Empty;
    public string description = string.Empty;
}

[Serializable]
public class TokenPair
{
    public string accessToken;
    public string refreshToken;
}

[Serializable]
public class TokenBody
{
    public TokenPair tokenPair;
    public UserRole role;
}
