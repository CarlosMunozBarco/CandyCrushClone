using System.Collections;
using TMPro;
using UnityEngine;

public class GameUI : MonoBehaviour
{
    [SerializeField] private TMP_Text   scoreText;
    [SerializeField] private GameObject noMovesPanel;
    [SerializeField] private TMP_Text   noMovesText;
    [SerializeField] private TMP_Text   comboText;

    private Coroutine _comboCoroutine;
    private Vector3   _comboTextOrigin;

    private static readonly Color[] ComboColors =
    {
        new Color(1f, 1f, 0f),    // x2 - amarillo
        new Color(1f, 0.6f, 0f),  // x3 - naranja
        new Color(1f, 0.2f, 0f),  // x4 - rojo-naranja
        new Color(1f, 0f, 0.5f),  // x5+ - fucsia
    };

    private void Awake()
    {
        if (comboText != null)
            _comboTextOrigin = comboText.transform.localPosition;
    }

    public void UpdateScore(int score)
    {
        scoreText.text = $"Score: {score}";
    }

    public void ShowComboText(int level)
    {
        if (comboText == null) return;
        if (_comboCoroutine != null) StopCoroutine(_comboCoroutine);
        _comboCoroutine = StartCoroutine(ComboTextCoroutine(level));
    }

    private IEnumerator ComboTextCoroutine(int level)
    {
        comboText.text  = $"COMBO x{level}!";
        int colorIndex  = Mathf.Clamp(level - 2, 0, ComboColors.Length - 1);
        Color baseColor = ComboColors[colorIndex];
        comboText.gameObject.SetActive(true);
        comboText.transform.localScale = Vector3.zero;

        // Pop in: scale 0 → 1.3
        float elapsed = 0f;
        float popDuration = 0.2f;
        while (elapsed < popDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / popDuration;
            float scale = Mathf.Lerp(0f, 1.3f, t * t);
            comboText.transform.localScale = Vector3.one * scale;
            comboText.color = new Color(baseColor.r, baseColor.g, baseColor.b, 1f);
            yield return null;
        }

        // Settle: scale 1.3 → 1.0
        elapsed = 0f;
        float settleDuration = 0.1f;
        while (elapsed < settleDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / settleDuration;
            float scale = Mathf.Lerp(1.3f, 1f, t);
            comboText.transform.localScale = Vector3.one * scale;
            yield return null;
        }
        comboText.transform.localScale = Vector3.one;

        // Hold
        yield return new WaitForSeconds(0.5f);

        // Fade out with upward drift
        elapsed = 0f;
        float fadeDuration = 0.35f;
        comboText.transform.localPosition = _comboTextOrigin;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / fadeDuration;
            comboText.color = new Color(baseColor.r, baseColor.g, baseColor.b, 1f - t);
            comboText.transform.localPosition = _comboTextOrigin + Vector3.up * (40f * t);
            yield return null;
        }

        comboText.transform.localPosition = _comboTextOrigin;
        comboText.gameObject.SetActive(false);
        _comboCoroutine = null;
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
