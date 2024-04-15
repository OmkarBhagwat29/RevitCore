

namespace RevitCore.Extensions.Filters
{
    public static class FilterExtension
    {
        public static IEnumerable<Element> GetMultiCategoryElements(this Document doc, 
            ICollection<BuiltInCategory> categories) {
        
            var multicategoryFilter = new ElementMulticategoryFilter(categories);

            return new FilteredElementCollector(doc)
                .WherePasses(multicategoryFilter)
                .ToElements();
        }

        public static IEnumerable<Element> GetCategoryElementsOfType(this Document doc, BuiltInCategory category)
        {
            return new FilteredElementCollector(doc)
                .OfCategory(category);
        }

        public static IEnumerable<Element>GetAndFilterElements(this Document doc,
            ElementFilter filter1, ElementFilter filter2)
        {
            var andFilter = new LogicalAndFilter(filter1, filter2);
             return new FilteredElementCollector (doc) .WherePasses(andFilter).ToElements();
        }
    }
}
