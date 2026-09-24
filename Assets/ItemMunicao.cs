using UnityEngine;

public class ItemMunicao : MonoBehaviour
{
    public int quantidadeRecarga = 10;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerController player = collision.GetComponent<PlayerController>();
            if (player != null)
            {
                player.AdicionarMunicao(quantidadeRecarga);
            }

            Destroy(gameObject);
        }
    }
}