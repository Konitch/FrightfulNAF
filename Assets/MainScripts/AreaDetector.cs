using UnityEngine;

public class AreaDetector : MonoBehaviour  // Garante que herda de MonoBehaviour
{
    public string areaNome; // Nome da área: "A" ou "B"
    private AIDiretora aiDiretora;

    void Start()
    {
        aiDiretora = FindObjectOfType<AIDiretora>(); // Encontra a IA Diretora
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // Verifica se o jogador entrou
        {
            aiDiretora.JogadorEntrouNaArea(areaNome);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) // Verifica se o jogador saiu
        {
            aiDiretora.JogadorSaiuDaArea(areaNome);
        }
    }
}
