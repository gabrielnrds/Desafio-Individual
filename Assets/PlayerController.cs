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

    [Header("Sistema de Munição")]
    public int municaoAtual = 20;
    public int municaoMaxima = 30;

    [Header("Ação Especial: Sobrecarga")]
    public int custoMuniConsumoEspecial = 5;
    public float tempoCooldownEspecial = 2.5f;
    public float anguloEspalhamento = 15f; // Ângulo das balas laterais
    private bool especialEmCooldown = false;

    [Header("Ação: Tiro Normal")]
    public GameObject projetilPrefab;
    public Transform pontoDisparo;
    public float cooldownTiro = 0.3f;
    private float ultimoTiro;
    public float alturaTiro = 0.5f;
    public float raioPontoDisparo = 0.8f;

    [Header("Ação: Dash")]
    public float velocidadeDash = 20f;
    public float duracaoDash = 0.2f;
    public float cooldownDash = 1f;
    private bool podeFazerDash = true;
    private bool estaEmDash;

    [Header("Ação: Mina")]
    public GameObject minaPrefab;
    public Transform pontoMina;
    public float cooldownMina = 2f;
    private float ultimaMina;

    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

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
        if (estaEmDash) return;

        // 1. Leitura de Entrada de Movimento
        horizontalInput = Input.GetAxisRaw("Horizontal");

        if (horizontalInput > 0)
        {
            direcaoOlhando = 1f;
            spriteRenderer.flipX = false;

            if (pontoDisparo != null)
            {
                pontoDisparo.localPosition = new Vector3(Mathf.Abs(pontoDisparo.localPosition.x), pontoDisparo.localPosition.y, 0f);
            }
        }
        else if (horizontalInput < 0)
        {
            direcaoOlhando = -1f;
            spriteRenderer.flipX = true;

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

            bool segurandoW = Input.GetKey(KeyCode.W);
            bool segurandoQ = Input.GetKey(KeyCode.Q);
            bool segurandoE = Input.GetKey(KeyCode.E);

            animator.SetFloat("AimY", segurandoW || segurandoQ || segurandoE ? 1f : 0f);
            animator.SetBool("IsDiagonal", (segurandoQ || segurandoE) && isGrounded);
        }

        // 3. Pulo
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            jumpRequested = true;
        }

        // 4. Dash
        if (Input.GetKeyDown(KeyCode.LeftShift) && podeFazerDash)
        {
            StartCoroutine(ExecutarDash());
        }

        // 5. Tiro Normal (Botão Esquerdo / Tecla J)
        if ((Input.GetKeyDown(KeyCode.J) || Input.GetMouseButtonDown(0)) && Time.time >= ultimoTiro + cooldownTiro && !especialEmCooldown)
        {
            AtirarNormal();
            ultimoTiro = Time.time;
        }

        // 6. Tiro Especial - Sobrecarga (Botão Direito / Tecla K)
        if ((Input.GetKeyDown(KeyCode.K) || Input.GetMouseButtonDown(1)) && !especialEmCooldown)
        {
            AtirarEspecial();
        }

        // 7. Plantar Mina (Tecla F)
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

    void AtirarNormal()
    {
        if (municaoAtual <= 0)
        {
            Debug.Log("Sem munição!");
            return;
        }

        Vector2 direcaoTiro = ObterDirecaoTiro();
        if (direcaoTiro == Vector2.zero) return;

        municaoAtual--; // Desconta 1 de munição

        InstanciarBala(direcaoTiro);

        if (animator != null)
        {
            animator.SetTrigger("IsShooting");
        }
    }

    void AtirarEspecial()
    {
        if (municaoAtual < custoMuniConsumoEspecial)
        {
            Debug.Log("Munição insuficiente para o Tiro Especial!");
            return;
        }

        Vector2 direcaoBase = ObterDirecaoTiro();
        if (direcaoBase == Vector2.zero) return;

        municaoAtual -= custoMuniConsumoEspecial; // Desconta 5 de munição

        // Dispara 3 projéteis (Central, Esquerda e Direita)
        InstanciarBala(direcaoBase);
        InstanciarBala(Quaternion.Euler(0, 0, anguloEspalhamento) * direcaoBase);
        InstanciarBala(Quaternion.Euler(0, 0, -anguloEspalhamento) * direcaoBase);

        // Aplica a punição de sobrecarga (cooldown longo que trava armas)
        StartCoroutine(RotinaCooldownEspecial());

        if (animator != null)
        {
            animator.SetTrigger("IsShooting");
        }
    }

    void InstanciarBala(Vector2 direcao)
    {
        if (projetilPrefab == null || pontoDisparo == null) return;

        Vector3 offsetLocal = (Vector3)direcao.normalized * raioPontoDisparo;
        offsetLocal.y += alturaTiro;

        pontoDisparo.localPosition = offsetLocal;

        GameObject bala = Instantiate(projetilPrefab, pontoDisparo.position, Quaternion.identity);
        Projetil scriptBala = bala.GetComponent<Projetil>();

        if (scriptBala != null)
        {
            scriptBala.Setup(direcao.normalized);
        }
    }

    IEnumerator RotinaCooldownEspecial()
    {
        especialEmCooldown = true;
        yield return new WaitForSeconds(tempoCooldownEspecial);
        especialEmCooldown = false;
    }

    Vector2 ObterDirecaoTiro()
    {
        bool segurandoW = Input.GetKey(KeyCode.W);
        bool segurandoQ = Input.GetKey(KeyCode.Q);
        bool segurandoE = Input.GetKey(KeyCode.E);
        bool estaMovimentando = Mathf.Abs(horizontalInput) > 0.1f;

        if (segurandoW && !estaMovimentando) return Vector2.up;
        if (segurandoQ && isGrounded) return new Vector2(-1f, 1f).normalized;
        if (segurandoE && isGrounded) return new Vector2(1f, 1f).normalized;

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
        rb.gravityScale = 0f;
        
        rb.linearVelocity = new Vector2(direcaoOlhando * velocidadeDash, 0f);

        yield return new WaitForSeconds(duracaoDash);

        rb.gravityScale = gravidadeOriginal;
        estaEmDash = false;

        yield return new WaitForSeconds(cooldownDash);
        podeFazerDash = true;
    }

    public void AdicionarMunicao(int quantidade)
    {
        municaoAtual = Mathf.Min(municaoAtual + quantidade, municaoMaxima);
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