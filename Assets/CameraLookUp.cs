using UnityEngine;
using Unity.Cinemachine;

public class CameraLookUp : MonoBehaviour
{
    [Header("Configurações do Deslocamento")]
    public float offsetYLookUp = 4f;        // Altura extra ao olhar para cima
    public float offsetYPadrao = 0f;        // Offset vertical normal
    public float velocidadeTransicao = 3f;  // Velocidade da transição da câmera

    private CinemachineCameraOffset cameraOffset;
    private float targetOffsetY;

    void Start()
    {
        // Obtém o componente de Offset da Cinemachine 3
        cameraOffset = GetComponent<CinemachineCameraOffset>();

        if (cameraOffset == null)
        {
            Debug.LogError("Por favor, adicione o componente 'Cinemachine Camera Offset' no Inspector desta Câmera!");
        }
    }

    void Update()
    {
        if (cameraOffset == null) return;

        // Detecta se o jogador pressiona W ou a Seta para Cima
        bool olhandoParaCima = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow);
        targetOffsetY = olhandoParaCima ? offsetYLookUp : offsetYPadrao;

        // Interpola suavemente a posição Y da câmera
        Vector3 offsetAtual = cameraOffset.Offset;
        offsetAtual.y = Mathf.Lerp(offsetAtual.y, targetOffsetY, Time.deltaTime * velocidadeTransicao);
        cameraOffset.Offset = offsetAtual;
    }
}