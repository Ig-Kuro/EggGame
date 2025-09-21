using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoaderTrigger : MonoBehaviour
{
    public string sceneToLoad;
    public string playerTag = "Player";

    private void OnTriggerEnter(Collider other)
    {
        // Verifica se o objeto que colidiu tem a tag de jogador
        if (other.CompareTag(playerTag))
        {
            Debug.Log("Carregando a cena: " + sceneToLoad);
            // Carrega a cena especificada
            SceneManager.LoadScene(sceneToLoad);
        }
    }
}