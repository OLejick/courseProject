using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using UnityEngine;

public class AccountManager : MonoBehaviour
{
    public TokenBody? Token { get; private set; }
    //public Organization? CurrentOrganization { get; private set; }

    public static AccountManager Instance;

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
}
[System.Serializable]
public class LoginByPhoneDto
{
    public string phone = string.Empty;
    public string password = string.Empty;
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
    public int id;
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
