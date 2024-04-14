
using Autodesk.Revit.DB;
using RevitCore.Extensions.Definition;


namespace RevitCore.Extensions
{
    public static class DocumentExtension
    {
        public static DirectShape CreateDirectShape(this Document doc, List<GeometryObject> geometryObjects,
            BuiltInCategory category = BuiltInCategory.OST_GenericModel)
        {
            var directShape = DirectShape.CreateElement(doc,new ElementId(category));

            directShape.SetShape(geometryObjects);

            return directShape;
        }

        public static Binding CreateBinding(this Document doc, List<BuiltInCategory> categories,
            BindingKind bindingKind)
        {
            if(doc == null)throw new ArgumentNullException(nameof(doc));
            if(categories is null)throw new ArgumentNullException(nameof(categories));

            var categorySet = new CategorySet();
            categories.ForEach(c => categorySet.Insert(Category.GetCategory(doc, c)));

            if (bindingKind is BindingKind.Instance) return new InstanceBinding(categorySet);

            return new TypeBinding(categorySet);
        }


        public static IEnumerable<TElement> GetElements<TElement>(this Document doc, Func<TElement,bool>validate = null)
            where TElement : Element
        {
            validate = validate ?? (e => true);

            return new FilteredElementCollector(doc)
                .OfClass(typeof(TElement))
                .Cast<TElement>()
                .Where(e => validate(e));
        }

        public static IEnumerable<TElement> GetElementsByType<TElement>(this Document doc, Func<TElement, bool> validate = null)
    where TElement : Element
        {
            validate = validate ?? (e => true);

            return new FilteredElementCollector(doc)
                .OfClass(typeof(TElement))
                .WhereElementIsElementType()
                .Cast<TElement>()
                .Where(e => validate(e));
        }

        public static IEnumerable<Element> GetElements(this Document doc, params Type[] types)
        {
            if (!types.Any())
                throw new ArgumentNullException("No Types Provided.");

            return new FilteredElementCollector(doc)
                .WherePasses(new ElementMulticlassFilter(types));
        }

        public static IEnumerable<Element> GetElements(this Document doc, List<Type> types)
        {
            return new FilteredElementCollector(doc)
                .WherePasses(new ElementMulticlassFilter(types));
        }

        public static IEnumerable<TElement> GetElementsByInstances<TElement>(this Document doc) where TElement : Element
        {
            return new FilteredElementCollector(doc)
                .OfClass(typeof(TElement))
                .WhereElementIsNotElementType()
                .Cast<TElement>();
        }

        public static IEnumerable<TElement> GetElementsByInstances<TElement>(this Document doc, Func<TElement, bool> validate)
            where TElement : Element
        {
            validate = validate ?? (e=>true);

            return new FilteredElementCollector(doc)
                .OfClass(typeof(TElement))
                .WhereElementIsNotElementType()
                .Cast<TElement>()
                .Where(e => validate(e));
        }

        public static TElement GetElementByName<TElement>(this Document doc, string name) where TElement : Element
        {
            var element = new FilteredElementCollector(doc)
                .OfClass(typeof(TElement))
                .FirstOrDefault(e => e.Name == name);

            element = element ?? throw new ArgumentNullException(nameof(element));

            return element as TElement;
        }

        public static IEnumerable<Element> GetInstancesOfCategory(this Document doc, BuiltInCategory category,
            Func<Element,bool>validate = null,
            ElementId viewId = null)
        {

            validate = validate ?? (e => true);

            if (viewId == null)
                return new FilteredElementCollector(doc)
                    .OfCategory(category)
                    .WhereElementIsNotElementType()
                    .Where(e=>validate(e));

            return new FilteredElementCollector(doc, viewId)
                .OfCategory(category)
                .WhereElementIsNotElementType()
                .Where(e=>validate(e));
        }
        
    }
}
