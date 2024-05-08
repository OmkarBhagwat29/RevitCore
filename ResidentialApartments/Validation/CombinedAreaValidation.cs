

namespace RevitCore.ResidentialApartments.Validation
{
    public class CombinedAreaValidation(List<SpatialElement> spatialElements, double requiredArea) : ISpatialValidation
    {
        public ValidationAccuracy Accuracy { get; private set; }
        public List<SpatialElement> SpatialElements { get; } = spatialElements;
        public double RequiredArea { get; } = requiredArea;
        public List<double> AchievedArea { get; private set; } = [];
        public bool ValidationSuccess { get; private set; }

        public IEnumerable<Element> Bake(Document doc)
        {
            return null;
        }

        public void Validate()
        {
            this.SpatialElements.ForEach(e => 
            this.AchievedArea.Add(e.Area.ToUnit(UnitTypeId.SquareMeters)));

            var sum = this.AchievedArea.Sum();

            this.Accuracy = ValidationAccuracy.Accurate;

            this.ValidationSuccess = sum >= this.RequiredArea;
        }
    }
}
