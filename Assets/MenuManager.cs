using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance { get; private set; }

    [Header("Painéis de UI")]
    public GameObject painelPause;
    public GameObject painelGameOver;

    private bool jogoPausado = false;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Update()
    {
        // Tecla ESC para pausar/despausar
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (jogoPausado) ResumirJogo();
            else PausarJogo();
        }
    }

    // --- MÉTODOS DE PAUSE ---
    public void PausarJogo()
    {
        if (painelPause != null) painelPause.SetActive(true);
        Time.timeScale = 0f; // Congela o tempo do jogo
        jogoPausado = true;
    }

    public void ResumirJogo()
    {
        if (painelPause != null) painelPause.SetActive(false);
        Time.timeScale = 1f; // Retoma o tempo normal
        jogoPausado = false;
    }

    // --- MÉTODOS DE GAME OVER ---
    public void ExibirGameOver()
    {
        if (painelGameOver != null) painelGameOver.SetActive(true);
        Time.timeScale = 0f;
    }

    // --- NAVEGAÇÃO DE CENAS ---
    public void Jogar()
    {
        Time.timeScale = 1f; // Garante que o tempo não esteja congelado por um Pause prévio
        SceneManager.LoadScene("SampleScene"); // Substitua pelo NOME EXATO da sua cena de jogo
    }
    
    public void ReiniciarJogo()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void CarregarMenuPrincipal()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MenuPrincipal"); // Nome da tua cena de menu
    }

    public void SairDoJogo()
    {
        Application.Quit();
        Debug.Log("Jogo Encerrado!");
    }
}