using UnityEngine;

[CreateAssetMenu(fileName = "CandyData", menuName = "CandyCrush/CandyData")]
public class CandyData : ScriptableObject
{
    public CandyType candyType;
    public bool      isSpecial;
    public Sprite    sprite;
    public Color     tintColor = Color.white;
}
