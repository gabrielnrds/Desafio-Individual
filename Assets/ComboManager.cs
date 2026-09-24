using UnityEngine;
using TMPro;

public class ComboManager : MonoBehaviour
{
    public static ComboManager Instance { get; private set; }

    [Header("Configurações do Combo")]
    public int comboAtual = 0;
    public int acertosParaSpawnDrone = 5; // A cada 5 acertos, surge um drone

    [Header("Referências de Interface e Spawner")]
    public TextMeshProUGUI textoCombo;
    public DroneSpawner droneSpawner;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        AtualizarUI();
    }

    public void RegistraAcerto()
    {
        comboAtual++;
        AtualizarUI();

        // Se o combo atingiu o número necessário, invoca um Drone no alto
        if (comboAtual % acertosParaSpawnDrone == 0)
        {
            if (droneSpawner != null)
            {
                droneSpawner.SpawnDrone();
            }
        }
    }

    public void ResetarCombo()
    {
        comboAtual = 0;
        AtualizarUI();
    }

    void AtualizarUI()
    {
        if (textoCombo != null)
        {
            textoCombo.text = comboAtual > 1 ? $"COMBO x{comboAtual}!" : "";
        }
    }
}