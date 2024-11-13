using System;

namespace FastRide.Server.Services.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Property)]
public class TableNameAttribute : Attribute
{
    private string _name;
    
    public TableNameAttribute(string name)
    {
        _name = name;
    }
    
    public string Name => _name;
}