namespace RevitCore.ResidentialApartments.Validation
{
    public interface ISpatialValidation
    {
        public ValidationAccuracy Accuracy { get; }
        public void Validate();

        public IEnumerable<Element> Bake(Document doc);
    }
}
