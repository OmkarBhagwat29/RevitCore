


using RevitCore.ResidentialApartments.Rooms;
using System.Text;
using System.Windows.Data;

namespace RevitCore.ResidentialApartments.Validation
{
    public class AggregateAreaValidation : ISpatialValidation
    {
        public AggregateAreaValidation(List<double> achievedAreas,
            List<double> requiredAreas, Type spatialType)
        {
            AchievedAreas = achievedAreas;
            RequiredAreas = requiredAreas;
            SpatialType = spatialType;
        }

        public Type SpatialType { get; }
        public List<double> RequiredAreas { get; } = [];

        public List<double> AchievedAreas { get; private set; } = [];

        public List<double> ClosestPassingAreas { get; } = [];

        public List<bool> ValidationResults { get; private set; } = [];

        public bool ValidationSuccess { get; private set; } = false;

        public string GetValidationReport()
        {
            if (!this.ValidationSuccess)
            {
               return $"Error: Achieved Area is lesser than any of Required Areas.";
            }

            return string.Empty;
        }

        public void Bake(Document doc)
        {
        }

        public void Validate()
        {
            try
            {
               
                this.ValidationResults.Clear();
                for (int i = 0; i < this.AchievedAreas.Count; i++)
                {
                    var achArea = this.AchievedAreas[i];
                    var reqArea = this.RequiredAreas[i];

                    this.ValidationResults.Add(achArea > reqArea);
                }

                if (this.ValidationResults.Contains(false))
                    this.ValidationSuccess = false;
                else
                    this.ValidationSuccess = true;

            }
            catch (Exception)
            {
                this.ValidationSuccess = false;
            }

        }


    }
}
