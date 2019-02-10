using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using UnityEngine;

public abstract class Stat 
{
    public event Action<Stat> OnStatChange;
    public event Action<Stat> OnMaxValueChange;

    protected StatSheet statSheet;

    public string Name { get; protected set; }

    protected float limitBreakMaxValue;
    protected float limitBreakValue;

    float baseMaxValue;
    float baseValue;

    public float Value
    {
        get { return baseValue + limitBreakValue; }
    }

    public float BuffedValue
    {
        get
        {
            return Value * efficencyBuffs.ModifierValue;
        }
    }

    protected float StatPercent
    {
        get
        {
            return BuffedValue / baseMaxValue;
        }
    }

    public string Description { get; protected set; }

    BuffCollection statIncreaseBuff;
    BuffCollection efficencyBuffs;

    Dictionary<Stat, float> limitIncreasePerStat;//remember each change cause what if buffs change 

    public Stat(string name, float value, float baseMaxValue, float limitbreakMaxValue ,string description, StatSheet statSheet)
    {
        Name = name;
        baseValue = value;
        limitBreakMaxValue = limitbreakMaxValue;
        this.baseMaxValue = baseMaxValue;
        Description = description;
        statIncreaseBuff = new BuffCollection();
        efficencyBuffs = new BuffCollection();
        this.statSheet = statSheet;
        limitBreakValue = 0;
        limitIncreasePerStat = new Dictionary<Stat, float>();
    }

    public float Change(float val)
    {
        float diff = 0f;
        if (val > 0)
        {
            if(baseValue >= baseMaxValue)
            {
                diff = IncreaseLimitBreak(val);
            }
            else
            {
                diff = IncreaseBaseValue(val);
            }
        }
        else
        {
            if(limitBreakValue > 0)
            {
                //decrease limit break first carry over to value
                limitBreakValue += val;//val is smaller than 0
                if(limitBreakValue < 0)
                {
                    baseValue += limitBreakValue;//limitbreak is smaller than 0
                    limitBreakValue = 0;
                    baseValue = Mathf.Clamp(baseValue, 0, baseMaxValue);//clamp to 0 if below
                }
            }
            else
            {
                baseValue += val; //decreasing so no need to check over max
                baseValue = Mathf.Clamp(baseValue, 0, baseMaxValue);//clamp to 0 if below
            }
        }
        OnStatChange?.Invoke(this);
        return diff;
    }  

    public float Increase(float val)
    {
        return Change(val);
    }

    public float Decrease(float val)
    {
        return Change(-val);
    }

    protected float IncreaseLimitBreak(float val)
    {
        float diff = 0f;
        //see if limit breakable
        if (limitBreakValue + val > limitBreakMaxValue)
        {
            //return unused points
            diff = (limitBreakValue + val) - limitBreakMaxValue;
            limitBreakValue = limitBreakMaxValue;
        }
        else
        {
            //no unused points
            limitBreakValue += val * statIncreaseBuff.ModifierValue;
            limitBreakValue = Mathf.Clamp(limitBreakValue, 0, limitBreakMaxValue);
        }
        return diff;
    }

    protected float IncreaseBaseValue(float val)
    {
        float diff = 0f;
        //see if limit breakable
        if (baseValue + val > baseMaxValue)
        {
            //return unused points
            diff = (baseValue + val) - baseMaxValue;

            //increase limit break
            diff = IncreaseLimitBreak(diff);

            baseValue = baseMaxValue;
        }
        else
        {
            baseValue += val * statIncreaseBuff.ModifierValue;//just increase
            baseValue = Mathf.Clamp(baseValue, 0, baseMaxValue);
        }
        return diff;
    }

    public float GetStatModifiedPercentEffect(float effect)
    {
        return effect* StatPercent;
    }

    public void AddEfficencyBuff(Modifier b)
    {
        efficencyBuffs.AddModifier(b);
        OnStatChange?.Invoke(this);
    }

    public void AddStatIncreaseBuff(Modifier b)
    {
        statIncreaseBuff.AddModifier(b);
    }

    public void ConnectMaxValueCahngeToOtherStatChange(Stat s, float maxInfluenceEfficency)
    {
        float efficency = maxInfluenceEfficency;//for closure
        s.OnStatChange += (st) => OnMaxControllingStatChange(st, efficency);
        AddStatToLimitIncrease(s);
    }

    protected float AddStatToLimitIncrease(Stat s)
    {
        limitIncreasePerStat.Add(s, 0f);
        return 0f;
    }

    public void OnMaxControllingStatChange(Stat s, float efficency)
    {
        float decrease = limitIncreasePerStat.ContainsKey(s) ? limitIncreasePerStat[s] : AddStatToLimitIncrease(s);
        float increase = s.BuffedValue * efficency;
        limitBreakMaxValue = (limitBreakMaxValue - decrease) + increase;

        limitIncreasePerStat[s] = increase;

        OnMaxValueChange?.Invoke(this);
    }

    public override string ToString()
    {
        return $@"Name: {Name}
Base: {baseValue} / {baseMaxValue}
Limit Break: {limitBreakValue} / {limitBreakMaxValue}
Stat Value: {Value}
Stat Increase Buff: {statIncreaseBuff.ModifierValue}
Stat efficency Buff: {efficencyBuffs.ModifierValue}
Stat Percent: {StatPercent}";
    }
}

public class Strength : Stat
{
    public Strength(float value, float maxValue, float limitbreakMaxValue, StatSheet statSheet) : base(typeof(Strength).ToString(), value, maxValue, limitbreakMaxValue,"The Strength of a character influences the damage delt with physical attacks, as well as the carrying capacity, and if certain equipment can be equipt", statSheet)
    {}
}

public class Vitality : Stat
{
    public Vitality(float value, float maxValue, float limitbreakMaxValue, StatSheet statSheet) : base(typeof(Vitality).ToString(), value, maxValue, limitbreakMaxValue, "Vitality influences the maximum health a character has, to a minor extent damage reduction", statSheet)
    {}
}

public class Inteligence : Stat
{
    public Inteligence(float value, float maxValue, float limitbreakMaxValue, StatSheet statSheet) : base(typeof(Inteligence).ToString(), value, maxValue, limitbreakMaxValue, "Inteligence influences max mana, restricts certain equipment", statSheet)
    {}
}

public class Dexterity : Stat
{
    public Dexterity(float value, float maxValue, float limitbreakMaxValue, StatSheet statSheet) : base(typeof(Dexterity).ToString(), value, maxValue,limitbreakMaxValue, "Dexterity influcens character movement speed to a minor degree, and restricts equipable items", statSheet)
    {}
}

public class Wisdom : Stat
{
    public Wisdom(float value, float maxValue, float limitbreakMaxValue, StatSheet statSheet) : base(typeof(Wisdom).ToString(), value, maxValue,limitbreakMaxValue, "Wisdom influences the mana recharge rate. Restricts equiupment", statSheet)
    {}
}

public class Charisma : Stat
{
    public Charisma(float value, float maxValue, float limitbreakMaxValue, StatSheet statSheet) : base(typeof(Charisma).ToString(), value, maxValue, limitbreakMaxValue,"Charisma is inportant when negiotiating with quest givers and merchants, and may lead to different quest options and or discounts when buying", statSheet)
    {}
}

public class Luck : Stat
{
    public Luck(float value, float maxValue, float limitbreakMaxValue, StatSheet statSheet) : base(typeof(Luck).ToString(), value, maxValue, limitbreakMaxValue, "Luck is a useless stat, and if you already spent points in it without reading the description first, one could say you are unluky. But I would say that your Inteligence stat might be low.", statSheet)
    {}
}

public class Defense : Stat
{
    public Defense(float value, float maxValue, float limitbreakMaxValue, StatSheet statSheet) : base(typeof(Defense).ToString(), value, maxValue, limitbreakMaxValue, "Defense reduces damage recieved from physical attaks", statSheet)
    {}
}

public class Endurance : Stat
{
    public Endurance(float value, float maxValue, float limitbreakMaxValue, StatSheet statSheet) : base(typeof(Endurance).ToString(),value, maxValue,limitbreakMaxValue, "Endurance influences the health recharge rate", statSheet)
    {
    }
}

public class Resitance : Stat
{
    public Resitance(float value, float maxValue, float limitbreakMaxValue, StatSheet statSheet) : base(typeof(Resitance).ToString(), value, maxValue, limitbreakMaxValue,"Resitance decreases the damage received from magical attacks", statSheet)
    {
    }
}

public class StatSheet
{
    public static void SetGraphStatInfluence(StatInfluenceGraphStorage st)
    {
        graphStatInfluence = GraphStatInfluence.CreateStatInfluenceGraph(st);
    }

    protected static GraphStatInfluence graphStatInfluence;

    public Strength Strenght { get; protected set; }
    public Vitality Vitality { get; protected set; }
    public Inteligence Inteligence { get; protected set; }
    public Dexterity Dexterity { get; protected set; }
    public Wisdom Wisdom { get; protected set; }
    public Charisma Charisma { get; protected set; }
    public Luck Luck { get; protected set; }
    public Defense Defense { get; protected set; }
    public Endurance Endurance { get; protected set; }
    public Resitance Resitance { get; protected set; }

    protected StatSheet()
    {}

    protected void EntagleStats()
    {

        foreach(GraphStatInfluence.StatConnection connection in graphStatInfluence.StatConnections)
        {
            Stat influencer = GetStatByIDX(connection.Influencer);
            
            foreach(GraphStatInfluence.StatConnection.Influenced influenceInfo in connection.InfluencedStats)
            {
                Stat influenced = GetStatByIDX(influenceInfo.idx);
                influenced.ConnectMaxValueCahngeToOtherStatChange(influencer, influenceInfo.val);
                influenced.OnMaxControllingStatChange(influencer, influenceInfo.val);
            }
        }
    }

    /// <summary>
    /// new contender for ugliest function
    /// </summary>
    protected Stat GetStatByIDX(int idx)
    {
        PropertyInfo[] infos = GetType().GetProperties();
        if (idx < 0 || idx >= infos.Length)
            throw new Exception($"Index out of range allowed indices: 0 - {infos.Length - 1}");

        return infos[idx].GetValue(this) as Stat;
    }

    public static class Creator
    {
        public static StatSheet CreateRandomStatSheetInRange(float min, float max)
        {
            StatSheet s = new StatSheet();

            s.Strenght = new Strength(UnityEngine.Random.Range(min,max), max, max, s);
            s.Vitality = new Vitality(UnityEngine.Random.Range(min, max), max, max, s);
            s.Inteligence = new Inteligence(UnityEngine.Random.Range(min, max), max, max, s);
            s.Dexterity = new Dexterity(UnityEngine.Random.Range(min, max), max, max, s);
            s.Wisdom = new Wisdom(UnityEngine.Random.Range(min, max), max, max, s);
            s.Charisma = new Charisma(UnityEngine.Random.Range(min, max), max, max, s);
            s.Luck = new Luck(UnityEngine.Random.Range(min, max), max, max, s);
            s.Defense = new Defense(UnityEngine.Random.Range(min, max), max, max, s);
            s.Endurance = new Endurance(UnityEngine.Random.Range(min, max), max, max, s);
            s.Resitance = new Resitance(UnityEngine.Random.Range(min, max), max, max, s);

            s.EntagleStats();

            return s;
        }
    }
}
