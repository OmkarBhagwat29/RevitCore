
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.ExtensibleStorage;
using System;
using System.Linq;


namespace RevitCore.Extensions.ExtensibleStorage
{
    public enum SchemaFieldType
    {
        Map,
        Array,
        Simple
    }

    public static class ExtensibleStorageExtension
    {
        public static Schema AddSimpleFiledSchemaToElement<TElement>(this Element element,
            string schemaName, string fieldName,TElement fieldValue, string documentation,string vendorId = null,
            AccessLevel readAccessLevel = AccessLevel.Public,
            AccessLevel writeAccessLevel = AccessLevel.Application)
        {

            Schema schema = GetSchemaByName(schemaName);

            if (schema == null)
            {
                //create
                var schemaBuilder = CreateSchemaBuilder();

                schemaBuilder.SetReadAccessLevel(readAccessLevel);

                if (vendorId != null)
                {
                    schemaBuilder.SetWriteAccessLevel(writeAccessLevel);
                    schemaBuilder.SetVendorId(vendorId);
                }

                schemaBuilder.SetSchemaName(schemaName);

                FieldBuilder field = schemaBuilder.AddSimpleField(fieldName, typeof(TElement));

                if (field == null) throw new ArgumentNullException(nameof(field));

                field.SetDocumentation(documentation);

                schema = schemaBuilder.Finish();
            }

           schema.SetValueToSimpleFiledSchema<TElement>(fieldName, element, fieldValue);
           
            return schema;
        }

        private static SchemaBuilder CreateSchemaBuilder()
        {
            Guid id = Guid.NewGuid();

            return  new SchemaBuilder(id);
        }

        public static Schema GetSchemaByName(string schemaName)
        {
            var schemas = Schema.ListSchemas();
            if(schemas==null || schemas.Count == 0) throw new ArgumentNullException(nameof(schemas));

            return schemas.FirstOrDefault(s => s.SchemaName == schemaName);
        }

        public static void SetValueToSimpleFiledSchema<TElement>(this Schema schema,string fieldName,
            Element element, TElement value)
        {

            Entity entity = new Entity(schema);
            Field field = schema.GetField(fieldName) ?? throw new ArgumentNullException("Filed not found in the provided Schema");

            entity.Set<TElement>(field, value);

            

            element.SetEntity(entity);
        }

        public static TElement GetSchemaData<TElement>(this Schema schema,string fieldName, Element element)
        {
            var entity = element.GetEntity(schema);
            TElement value = entity.Get<TElement>(schema.GetField(fieldName));

            if (value == null) throw new ArgumentNullException($"unable to retrieve entity from an element => {element.Name}");

            return value;
        }
    }
}
