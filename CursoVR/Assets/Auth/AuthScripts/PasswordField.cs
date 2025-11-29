using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PasswordField : MonoBehaviour
{
    [SerializeField] private TMP_InputField _passwordField;

    [SerializeField] private Image _setVisiblePasswordButton;

    [SerializeField] private Sprite _openedEye;
    [SerializeField] private Sprite _closedEye;

    [SerializeField] private bool _passwordVisible;

    public void SetVisiblePassword() 
    {
        if (_passwordVisible)
        {
            _passwordField.contentType = TMP_InputField.ContentType.Password;
            _setVisiblePasswordButton.sprite = _closedEye;
            _passwordVisible = false;
        }
        else
        {
            _passwordField.contentType = TMP_InputField.ContentType.Standard;
            _setVisiblePasswordButton.sprite = _openedEye;
            _passwordVisible = true;
        }

        _passwordField.ForceLabelUpdate();
    }
}
