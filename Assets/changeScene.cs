using UnityEngine;
using UnityEngine.SceneManagement;

public class CambiaScena : MonoBehaviour
{
    // Puoi chiamare questo metodo da un pulsante o da un'altra parte del tuo script
    public void CambiaScenaDesiderata(string nomeScena)
    {
        SceneManager.LoadScene(nomeScena);
    }
}