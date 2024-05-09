using System;
using UnityEngine;
using System.IO;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.UI;

public class QuizItem
{
    public int Index { get; set; }
    public string Question { get; set; }
    public string[] Options { get; set; }
    
    public string CorrectAnswer { get; set; }
}

public class QuizContainer
{
    public QuizItem[] Questions;
}

public class InteractivePanelLogic : MonoBehaviour
{
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text questionText;
    [SerializeField] private GameObject optionPrefab;
    [SerializeField] private ToggleGroup toggleGroup;
    
    public Color onColor;
    public Color offColor;
    // Start is called before the first frame update
    void Start()
    {
        QuizItem qi = GenerateDateQuestion();
        Shuffle(qi.Options);
        UpdateQuestion(qi);
    }

    private void UpdateQuestion(QuizItem qi)
    {
        ClearPreviousQuestion();
        titleText.text = "Question " + qi.Index;
        questionText.text = qi.Question;
        for (int i = 0; i < qi.Options.Length; i++)
        {
            GameObject temp = Instantiate(optionPrefab, transform);
            temp.GetComponent<Toggle>().group = toggleGroup;
            temp.GetComponent<Toggle>().onValueChanged.AddListener(HandleToggleValueChanged);
            temp.GetComponentInChildren<Text>().text = qi.Options[i];
        }
    }
    
    private void HandleToggleValueChanged(bool isOn)
    {
        foreach (Toggle toggle in toggleGroup.GetComponentsInChildren<Toggle>())
        {
            if (toggle.isOn)
            {
                toggle.GetComponentInChildren<Image>().color = onColor;
            }
            else
            {
                toggle.GetComponentInChildren<Image>().color = offColor;
            }
        }
    }
    
    private void ClearPreviousQuestion()
    {
        // Get all child components with the tag "QuizOption"
        foreach (Transform child in transform)
        {
            if (child.CompareTag("QuizOption"))
            {
                child.GetComponent<Toggle>().onValueChanged.RemoveListener(HandleToggleValueChanged);
                Destroy(child.gameObject);
            }
        }
    }
    
    // Reads a JSON file and returns a QuizContainer object
    public QuizContainer LoadQuiz(string filePath)
    {
        try
        {
            // Read all text from the file
            string jsonContent = File.ReadAllText(filePath);

            // Deserialize the JSON content to a QuizContainer object
            QuizContainer quiz = JsonUtility.FromJson<QuizContainer>(jsonContent);

            return quiz;
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to load quiz data: " + e.Message);
            return null;
        }
    }
    
    // Generates a quiz item with today's date and closely neighboring dates
    public QuizItem GenerateDateQuestion()
    {
        QuizItem quizItem = new QuizItem();
        quizItem.Index = 1;
        quizItem.Question = "What is the date today?";
        quizItem.Options = new string[4];

        // Get today's date
        DateTime today = DateTime.Today;
        // Randomly decide to go backwards or forwards
        System.Random rnd = new System.Random();
        int direction = rnd.Next(0, 2) * 2 - 1;

        // Assign the dates
        for (int i = 0; i < 4; i++)
        {
            quizItem.Options[i] = today.AddDays(i * direction).ToString("MM/dd/yyyy");
        }

        quizItem.CorrectAnswer = today.ToString("MM/dd/yyyy");
        return quizItem;
    }
    
    private void Shuffle(string[] options)
    {
        System.Random rnd = new System.Random();
        for (int i = options.Length - 1; i > 0; i--)
        {
            int j = rnd.Next(i + 1);
            string temp = options[i];
            options[i] = options[j];
            options[j] = temp;
        }
    }
}
