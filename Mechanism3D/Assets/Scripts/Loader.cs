using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class Loader 
{
    public enum Scene
    {
        GameScene,
        LoginScene

    }
    private static Scene targetScene;

    public static void Load(Scene targetScene)
    {
        Loader.targetScene = targetScene;
        SceneManager.LoadScene(targetScene.ToString());
        //SceneManager.LoadScene(Scene.LoadingScene.ToString());

    }

    /*public static void LoaderCallback()
    {
        SceneManager.LoadScene(targetScene.ToString());
    }*/
}
