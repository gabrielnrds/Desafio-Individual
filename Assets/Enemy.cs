using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class InimigoSeguidor : MonoBehaviour
{
    [Header("Configurações de Movimento")]
    public float velocidade = 3f;
    public float distanciaParaSeguir = 12f; // Alcance para detectar o Player

    private Transform playerTransform;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Animator animator;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        // Localiza o Player na cena pela Tag
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerTransform = playerObj.transform;
        }
    }

    void FixedUpdate()
    {
        if (playerTransform == null) return;

        // Calcula a distância horizontal e total até o Player
        float distanciaX = playerTransform.position.x - transform.position.x;
        float distanciaTotal = Vector2.Distance(transform.position, playerTransform.position);

        // Persegue o Player apenas se estiver dentro do raio de visão
        if (distanciaTotal <= distanciaParaSeguir)
        {
            // Determina a direção (-1 para esquerda, 1 para direita)
            float direcao = Mathf.Sign(distanciaX);

            // Aplica a velocidade horizontal mantendo a gravidade no eixo Y
            rb.linearVelocity = new Vector2(direcao * velocidade, rb.linearVelocity.y);

            // Vira o Sprite conforme o movimento
            if (spriteRenderer != null)
            {
                spriteRenderer.flipX = (direcao < 0);
            }

            // Atualiza o Animator (caso use parâmetro de velocidade)
            if (animator != null)
            {
                animator.SetFloat("Speed", Mathf.Abs(rb.linearVelocity.x));
            }
        }
        else
        {
            // Fica parado no eixo X se o Player estiver longe
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);

            if (animator != null)
            {
                animator.SetFloat("Speed", 0f);
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Se tocar no Player via colisão física, reseta o jogo
        if (collision.gameObject.CompareTag("Player"))
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(
                UnityEngine.SceneManagement.SceneManager.GetActiveScene().name
            );
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Detecção de dano recebido pelo tiro ou mina (que continuam como Triggers)
        if (collision.CompareTag("Projetil") || collision.CompareTag("Mina"))
        {
            Destroy(collision.gameObject);
            Destroy(gameObject);
        }
    }
}