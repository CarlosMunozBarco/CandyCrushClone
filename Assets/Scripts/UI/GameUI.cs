using TMPro;
using UnityEngine;

public class GameUI : MonoBehaviour
{
    [SerializeField] private TMP_Text   scoreText;
    [SerializeField] private GameObject noMovesPanel;
    [SerializeField] private TMP_Text   noMovesText;

    public void UpdateScore(int score)
    {
        scoreText.text = $"Score: {score}";
    }

    public void ShowNoMovesMessage()
    {
        if (noMovesPanel != null) noMovesPanel.SetActive(true);
        if (noMovesText  != null) noMovesText.text = "Sin movimientos disponibles\n¡Reorganizando el tablero!";
    }

    public void HideNoMovesMessage()
    {
        if (noMovesPanel != null) noMovesPanel.SetActive(false);
    }
}
