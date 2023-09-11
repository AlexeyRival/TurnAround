using UnityEngine;
using UnityEngine.UI;

public class Controller : MonoBehaviour
{
    private int turn;
    private bool turnPhase;
    [SerializeField]
    private Warrior leftWarrior, rightWarrior;
    [Header("UI")]
    [SerializeField]
    private Text turnText;
    [SerializeField]
    private Button leftBuffBtn, leftAttackBtn, rightBuffBtn, rightAttackBtn;
    [Header("Buffs")]
    [SerializeField]
    private Buff[] buffs;
    
    //Actions
    public void Attack() 
    {
        if (turnPhase)
        {
            rightWarrior.Attack();
        }
        else 
        {
            leftWarrior.Attack();
        }
        EndPhase();
    }
    public void AddBuff()
    {
        Buff buf = buffs[Random.Range(0, buffs.Length)];
        if (turnPhase)
        {
            if (rightWarrior.buffs.Count == 1) 
            {
                while (rightWarrior.buffs[0].name == buf.name) 
                {
                    buf = buffs[Random.Range(0, buffs.Length)];
                }
            }
            rightWarrior.AddBuff(buf);
            rightBuffBtn.interactable = false;
        }
        else 
        {
            if (leftWarrior.buffs.Count == 1)
            {
                while (leftWarrior.buffs[0].name == buf.name)
                {
                    buf = buffs[Random.Range(0, buffs.Length)];
                }
            }
            leftWarrior.AddBuff(buf);
            leftBuffBtn.interactable = false;
        }
    }
    
    //Turn logic
    private void EndPhase() 
    {
        (turnPhase ? rightWarrior : leftWarrior).EndTurn();
        turnPhase = !turnPhase;
        if (!turnPhase) 
        {
            EndTurn();
        }
        turnText.text = $"Раунд {turn}\nХод {(turnPhase ? "Правого" : "Левого")}";
        leftBuffBtn.interactable = !turnPhase && leftWarrior.buffs.Count < 2;
        rightBuffBtn.interactable = turnPhase && rightWarrior.buffs.Count < 2;
        leftAttackBtn.interactable = !turnPhase;
        rightAttackBtn.interactable = turnPhase;
    }
    private void EndTurn() 
    {
        ++turn;
        if (leftWarrior.GetHp() == 0 || rightWarrior.GetHp() == 0) 
        {
            Restart();
        }
    }
    public void Restart() 
    {
        Application.LoadLevel(0);
    }
}