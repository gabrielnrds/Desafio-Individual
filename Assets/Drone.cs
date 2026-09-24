using UnityEngine;

public class Drone : MonoBehaviour
{
    public float velocidade = 4f;
    public float limiteEsquerda = -12f;
    public float limiteDireita = 12f;
    public GameObject pacoteMunicaoPrefab;

    private int direcao = 1; // 1 = direita, -1 = esquerda

    void Update()
    {
        transform.Translate(Vector3.right * direcao * velocidade * Time.deltaTime);

        if (transform.position.x >= limiteDireita) direcao = -1;
        else if (transform.position.x <= limiteEsquerda) direcao = 1;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Se for atingido por um projétil do jogador
        if (collision.CompareTag("Projetil"))
        {
            if (ComboManager.Instance != null)
            {
                ComboManager.Instance.RegistraAcerto();
            }

            // Instancia o item de munição na posição onde o drone caiu
            if (pacoteMunicaoPrefab != null)
            {
                Instantiate(pacoteMunicaoPrefab, transform.position, Quaternion.identity);
            }

            Destroy(collision.gameObject);
            Destroy(gameObject);
        }
    }
}