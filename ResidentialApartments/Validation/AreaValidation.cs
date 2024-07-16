
namespace RevitCore.ResidentialApartments.Validation
{
    public class AreaValidation : ISpatialValidation
    {
        public AreaValidation(double achievedArea,
            double requiredArea, bool checkForAreaGreater = false)
        {
            this.AchievedArea = achievedArea;
            this.RequiredArea = requiredArea;
            this.CheckForRequiredAreaGreater = checkForAreaGreater;
        }

        public bool CheckForRequiredAreaGreater { get; }

        public Type SpatialType { get; }
        public double RequiredArea { get; }
        public double AchievedArea { get; private set; }

        public bool ValidationSuccess { get; private set; } = false;

        public void Bake(Document doc)
        {

        }

        public string GetValidationReport()
        {
            if (!this.CheckForRequiredAreaGreater)
            {
                if (!this.ValidationSuccess)
                    return "Error: Achieved area is lesser than required area.";
            }
            else
            {
                if (!this.ValidationSuccess)
                    return "Error: Achieved area can not be greater than required area.";
            }
           

            return string.Empty;
        }

        public void Validate()
        {
            if (!this.CheckForRequiredAreaGreater)
            {
                this.ValidationSuccess = this.AchievedArea >= this.RequiredArea;
            }
            else
            {
                this.ValidationSuccess = this.AchievedArea < this.RequiredArea;
            }
        }

        public bool IsGreaterThan(double percentage)
        {
            double percentArea = (percentage / 100.0) * this.RequiredArea;
            double difference = this.AchievedArea - percentArea;

            return difference > this.RequiredArea;
        }
    }
}
