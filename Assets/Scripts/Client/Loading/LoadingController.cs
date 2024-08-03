using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using Michsky.MUIP;

public class LoadingController : Singleton<LoadingController> 
{
    [SerializeField] private TMP_Text _loadingText;
    [SerializeField] private Canvas _canvas;
    [SerializeField] private ProgressBar _progressBar;


    private void Start()
    {
        AnimateLoadingText();
    }

    public void Show()
    {
        _canvas.gameObject.SetActive(true);
    }

    public void Hide()
    {
        _canvas.gameObject.SetActive(false);
    }

    public void SetProgress(float value)
    {
        _progressBar.currentPercent = value;
    }

    private async void AnimateLoadingText()
    {
        string finalText = "LOADING...";
        string currentText = "";

        while (true)
        {
            for (int i = 0; i <= finalText.Length; i++)
            {
                currentText = finalText.Substring(0, i);
                _loadingText.text = currentText;

                await Task.Delay(120);
                if(i >= 7)
                    await Task.Delay(1000);

                if (i == finalText.Length)
                {
                    currentText = "";
                    _loadingText.text = currentText;
                    await Task.Delay(250);
                }
            }
        }
        
    }
}


