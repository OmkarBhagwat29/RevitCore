
namespace RevitCore.ResidentialApartments.Validation
{
    public class AreaValidation : ISpatialValidation
    {
        public AreaValidation(SpatialElement spatialElement,
            double requiredArea)
        {
            this.SpatialElement = spatialElement;
            RequiredArea = requiredArea;
        }

        public SpatialElement SpatialElement { get; }
        public double RequiredArea { get; }
        public double AchievedArea { get; private set; }

        public ValidationAccuracy Accuracy { get; private set; }

        public bool ValidationSuccess { get; private set; }

        public IEnumerable<Element> Bake(Document doc)
        {
            return null;
        }

        public void Validate()
        {
            this.AchievedArea = this.SpatialElement.Area.ToUnit(UnitTypeId.SquareMeters);

            this.ValidationSuccess = this.AchievedArea >= this.RequiredArea;
        }
    }
}
