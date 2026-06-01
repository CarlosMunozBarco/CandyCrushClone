using UnityEngine;

[CreateAssetMenu(fileName = "CandyData", menuName = "CandyCrush/CandyData")]
public class CandyData : ScriptableObject
{
    public CandyType             candyType;
    public CandySpecialBehaviour specialBehaviour;
    public Sprite                sprite;
    public Color                 tintColor = Color.white;

    public bool IsSpecial => specialBehaviour != null;
}
