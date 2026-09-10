using UnityEngine;

public class Projetil : MonoBehaviour
{
    public float velocidade = 15f;
    public float tempoVida = 3f;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        Destroy(gameObject, tempoVida);
    }

    public void Setup(Vector2 direcao)
    {
        rb = GetComponent<Rigidbody2D>();
        
        // Aplica a velocidade no vetor recebido
        rb.linearVelocity = direcao * velocidade;

        // Calcula o ângulo em graus para girar a sprite da bala na direção do vetor
        float angulo = Mathf.Atan2(direcao.y, direcao.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angulo);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Inimigo"))
        {
            Destroy(collision.gameObject);
            Destroy(gameObject);
        }
        else if (collision.CompareTag("Ground"))
        {
            Destroy(gameObject);
        }
    }
}