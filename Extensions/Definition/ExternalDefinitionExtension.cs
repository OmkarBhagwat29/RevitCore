
namespace RevitCore.Extensions.DefinitionExt
{
    public static class ExternalDefinitionExtension
    {
#if REVIT2022_OR_GREATER

        public static void TryAddToDocument(this IEnumerable<ExternalDefinition> externalDefinitions, Document doc,
    List<BuiltInCategory> categories, BindingKind bindingKind = BindingKind.Instance,
    ForgeTypeId builtInParameterGroupId = null)
        {
            if (externalDefinitions == null)
                builtInParameterGroupId = GroupTypeId.IdentityData;

            if (externalDefinitions == null) throw new ArgumentNullException(nameof(externalDefinitions));
            if (doc == null) throw new ArgumentNullException(nameof(doc));
            if (categories == null) throw new ArgumentNullException(nameof(categories));

            foreach (var externalDefinition in externalDefinitions)
            {
                var binding = doc.CreateBinding(categories, bindingKind);
                doc.ParameterBindings.Insert(externalDefinition, binding, builtInParameterGroupId);
            }

        }


#else
        public static void TryAddToDocument(this IEnumerable<ExternalDefinition> externalDefinitions, Document doc,
            List<BuiltInCategory> categories,BindingKind bindingKind = BindingKind.Instance,
            BuiltInParameterGroup builtInParamGroup = BuiltInParameterGroup.INVALID)
        {
            if(externalDefinitions == null)throw new ArgumentNullException(nameof(externalDefinitions));
            if(doc == null) throw new ArgumentNullException(nameof(doc));
            if (categories == null) throw new ArgumentNullException(nameof(categories));

            foreach (var externalDefinition in externalDefinitions)
            {
                var binding = doc.CreateBinding(categories, bindingKind);
                doc.ParameterBindings.Insert(externalDefinition, binding, builtInParamGroup);
            }
        }

#endif
    }
}
