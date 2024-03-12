using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FeedbackForm : MonoBehaviour
{
    ItemDetails itemDetails;
    [SerializeField] private GameObject cameraObject;
    [SerializeField] private CameraPage cameraPage;
    [SerializeField] private string partName;
    [SerializeField] private string dateOfInspection;
    [SerializeField] private float crackLength;
    [SerializeField] private string comments;
    
    [SerializeField] private TMP_InputField dateOfInspectionInput;
    [SerializeField] private TMP_InputField crackLengthInput;
    [SerializeField] private TMP_InputField commentsInput;

    [SerializeField] private Button submitButton;
    [SerializeField] private Button cancelButton;
    [SerializeField] private Button photoButton;

    private void Awake()
    {
        cameraPage = cameraObject.GetComponent<CameraPage>();
        submitButton.onClick.AddListener(() => { SaveData(); });  
        cancelButton.onClick.AddListener(() => { HideFeedbackPage(); });
        photoButton.onClick.AddListener(() => {  cameraPage.Show(); });
    }


    public void ShowFeedbackPage()
    {
        this.gameObject.SetActive(true);
    }
    public void HideFeedbackPage()
    {
        this.gameObject.SetActive(false);
    }

    public void GetPartGameObject(GameObject selectedPart)
    {
        itemDetails = selectedPart.GetComponent<ItemDetails>();
        if(itemDetails.name == "Cube.031")
        {
            LoadData();
        }
    }

    public void SaveData()
    {
        partName = itemDetails.name;
        dateOfInspection = dateOfInspectionInput.text;
        crackLength = float.Parse(crackLengthInput.text);
        comments = commentsInput.text;

        itemDetails.SetDateOfInspection(dateOfInspection);
        itemDetails.SetCrackLength(crackLength);
        itemDetails.SetComments(comments);
        HideFeedbackPage();

        PlayerPrefs.SetString("itemName", partName);
        PlayerPrefs.SetString("date", dateOfInspection);
        PlayerPrefs.SetString("comments", comments);
        PlayerPrefs.SetFloat("crackLength", crackLength);

    }


    public void LoadData()
    {
        partName = PlayerPrefs.GetString("itemName", partName);
        dateOfInspection = PlayerPrefs.GetString("date", dateOfInspection);
        comments = PlayerPrefs.GetString("comments", comments);
        crackLength = PlayerPrefs.GetFloat("crackLength", crackLength);

        itemDetails.name = partName;
        dateOfInspectionInput.text = dateOfInspection;
        crackLengthInput.text = crackLength.ToString();
        commentsInput.text = comments;
    }








}
