
namespace RevitCore.ResidentialApartments.Validation
{
    public class AreaValidation : ISpatialValidation
    {
        public AreaValidation(double achievedArea,
            double requiredArea)
        {
            this.AchievedArea = achievedArea;
            RequiredArea = requiredArea;
        }

        public Type SpatialType { get; }
        public double RequiredArea { get; }
        public double AchievedArea { get; private set; }

        public bool ValidationSuccess { get; private set; } = false;

        public void Bake(Document doc)
        {

        }

        public string GetValidationReport()
        {
            if (this.AchievedArea < this.RequiredArea)
                return "Error: Achieved area is lesser than required area.";

            return string.Empty;
        }

        public void Validate()
        {
            this.ValidationSuccess = this.AchievedArea >= this.RequiredArea;
        }

        public bool IsGreaterThan(double percentage)
        {
            double percentArea = (percentage / 100.0) * this.RequiredArea;
            double difference = this.AchievedArea - percentArea;

            return difference > this.RequiredArea;
        }
    }
}
