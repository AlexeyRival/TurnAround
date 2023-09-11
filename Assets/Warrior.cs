using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Warrior : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField]
    private int hp = 100;
    [SerializeField]
    private int dmg = 15;
    [SerializeField]
    private float armor = 0, vampire = 0;

    [Header("UI")]
    [SerializeField]
    private Slider hpSlider;
    [SerializeField]
    private Slider dmgSlider, armorSlider, vampireSlider;
    [SerializeField]
    private Text hpText, dmgText, armorText, vampireText;
    [SerializeField]
    private Transform buffContainer;
    [SerializeField]
    private GameObject buffTextPrefab;

    [Header("Renderer")]
    public MeshRenderer meshRenderer;
    //Beautiful effects
    private float targetHp, targetDmg, targetArmor, targetVampire;
    private float dmgEffect;
    private Color basecolor;

    public List<Buff> buffs;
    public Warrior enemy;

    

    //Base methods
    private void Start()
    {
        basecolor = meshRenderer.material.color;
        UpdateUI();
    }
    private void Update()
    {
        hpSlider.value = Mathf.Lerp(hpSlider.value, targetHp, Time.deltaTime);
        dmgSlider.value = Mathf.Lerp(dmgSlider.value, targetDmg, Time.deltaTime);
        armorSlider.value = Mathf.Lerp(armorSlider.value, targetArmor, Time.deltaTime);
        vampireSlider.value = Mathf.Lerp(vampireSlider.value, targetVampire, Time.deltaTime);

        if (dmgEffect > 0)
        {
            meshRenderer.material.SetColor("_Color",Color.Lerp(basecolor, Color.red, dmgEffect));
            dmgEffect -= Time.deltaTime;
        }
    }


    //Battle logic
    public void Attack() 
    {
        int vamp = (int)(enemy.Dmg(GetDmg()) * .01f * GetVampire());
        if (vamp > 0) { Heal(vamp); }
        UpdateUI();
    }
    public int Dmg(int dmg) 
    {
        dmg = (int)(dmg * (1 - 0.01f * GetArmor()));
        hp -= dmg;
        if (hp < 0) { hp = 0; }
        if (dmg > 0) 
        {
            dmgEffect = 1f;
        }
        UpdateUI();
        return dmg;
    }
    public void Heal(int amout) 
    {
        hp += amout;
        if (hp > 100) { hp = 100; }
        UpdateUI();
    }
    //Buffs logic
    public void AddBuff(Buff buff)
    {
        if (buffs.Count == 2) { return; }
        buff = Instantiate(buff);
        buff.name = buff.name.Replace("(Clone)","");
        buffs.Add(buff);
        UpdateUI();
        enemy.UpdateUI();
    }
    public void UpdateUI()
    {
        UpdateBuffs();

        hpText.text = $"{GetHp()}/100";
        dmgText.text = $"{GetDmg()}";
        armorText.text = $"{GetArmor()}/100";
        vampireText.text = $"{GetVampire()}/100";

        targetHp = GetHp() / 100f;
        targetDmg = GetDmg() / 100f;
        targetArmor = GetArmor() / 100f;
        targetVampire = GetVampire() / 100f;

    }
    private void UpdateBuffs()
    {
        ClearRoot(buffContainer);
        Text bufftext;
        if (buffs.Count > 0)
        {
            for (int i = 0; i < buffs.Count; ++i)
            {
                bufftext = Instantiate(buffTextPrefab, buffContainer).GetComponent<Text>();
                bufftext.text = $"{buffs[i].name} {buffs[i].turns} ходов";
            }
        }
    }
    
    //Turn logic
    public void EndTurn() 
    {
        BuffTurn();
        UpdateUI();
    }
    private void BuffTurn()
    {
        for (int i = buffs.Count - 1; i >= 0; --i)
        {
            buffs[i].turns--;
            if (buffs[i].turns <= 0)
            {
                Destroy(buffs[i]);
                buffs.RemoveAt(i);
            }
        }
    }
    
    
    //Getters
    public int GetHp() 
    {
        //первая итерация - у меня, влияющее на меня, вторая - у противника, влияющее на меня
        int hp = GetDeltaStat(this.hp, Stat.hp, true);
        hp = enemy.GetDeltaStat(hp, Stat.hp, false);
        hp = Mathf.Clamp(hp, 0, 100);
        return hp;
    }
    public int GetDmg()
    {
        int dmg = GetDeltaStat(this.dmg, Stat.damage, true);
        dmg = enemy.GetDeltaStat(dmg, Stat.damage, false);
        return dmg;
    }
    public int GetArmor()
    {
        int armor = GetDeltaStat((int)this.armor, Stat.armor, true);
        armor = enemy.GetDeltaStat(armor, Stat.armor, false);
        armor = Mathf.Clamp(armor, 0, 100);
        return armor;
    }
    public int GetVampire()
    {
        int vampire = GetDeltaStat((int)this.vampire, Stat.vampire, true);
        vampire = enemy.GetDeltaStat(vampire, Stat.vampire, false);
        vampire = Mathf.Clamp(vampire, 0, 100);
        return vampire;
    }
    protected int GetDeltaStat(int val, Stat stat, bool onSelf)
    {
        //for быстрее на списках, foreach на массивах.
        for (int i = 0; i < buffs.Count; ++i)
            foreach (var effect in buffs[i].effects)
            {
                if (
                    (
                    (effect.target != BuffEffectTarget.enemy&&onSelf)||
                    (effect.target != BuffEffectTarget.self&&!onSelf)
                    )
                    && effect.stat == stat
                    )
                {
                    if (effect.isMultiplier)
                    {
                        val = (int)(effect.value * val);
                    }
                    else
                    {
                        val += (int)effect.value;
                    }
                }
            }
        return val;
    }

    //Utils
    private void ClearRoot(Transform transform)
    {
        for (int i = 0; i < transform.childCount; ++i)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
    }

}
