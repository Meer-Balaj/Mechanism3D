using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDetails : MonoBehaviour
{
    public string partName;
    public string dateOfInspection;
    public float crackLength;
    public string comments;

    private void Start()
    {
        partName = this.gameObject.name;
    }

    public void SetDateOfInspection( string date )
    {
        dateOfInspection = date;
    } 
    public void SetCrackLength(float length)
    {
        crackLength = length;
    } 
    public void SetComments( string com )
    {
        comments = com;
    }

}
