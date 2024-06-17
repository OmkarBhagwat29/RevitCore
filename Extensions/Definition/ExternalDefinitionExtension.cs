
namespace RevitCore.Extensions.DefinitionExt
{
    public static class ExternalDefinitionExtension
    {
        public static void TryAddToDocument(this IEnumerable<ExternalDefinition> externalDefinitions, Document doc,
    List<BuiltInCategory> categories, BindingKind bindingKind = BindingKind.Instance,
    ForgeTypeId builtInParameterGroupId = null)
        {
           
            if (externalDefinitions == null) throw new ArgumentNullException(nameof(externalDefinitions));
            if (doc == null) throw new ArgumentNullException(nameof(doc));
            if (categories == null) throw new ArgumentNullException(nameof(categories));

            if (builtInParameterGroupId == null)
            {
                foreach (var externalDefinition in externalDefinitions)
                {
                    var binding = doc.CreateBinding(categories, bindingKind);
                    
                    doc.ParameterBindings.Insert(externalDefinition, binding);
                }
            }
            else
            {
                foreach (var externalDefinition in externalDefinitions)
                {
                    var binding = doc.CreateBinding(categories, bindingKind);
                    doc.ParameterBindings.Insert(externalDefinition, binding, builtInParameterGroupId);
                }
            }
        }
    }
}
