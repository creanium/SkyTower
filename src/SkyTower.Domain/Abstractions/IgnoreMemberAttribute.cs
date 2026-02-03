namespace SkyTower.Domain.Abstractions;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public sealed class IgnoreMemberAttribute : Attribute
{
}