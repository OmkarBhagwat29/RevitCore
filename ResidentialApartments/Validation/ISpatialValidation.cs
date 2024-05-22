namespace RevitCore.ResidentialApartments.Validation
{
    public interface ISpatialValidation
    {
        public bool ValidationSuccess { get; }

        public void Validate();

        public void Bake(Document doc);

        public string GetValidationReport();

        public Type SpatialType { get; }
    }
}
