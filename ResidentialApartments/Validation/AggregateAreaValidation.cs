


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
                StringBuilder sB = new StringBuilder();

                for (int i = 0; i < this.AchievedAreas.Count; i++)
                {
                    var achievedArea = this.AchievedAreas[i];
                    var passed = this.ValidationResults[i];

                    if (passed)
                        continue;

                    string message = $"Error: Achieved Area is lesser than any of Required Areas.\n";
                    sB.Append(message) ;
                }

                return sB.ToString();
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
                this.SetClosestPassingArea(this.AchievedAreas, this.RequiredAreas);

            }
            catch (Exception)
            {
                this.ValidationSuccess = false;
            }

        }

        private void SetClosestPassingArea(List<double> currentAreas,
            List<double> requiredAreas)
        {
           // List<double>result = new List<double>();

            //iterate through each required area
            foreach (var requiredArea in requiredAreas)
            {
                double closestPassingArea = double.MaxValue;
                double closestPassingDiff = double.MaxValue;

                //find the closest passing area for the current required area
                foreach (var currentArea in currentAreas)
                {
                    double diff = Math.Abs(currentArea - requiredArea);
                    if (currentArea >= requiredArea && diff < closestPassingDiff)
                    {
                        closestPassingArea = currentArea;
                        closestPassingDiff = diff;
                    }
                }

                // If no passing area found, add the required area as it is
                if (closestPassingArea == double.MaxValue)
                {
                    this.ValidationResults.Add(false);
                    this.ClosestPassingAreas.Add(double.MaxValue);
                }
                else
                {
                    // If passing area found, remove it from the lists
                    currentAreas = RemoveElement(currentAreas, closestPassingArea);
                    requiredAreas = RemoveElement(requiredAreas, requiredArea);

                    this.ValidationResults.Add(true);
                    this.ClosestPassingAreas.Add(closestPassingArea);
                }
            }

            if (this.ValidationResults.Count > 0)
            {
                if (this.ValidationResults.Contains(false))
                    this.ValidationSuccess = false;

            }
        }

        private static List<double> RemoveElement(List<double> array, double element)
        {
            List<double> tempList = new List<double>(array);
            tempList.Remove(element);
            return tempList;
        }

    }
}
