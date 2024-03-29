﻿namespace MystiickWeb.Shared;

[AttributeUsage(AttributeTargets.Class)]
public class InjectableAttribute : Attribute
{
    public Type Interface { get; set; }
    public InjectableSetting Setting { get; set; }

    public InjectableAttribute(Type implementedInterface, InjectableSetting setting = InjectableSetting.Scoped)
    {
        Interface = implementedInterface;
        Setting = setting;
    }

    public enum InjectableSetting
    {
        Scoped,
        Transient,
        Singleton
    }
}
