


namespace RevitCore.ResidentialApartments.Validation
{
    public class AggregateAreaValidation : ISpatialValidation
    {
        public AggregateAreaValidation(List<SpatialElement> spatialElements,
            List<double> requiredAreas)
        {
            SpatialElements = spatialElements;
            RequiredAreas = requiredAreas;
        }
        public List<SpatialElement> SpatialElements { get; }
        public List<double> RequiredAreas { get; }

        public List<double> AchievedAreas { get; set; }

        public List<bool> ValidationResults { get; private set; } = [];

        public ValidationAccuracy Accuracy { get; private set; }

        public IEnumerable<Element> Bake(Document doc)
        {
            return null;
        }

        public void Validate()
        {
            this.AchievedAreas = this.SpatialElements
                .Select(s => s.Area.ToUnit(UnitTypeId.SquareMeters))
                .ToList();

            this.SetClosestPassingArea(this.AchievedAreas,this.RequiredAreas);

            Accuracy = ValidationAccuracy.Accurate;
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
                }
                else
                {
                    // If passing area found, remove it from the lists
                    currentAreas = RemoveElement(currentAreas, closestPassingArea);
                    requiredAreas = RemoveElement(requiredAreas, requiredArea);

                    this.ValidationResults.Add(true);
                }
            }
        }

        private List<double> RemoveElement(List<double> array, double element)
        {
            List<double> tempList = new List<double>(array);
            tempList.Remove(element);
            return tempList.ToList();
        }
    }
}
