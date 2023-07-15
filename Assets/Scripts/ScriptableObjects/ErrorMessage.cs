using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ErrorMessage : MonoBehaviour
{
    [SerializeField] List<string> _errorMessageList;
    public Dictionary<ErrorType, string> ErrorMessageDictionary = new();

    private void Start()
    {
        int counter = 0;
        foreach (string message in _errorMessageList)
        {
            ErrorMessageDictionary.Add((ErrorType)counter, message);
            counter++;
        }
    }
}

public enum ErrorType
{
    EmptyCounter,
    CantPlaceHere
}
