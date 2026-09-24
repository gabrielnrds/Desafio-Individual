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
        rb.linearVelocity = direcao * velocidade;

        float angulo = Mathf.Atan2(direcao.y, direcao.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angulo);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Inimigo"))
        {
            // Acertou o inimigo: incrementa o combo!
            if (ComboManager.Instance != null)
            {
                ComboManager.Instance.RegistraAcerto();
            }

            Destroy(collision.gameObject);
            Destroy(gameObject);
        }
        else if (collision.CompareTag("Ground"))
        {
            // Errou o tiro no chão: quebra a sequência de combo
            if (ComboManager.Instance != null)
            {
                ComboManager.Instance.ResetarCombo();
            }

            Destroy(gameObject);
        }
    }
}