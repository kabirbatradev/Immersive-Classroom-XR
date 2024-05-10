using System;
using UnityEngine;
using System.IO;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.UI;

[System.Serializable]
public class QuizContainer
{
    public QuizItem[] Questions;
}

[System.Serializable]
public class QuizItem
{
    public int Index;
    public string Question;
    public string[] Options;
    public string CorrectAnswer;
}


public class InteractivePanelLogic : MonoBehaviour
{
    [SerializeField] private GameObject boundObject;
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text questionText;
    [SerializeField] private Toggle raiseHand;
    [SerializeField] private GameObject optionPrefab;
    [SerializeField] private ToggleGroup toggleGroup;

    public Sprite raiseHandRaised;
    public Sprite raiseHandNotRaised;
    
    public Color optionOnColor;
    public Color optionOffColor;

    public Color optionCorrectColor;
    public Color optionWrongColor;

    private QuizContainer qc;
    private QuizItem qi;

    void Start()
    {
        raiseHand.onValueChanged.AddListener(UpdateRaiseHandStatus);
        TextAsset file = Resources.Load("Quiz") as TextAsset;
        qc = LoadQuiz(file);
        qi = qc.Questions[0];
        // qi = GenerateDateQuestion();
        Shuffle(qi.Options);
        UpdateQuestion(qi);
        UpdateClippingBounds();
    }

    private void UpdateQuestion(QuizItem qi)
    {
        ClearPreviousQuestion();
        titleText.text = "Question " + qi.Index;
        questionText.text = qi.Question;
        for (int i = 0; i < qi.Options.Length; i++)
        {
            GameObject temp = Instantiate(optionPrefab, transform);
            // Calculate the desired sibling index to make it the second-to-last child
            int secondToLastIndex = Mathf.Max(transform.childCount - 2, 0);
            // Set the new object's sibling index
            temp.transform.SetSiblingIndex(secondToLastIndex);
            temp.GetComponent<Toggle>().group = toggleGroup;
            temp.GetComponent<Toggle>().onValueChanged.AddListener(HandleToggleValueChanged);
            temp.GetComponentInChildren<Text>().text = qi.Options[i];
        }
    }
    
    private void HandleToggleValueChanged(bool isOn)
    {
        foreach (Toggle toggle in toggleGroup.GetComponentsInChildren<Toggle>())
        {
            if (toggle.group == toggleGroup)
            {
                toggle.GetComponentInChildren<Image>().color = toggle.isOn ? optionOnColor : optionOffColor;
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

    public void GradeQuestion()
    {
        foreach (Toggle toggle in toggleGroup.GetComponentsInChildren<Toggle>())
        {
            if (toggle.group == toggleGroup)
            {
                if (toggle.isOn)
                {
                    bool isCorrect = toggle.GetComponentInChildren<Text>().text == qi.CorrectAnswer;
                    toggle.GetComponentInChildren<Image>().color = isCorrect ? optionCorrectColor : optionWrongColor;
                }
            }
        }
    }
    
    // Reads a JSON file and returns a QuizContainer object
    public QuizContainer LoadQuiz(TextAsset file)
    {
        try
        {
            // Read all text from the file
            string jsonContent = file.ToString();
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

    private void UpdateRaiseHandStatus(bool isOn)
    {
        raiseHand.GetComponent<Image>().sprite = isOn ? raiseHandRaised : raiseHandNotRaised;
        if (isOn)
        {
            // Other logic of calling out to the instructor
        }
    }
    
    // Update clipping bounds so that the cursor isn't shown outside of UI panel
    private void UpdateClippingBounds()
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
        // Get the corners of the RectTransform in world space
        Vector3[] corners = new Vector3[4];
        GetComponent<RectTransform>().GetWorldCorners(corners);

        // Calculate the bounding box that contains all corners
        Bounds bounds = new Bounds(corners[0], Vector3.zero);
        for (int i = 1; i < corners.Length; i++)
        {
            bounds.Encapsulate(corners[i]);
        }

        Vector3 lossyScale = boundObject.transform.lossyScale;
        boundObject.transform.localScale = new Vector3(bounds.size.x / lossyScale.x, bounds.size.y / lossyScale.y, boundObject.transform.localScale.z);
    }
}

