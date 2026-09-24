using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("Referências")]
    public TextMeshProUGUI textoMunicao;
    public PlayerController player;

    void Update()
    {
        if (player != null && textoMunicao != null)
        {
            textoMunicao.text = $"MUNIÇÃO: {player.municaoAtual} / {player.municaoMaxima}";
        }
    }
}