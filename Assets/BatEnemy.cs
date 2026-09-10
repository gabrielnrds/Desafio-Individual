using UnityEngine;

public class BatEnemy : MonoBehaviour
{
    [Header("Configurações de Movimento")]
    public float velocidade = 2.5f;
    private Transform playerTransform;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Busca o Player pela Tag na cena
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerTransform = playerObj.transform;
        }
    }

    void Update()
    {
        if (playerTransform == null) return;

        // Move diretamente em direção ao Player, ignorando paredes e chão
        transform.position = Vector3.MoveTowards(transform.position, playerTransform.position, velocidade * Time.deltaTime);

        // Vira a sprite conforme a direção do movimento
        if (spriteRenderer != null)
        {
            spriteRenderer.flipX = (playerTransform.position.x < transform.position.x);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Se tocar no Player, mata o jogador e reinicia a fase
        if (collision.CompareTag("Player"))
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(
                UnityEngine.SceneManagement.SceneManager.GetActiveScene().name
            );
        }
        // Se for atingido por projétil ou mina, o fantasma morre
        else if (collision.CompareTag("Projetil") || collision.CompareTag("Mina"))
        {
            Destroy(collision.gameObject);
            Destroy(gameObject);
        }
    }
}