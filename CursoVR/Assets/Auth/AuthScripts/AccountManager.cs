using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using UnityEngine;

public class AccountManager : MonoBehaviour
{
    public TokenBody Token { get; private set; }

    public static AccountManager Instance;

    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(this);
    }

    public void SetToken(TokenBody token)
    {
        if(Token == null)
        {
            Token = token;
        }
    }
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
