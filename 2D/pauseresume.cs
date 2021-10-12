using UnityEngine;

public class pauseresume : MonoBehaviour
{
    // Start is called before the first frame 
    [SerializeField] GameObject canvas;
    [SerializeField] GameObject pauseButton;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        { 
            Time.timeScale = 0;
            resumemenu();
        }
    }
   public void pause()
    {
        pauseButton.SetActive(false);
         Time.timeScale = 0;
        resumemenu();
    }
    void resumemenu()
    {
        canvas.SetActive(true);
    }

   public void resume()
    {
        canvas.SetActive(false);
        Time.timeScale = 1;
        pauseButton.SetActive(true);

    }

    public void exit()
    {
        Application.Quit();
    }
}
