using UnityEngine;
using TMPro;

public class CodeInputManager : MonoBehaviour
{
    [Header("References")]
    public FourDigitCodeManager codeManager;
    public TMP_InputField inputField;

    [Header("Settings")]
    public int maxLength = 4;

    private string currentInput = "";

    // Wordt aangeroepen door nummer knoppen
    public void PressNumber(int number)
    {
        if (currentInput.Length >= maxLength)
            return;

        currentInput += number.ToString();
        UpdateDisplay();
    }

    // Clear knop
    public void ClearInput()
    {
        currentInput = "";
        UpdateDisplay();
    }

    // Enter knop
    public void SubmitCode()
    {
        if (currentInput == codeManager.GetCode())
        {
            Debug.Log("Correct Code!");
            OnCorrectCode();
        }
        else
        {
            Debug.Log("Wrong Code!");
            OnWrongCode();
        }
    }

    void UpdateDisplay()
    {
        inputField.text = currentInput;
    }

    void OnCorrectCode()
    {
        // Hier open je bijvoorbeeld een deur
        Debug.Log("Door Opened!");
    }

    void OnWrongCode()
    {
        currentInput = "";
        UpdateDisplay();
    }
}
