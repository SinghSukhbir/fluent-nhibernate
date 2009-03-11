namespace FluentNHibernate.Mapping.Conventions.Defaults
{
    public class UserAlterationsDynamicInsertConvention : IClassConvention
    {
        public bool Accept(IClassMap target)
        {
            return !target.Attributes.Has("dynamic-insert");
        }

        public void Apply(IClassMap target, ConventionOverrides overrides)
        {
            if (overrides.DynamicInsert == null) return;
            
            var value = overrides.DynamicInsert(target);

            if (value == true)
                target.DynamicInsert();
            else if (value == false)
                target.Not.DynamicInsert();
        }
    }
}