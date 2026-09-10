using UnityEngine;

public class Mine : MonoBehaviour
{
    [Header("Configurações da Explosão")]
    public float raioExplosao = 2.5f;
    public LayerMask camadasAfetadas; // Marque Player e Inimigo no Inspector
    public float tempoAteDestruir = 0.5f; // Tempo para a animação de explosão tocar

    [Header("Componentes")]
    private Animator animator;
    private Collider2D colisorMina;
    private bool detonada = false;

    void Start()
    {
        animator = GetComponent<Animator>();
        colisorMina = GetComponent<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Se a mina for pisada por um Inimigo ou pelo Player (e ainda não tiver detonado)
        if (!detonada && (collision.CompareTag("Inimigo")))
        {
            Explodir();
        }
    }

    void Explodir()
    {
        detonada = true;

        // Desativa o colisor para não disparar novamente
        if (colisorMina != null) colisorMina.enabled = false;

        // Dispara a animação de explosão
        if (animator != null)
        {
            animator.SetTrigger("Explodir");
        }

        // Detecta todos os objetos na área circular/esférica
        Collider2D[] objetosAtingidos = Physics2D.OverlapCircleAll(transform.position, raioExplosao, camadasAfetadas);

        foreach (Collider2D obj in objetosAtingidos)
        {
            // Se for inimigo, destrói o objeto
            if (obj.CompareTag("Inimigo"))
            {
                Destroy(obj.gameObject);
            }
            // Se atingir o Player na área, recarrega a fase (morte do jogador)
            else if (obj.CompareTag("Player"))
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene(
                    UnityEngine.SceneManagement.SceneManager.GetActiveScene().name
                );
            }
        }

        // Destrói o GameObject da mina após o tempo da animação
        Destroy(gameObject, tempoAteDestruir);
    }

    // Desenha o raio da explosão na janela Scene para facilitar o ajuste visual
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, raioExplosao);
    }
}