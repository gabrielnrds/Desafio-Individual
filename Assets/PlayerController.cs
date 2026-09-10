using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movimentação Horizontal")]
    public float moveSpeed = 5f;

    [Header("Mecânica de Pulo")]
    public float jumpForce = 12f;
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    [Header("Ação: Tiro")]
    public GameObject projetilPrefab;
    public Transform pontoDisparo;
    public float cooldownTiro = 0.3f;
    private float ultimoTiro;
    public float alturaTiro = 0.5f; // <--- ADICIONE ESTA LINHA (Ajusta a altura do peito)

    [Header("Ação: Dash")]
    public float velocidadeDash = 20f;
    public float duracaoDash = 0.2f;
    public float cooldownDash = 1f;
    private bool podeFazerDash = true;
    private bool estaEmDash;

    [Header("Ação: Mina")]
    public GameObject minaPrefab;
    public Transform pontoMina; // Ponto nos pés onde a mina é colocada
    public float cooldownMina = 2f;
    private float ultimaMina;

    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    // Distância do ponto de disparo em relação ao centro do player
    public float raioPontoDisparo = 0.8f;

    private float horizontalInput;
    private bool isGrounded;
    private bool jumpRequested;
    private float direcaoOlhando = 1f; // 1 = Direita, -1 = Esquerda

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        // Se estiver executando o Dash, bloqueia os outros comandos
        if (estaEmDash) return;

        // 1. Leitura de Entrada de Movimento
        horizontalInput = Input.GetAxisRaw("Horizontal");

       // Atualiza a direção que o personagem está olhando e ajusta o PontoDisparo
        if (horizontalInput > 0)
        {
            direcaoOlhando = 1f;
            spriteRenderer.flipX = false;

            // Move o ponto de disparo para a direita do personagem
            if (pontoDisparo != null)
            {
                pontoDisparo.localPosition = new Vector3(Mathf.Abs(pontoDisparo.localPosition.x), pontoDisparo.localPosition.y, 0f);
            }
        }
        else if (horizontalInput < 0)
        {
            direcaoOlhando = -1f;
            spriteRenderer.flipX = true;

            // Espelha o ponto de disparo para a esquerda do personagem
            if (pontoDisparo != null)
            {
                pontoDisparo.localPosition = new Vector3(-Mathf.Abs(pontoDisparo.localPosition.x), pontoDisparo.localPosition.y, 0f);
            }
        }

        // 2. Checagem de Chão
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        if (animator != null)
        {
            animator.SetFloat("Speed", Mathf.Abs(horizontalInput));
            animator.SetBool("OnGround", isGrounded);

            // Identifica as teclas de mira ativas no frame
            bool segurandoW = Input.GetKey(KeyCode.W);
            bool segurandoQ = Input.GetKey(KeyCode.Q);
            bool segurandoE = Input.GetKey(KeyCode.E);

            // Define a mira vertical e diagonal para o Animator
            animator.SetFloat("AimY", segurandoW || segurandoQ || segurandoE ? 1f : 0f);
            animator.SetBool("IsDiagonal", (segurandoQ || segurandoE) && isGrounded);
        }

        // 3. Pulo (Barra de Espaço)
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            jumpRequested = true;
        }

        // 4. Tecla do Dash (Shift Esquerdo ou Botão de Fire2)
        if (Input.GetKeyDown(KeyCode.LeftShift) && podeFazerDash)
        {
            StartCoroutine(ExecutarDash());
        }

        // 5. Tecla do Tiro (Tecla J ou Botão de Fire1 / Clique Esquerdo)
        if ((Input.GetKeyDown(KeyCode.J) || Input.GetMouseButtonDown(0)) && Time.time >= ultimoTiro + cooldownTiro)
        {
            Atirar();
            ultimoTiro = Time.time;
        }

        // 6. Tecla para Plantar Mina (Tecla F)
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (Time.time >= ultimaMina + cooldownMina && isGrounded)
            {
                PlantarMina();
                ultimaMina = Time.time;
            }
        }
    }

    void FixedUpdate()
    {
        if (estaEmDash) return;

        rb.linearVelocity = new Vector2(horizontalInput * moveSpeed, rb.linearVelocity.y);

        if (jumpRequested)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            jumpRequested = false;
        }
    }

    void Atirar()
    {
        if (projetilPrefab == null || pontoDisparo == null) return;

        // Calcula o vetor de direção com base nas regras do jogo
        Vector2 direcaoTiro = ObterDirecaoTiro();

        // Se a combinação de teclas for inválida para o momento, não atira
        if (direcaoTiro == Vector2.zero) return;

        // Calcula a posição do tiro considerando a altura do peito no eixo Y
        Vector3 offsetLocal = (Vector3)direcaoTiro * raioPontoDisparo;
        offsetLocal.y += alturaTiro;

        pontoDisparo.localPosition = offsetLocal;

        // Instancia a bala
        GameObject bala = Instantiate(projetilPrefab, pontoDisparo.position, Quaternion.identity);
        Projetil scriptBala = bala.GetComponent<Projetil>();

        if (scriptBala != null)
        {
            scriptBala.Setup(direcaoTiro);
        }
        
        // Dispara a animação correspondente ao tipo de tiro
        if (animator != null)
        {
            animator.SetTrigger("IsShooting"); 
            // Ou se usar Bool: StartCoroutine(ResetShootingBool());
        }
    }

    Vector2 ObterDirecaoTiro()
    {
        bool segurandoW = Input.GetKey(KeyCode.W);
        bool segurandoQ = Input.GetKey(KeyCode.Q);
        bool segurandoE = Input.GetKey(KeyCode.E);
        bool estaMovimentando = Mathf.Abs(horizontalInput) > 0.1f;

        // Regra 1: Atirar Para Cima (Tecla W) - APENAS parado (no chão ou no ar)
        if (segurandoW && !estaMovimentando)
        {
            return Vector2.up; // (0, 1)
        }

        // Regra 2: Atirar na Diagonal Esquerda (Tecla Q) - Não permitido no ar
        if (segurandoQ && isGrounded)
        {
            return new Vector2(-1f, 1f).normalized; // (-0.71, 0.71)
        }

        // Regra 3: Atirar na Diagonal Direita (Tecla E) - Não permitido no ar
        if (segurandoE && isGrounded)
        {
            return new Vector2(1f, 1f).normalized; // (0.71, 0.71)
        }

        // Regra Padrão: Se NENHUMA tecla de mira for pressionada, atira reto para a direção atual
        return new Vector2(direcaoOlhando, 0f);
    }

    void PlantarMina()
    {
        if (minaPrefab == null) return;

        Vector3 posicaoMina = pontoMina != null ? pontoMina.position : transform.position;
        Instantiate(minaPrefab, posicaoMina, Quaternion.identity);
    }

    IEnumerator ExecutarDash()
    {
        podeFazerDash = false;
        estaEmDash = true;

        float gravidadeOriginal = rb.gravityScale;
        rb.gravityScale = 0f; // Anula a gravidade temporariamente para o dash ser retilíneo
        
        // Aplica o impulso na direção para onde está olhando
        rb.linearVelocity = new Vector2(direcaoOlhando * velocidadeDash, 0f);

        yield return new WaitForSeconds(duracaoDash);

        rb.gravityScale = gravidadeOriginal;
        estaEmDash = false;

        yield return new WaitForSeconds(cooldownDash);
        podeFazerDash = true;
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}