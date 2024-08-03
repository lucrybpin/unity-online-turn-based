using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScreenManager : Singleton<ScreenManager> 
{
    public enum SceneID
    {
        Splash,
        CharacterSelection,
        Sandbox
    }

    public async void LoadScene(SceneID sceneId)
    {
        AsyncOperation scene = SceneManager.LoadSceneAsync(sceneId.ToString());
        scene.allowSceneActivation = false;
        LoadingController.Instance.Show();
        do
        {
            LoadingController.Instance.SetProgress(scene.progress);
            await Task.Delay(10000);//TODO: REMOVE, JUST TO CHECK IF IT IS INCREASING BAR
        } while (scene.progress < 0.9f);

        scene.allowSceneActivation = true;
        LoadingController.Instance.Hide();
    }
}
