using UnityEngine;
using UnityEngine.AI; // Importante para usar NavMeshAgent
using System.Collections.Generic;

public class AIDiretora : MonoBehaviour
{
    public GameObject inimigo;
    public Transform[] areaA_Waypoints;
    public Transform[] areaB_Waypoints;
    public Transform areaA_Centro;
    public Transform areaB_Centro;
    public float tempoMaximoNaArea = 30f;
    private float tempoNaAreaAtual = 0f;

    private Transform[] waypointsAtuais;
    private string jogadorNaArea = "A";
    private NavMeshAgent agent; // Adicionado para controle do NavMeshAgent

    void Start()
    {
        agent = inimigo.GetComponent<NavMeshAgent>(); // Obtém o NavMeshAgent
        SetWaypoints(areaA_Waypoints);
    }

    void Update()
    {
        tempoNaAreaAtual += Time.deltaTime;

        if (tempoNaAreaAtual >= tempoMaximoNaArea)
        {
            if (jogadorNaArea == "A")
            {
                MoverInimigoParaAreaA();
            }
            else
            {
                MoverInimigoParaAreaB();
            }
        }
    }

    public void JogadorEntrouNaArea(string area)
    {
        jogadorNaArea = area;
        tempoNaAreaAtual = 0f;
    }

    public void JogadorSaiuDaArea(string area)
    {
        // Opcional
    }

    void SetWaypoints(Transform[] novosWaypoints)
    {
        waypointsAtuais = novosWaypoints;
        foreach (Transform wp in waypointsAtuais)
        {
            wp.gameObject.SetActive(true);
        }
        tempoNaAreaAtual = 0f;
    }

    void MoverInimigoParaAreaA()
    {
        if (agent != null)
        {
            agent.ResetPath(); // Para qualquer movimento anterior
            agent.enabled = false; // Desativa o NavMeshAgent
        }

        inimigo.transform.position = areaA_Centro.position;
        SetWaypoints(areaA_Waypoints);

        if (agent != null)
        {
            agent.enabled = true; // Reativa o NavMeshAgent
            agent.SetDestination(waypointsAtuais[0].position); // Define novo destino
        }
    }

    void MoverInimigoParaAreaB()
    {
        if (agent != null)
        {
            agent.ResetPath();
            agent.enabled = false;
        }

        inimigo.transform.position = areaB_Centro.position;
        SetWaypoints(areaB_Waypoints);

        if (agent != null)
        {
            agent.enabled = true;
            agent.SetDestination(waypointsAtuais[0].position);
        }
    }
}
