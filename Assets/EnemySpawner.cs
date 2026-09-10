using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Prefabs dos Inimigos")]
    public GameObject[] prefabsInimigos; // Adicione o Terrestre e o Fantasma aqui

    [Header("Pontos de Spawn")]
    public Transform[] pontosDeSpawn;

    [Header("Controle de Dificuldade")]
    public float tempoInicialSpawn = 4f;
    public float tempoMinimoSpawn = 0.8f;
    public float fatorDificuldade = 0.93f; // Reduz o intervalo a cada spawn

    private float tempoAtualSpawn;

    void Start()
    {
        tempoAtualSpawn = tempoInicialSpawn;
        StartCoroutine(RotinaSpawn());
    }

    IEnumerator RotinaSpawn()
    {
        while (true)
        {
            yield return new WaitForSeconds(tempoAtualSpawn);

            SpawnInimigoAleatorio();

            // Aumenta a frequência de surgimento dos inimigos
            tempoAtualSpawn = Mathf.Max(tempoMinimoSpawn, tempoAtualSpawn * fatorDificuldade);
        }
    }

    void SpawnInimigoAleatorio()
    {
        if (prefabsInimigos.Length == 0 || pontosDeSpawn.Length == 0) return;

        // Escolhe um tipo de inimigo aleatório da lista (ex: 0 = Terrestre, 1 = Fantasma)
        int indexInimigo = Random.Range(0, prefabsInimigos.Length);
        
        // Escolhe um ponto de origem aleatório
        int indexPonto = Random.Range(0, pontosDeSpawn.Length);

        Instantiate(prefabsInimigos[indexInimigo], pontosDeSpawn[indexPonto].position, Quaternion.identity);
    }
}