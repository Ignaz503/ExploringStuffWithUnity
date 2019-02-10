using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ModifierCollection
{
    public abstract float ModifierValue { get; protected set; }

    public ModifierCollection()
    {}

    public void AddModifier(Modifier b)
    {
        if (b.ModifierType == Modifier.Type.Additive)
            AddAdditiveModifier(b);
        else
            AddMultiplicativeModifier(b);
    }

    public void RemoveModifier(Modifier b)
    {
        if (b.ModifierType == Modifier.Type.Additive)
            RemoveAdditiveModifier(b);
        else
            RemoveMultiplicativeModifier(b);
    }

    protected abstract void AddMultiplicativeModifier(Modifier b);

    protected abstract void RemoveMultiplicativeModifier(Modifier b);

    protected abstract void Recalculate();

    protected abstract void AddAdditiveModifier(Modifier b);

    protected abstract void RemoveAdditiveModifier(Modifier b);
}

public abstract class GenericModifiersCollection<T> : ModifierCollection where T : ICollection<Modifier>, new()
{
    protected T additiveModifiers;
    protected T multiplicativeModifiers;

    public GenericModifiersCollection()
    {
        additiveModifiers = new T();
        multiplicativeModifiers = new T();
    }

    protected override void AddAdditiveModifier(Modifier b)
    {
        additiveModifiers.Add(b);
        AdditiveModifierAdded();
    }

    protected override void AddMultiplicativeModifier(Modifier b)
    {
        multiplicativeModifiers.Add(b);
        MultiplicativeModifierAdded();
    }

    protected override void RemoveAdditiveModifier(Modifier b)
    {
        AdditiveModifierRemoved(b, additiveModifiers.Remove(b));
    }

    protected override void RemoveMultiplicativeModifier(Modifier b)
    {
        MultiplicativeModifierRemoved(b, multiplicativeModifiers.Remove(b));
    }

    protected abstract void AdditiveModifierAdded();
    protected abstract void MultiplicativeModifierAdded();
    protected abstract void AdditiveModifierRemoved(Modifier removed, bool isRemoved);
    protected abstract void MultiplicativeModifierRemoved(Modifier removed, bool isRemoved);

    protected override void Recalculate()
    {
        float val = 0;

        float increase = 1f;
        foreach(Modifier mod in multiplicativeModifiers)
        {
            increase *= mod.Percent;
            val += increase;
        }
        foreach (Modifier mod in additiveModifiers)
        {
            val += mod.Percent;
        }
        ModifierValue = val;
    }
}

public abstract class UniqueInstantModifierCollection : GenericModifiersCollection<HashSet<Modifier>>
{
    public override float ModifierValue { get; protected set; }

    public UniqueInstantModifierCollection() : base()
    { }

    protected override void MultiplicativeModifierAdded()
    {
        Recalculate();
    }

    protected override void MultiplicativeModifierRemoved(Modifier removed,bool isRemoved)
    {
        if(isRemoved)
            Recalculate();
    }

    protected override void AdditiveModifierAdded()
    {
        //ModifierValue += b.Percent;
        Recalculate();
    }

    protected override void AdditiveModifierRemoved(Modifier removed, bool isRemoved)
    {
        if (isRemoved)
            Recalculate();
        //ModifierValue -= b.Percent;
    }
}

public abstract class InstantModifierCollection : GenericModifiersCollection<List<Modifier>>
{
    public override float ModifierValue { get; protected set; }

    public InstantModifierCollection() : base()
    { }

    protected override void MultiplicativeModifierAdded()
    {
        Recalculate();
    }

    protected override void MultiplicativeModifierRemoved(Modifier removed, bool isRemoved)
    {
        if (isRemoved)
            Recalculate();
    }

    protected override void AdditiveModifierAdded()
    {
        //ModifierValue += b.Percent;
        Recalculate();
    }

    protected override void AdditiveModifierRemoved(Modifier removed, bool isRemoved)
    {
        if (isRemoved)
            Recalculate();
        //ModifierValue -= b.Percent;
    }
}

public abstract class UniqueLazyModifierCollection : GenericModifiersCollection<HashSet<Modifier>>
{
    float modValue;

    public override float ModifierValue
    {
        get
        {
            if (isDirty)
            {
                Recalculate();
                isDirty = false;
            }
            return modValue;
        }
        protected set
        {
            modValue = value;
        }
    }

    protected bool isDirty;

    public UniqueLazyModifierCollection() : base()
    { }

    protected override void MultiplicativeModifierAdded()
    {
        //calculate buff

        isDirty = true;
    }

    protected override void MultiplicativeModifierRemoved(Modifier removed, bool isRemoved)
    {

        isDirty = true;
    }

    protected override void AdditiveModifierAdded()
    {

        isDirty = true;
    }

    protected override void AdditiveModifierRemoved(Modifier removed, bool isRemoved)
    {

            isDirty = true;
    }

}

public abstract class LazyModifierCollection : GenericModifiersCollection<HashSet<Modifier>>
{
    float modValue;

    public override float ModifierValue
    {
        get
        {
            if (isDirty)
            {
                Recalculate();
                isDirty = false;
            }
            return modValue;
        }
        protected set
        {
            modValue = value;
        }
    }

    protected bool isDirty;

    public LazyModifierCollection() : base()
    { }

    protected override void MultiplicativeModifierAdded()
    {
        //calculate buff
        isDirty = true;
    }

    protected override void MultiplicativeModifierRemoved(Modifier removed, bool isRemoved)
    {

        isDirty = true;
    }

    protected override void AdditiveModifierAdded()
    {

        isDirty = true;
    }

    protected override void AdditiveModifierRemoved(Modifier removed, bool isRemoved)
    {

        isDirty = true;
    }

}

public class BuffCollection : UniqueInstantModifierCollection
{
    public override float ModifierValue { get { return base.ModifierValue + 1f; } protected set { base.ModifierValue = value; } }

    public BuffCollection() : base()
    { }
}

public class DebuffCollection : UniqueInstantModifierCollection
{
    public override float ModifierValue
    {
        get
        {
            return 1f - base.ModifierValue;
        }
        protected set
        {
            base.ModifierValue = value;
        }
    }

    public DebuffCollection() : base()
    { }
}

public class Modifier
{
    public enum Type
    {
        Multiplicative,
        Additive
    }

    public int ID { get; protected set; }
    public Type ModifierType { get; protected set; }
    public float Percent { get; protected set; }

    public Modifier(int ID, Type buffType, float percent)
    {
        this.ID = ID;
        ModifierType = buffType;
        Percent = percent;
    }

    public override int GetHashCode()
    {
        return ID;
    }

    public override bool Equals(object obj)
    {
        return base.Equals(obj);
    }

    public override string ToString()
    {
        return $@"ID: {ID}
                Type: {ModifierType}
                Value: {Percent}";
    }
}

