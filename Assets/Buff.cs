using UnityEngine;

[CreateAssetMenu(menuName ="Custom/Buff")]
public class Buff : ScriptableObject
{
    public Effect[] effects;
    public int turns;
}
[System.Serializable]
public class Effect 
{
    [Tooltip("Характеристика")]
    public Stat stat;
    [Tooltip("Цель")]
    public BuffEffectTarget target;
    [Tooltip("Значение")]
    public float value;
    [Tooltip("Является ли множителем")]
    public bool isMultiplier;
}
public enum BuffEffectTarget 
{
    self,
    enemy,
    both
}
public enum Stat 
{
    hp,
    armor,
    damage,
    vampire
}
