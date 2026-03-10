using UnityEngine;

public class FourDigitCodeManager : MonoBehaviour
{
    public UVManager[] generators;

    public string CurrentCode { get; private set; }

    void Start()
    {
        GenerateCode();
        gameObject.SetActive(false);
    }
    public string GetCode()
    {
        return CurrentCode;
    }

    public void GenerateCode()
    {
        CurrentCode = "";

        foreach (var generator in generators)
        {
            generator.Generate();
            CurrentCode += generator.SelectedNumber.ToString();
        }

        Debug.Log("Generated Code: " + CurrentCode);
    }
}