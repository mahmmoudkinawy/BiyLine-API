namespace BiyLineApi.Attributes; 

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class EnsureSingleStoreAttribute : TypeFilterAttribute
{
    public EnsureSingleStoreAttribute() : base(typeof(EnsureSingleStoreFilter)) { }
}
