

using RevitCore.ResidentialApartments.Rooms;

namespace RevitCore.ResidentialApartments.Validation
{
    public class CombinedAreaValidation(List<double> achievedRoomAreas, double requiredArea, Type spatialType) : ISpatialValidation
    {
        public Type SpatialType { get { return spatialType; } }
        public List<double> AchievedRoomAreas { get; } = achievedRoomAreas;

        public double CombinedArea { get; private set; }

        public double RequiredArea { get; } = requiredArea;
        public bool ValidationSuccess { get; private set; }

        public void Bake(Document doc)
        {

        }

        public string GetValidationReport()
        {
            this.CombinedArea = this.AchievedRoomAreas.Sum();

            if (this.CombinedArea < this.RequiredArea)
                return $"Error: Achieved area is lesser than required area";

            return string.Empty;
        }

        public void Validate()
        {
            this.CombinedArea = this.AchievedRoomAreas.Sum();

            this.ValidationSuccess = Math.Round(this.CombinedArea,2) >= this.RequiredArea;
        }
    }
}
