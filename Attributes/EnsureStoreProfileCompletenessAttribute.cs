namespace BiyLineApi.Attributes;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public class EnsureStoreProfileCompletenessAttribute : TypeFilterAttribute
{
    public EnsureStoreProfileCompletenessAttribute() : base(typeof(EnsureStoreProfileCompletenessFilter)) { }
}
