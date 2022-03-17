using UnityEngine;
using UnityEngine.SceneManagement;
public class Preload : MonoBehaviour
{
    void Update()
    {
        if (SceneManager.GetActiveScene().isLoaded)
        {
            SceneManager.LoadScene("MainMenu");
            Destroy(gameObject);
        }
    }
}
