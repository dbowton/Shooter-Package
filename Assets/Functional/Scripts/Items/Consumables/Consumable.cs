using System;

public class Consumable : Item
{
    public override Type GetBaseType()
    {
        return typeof(Consumable);
    }

    public override string GetDesc()
    {
        return "consumable";
    }

    public void Use()
    {

    }
}
