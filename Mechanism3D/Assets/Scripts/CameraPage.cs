using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraPage : MonoBehaviour
{

    [SerializeField] private Button takePictureButton;
    [SerializeField] private Button cancelButton;
    [SerializeField] private RawImage rawPhotoTaken;


    private void Awake()
    {
        takePictureButton.onClick.AddListener(() => { TakePictureButton(); });
        cancelButton.onClick.AddListener(() => { Hide(); });
    }

    public void Show()
    {
        this.gameObject.SetActive(true);
    }
    public void Hide()
    {
        this.gameObject.SetActive(false);
    }

    public void TakePictureButton()
    {
        TakePicture(512);
    }

    private void TakePicture(int maxSize)
    {
       NativeCamera.Permission permission = NativeCamera.TakePicture((path) =>
       {
           Debug.LogError("Image path: " + path);
           if(path != null)
           {
               // Create texture2D from the captured Image
               Texture2D texture = NativeCamera.LoadImageAtPath(path, maxSize);
               if(texture == null)
               {
                   Debug.Log("Couldn't load texture from " + path);
                   return;
               }
               // add the above image texture to our raw image component
               rawPhotoTaken.texture = texture;
               rawPhotoTaken.gameObject.SetActive(true);
           }
       }, maxSize);

        Debug.Log("Permission result: " + permission);

    }
}
