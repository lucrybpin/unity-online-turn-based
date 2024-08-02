using System.Threading.Tasks;
using UnityEngine;
using static ScreenManager;

public class SplashScreenController : MonoBehaviour
{
    async void Start()
    {
        await Task.Delay(4000);
        ScreenManager.Instance.LoadScene(SceneID.CharacterSelection);
    }

}
