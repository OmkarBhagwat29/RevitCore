
using RevitCore.GeometryUtils;

namespace RevitCore.ResidentialApartments.Validation
{
    public class DimensionValidation : ISpatialValidation
    {
        public DimensionValidation(SpatialElement spatialElement,
            double requiredMinWidth)
        {
            this.SpatialElement = spatialElement;
            RequiredMinWidth = requiredMinWidth;
            //AchievedMinWidth = achievedMinWidth;
        }
        public SpatialElement SpatialElement { get; }

        public double RequiredMinWidth { get; }
        public double AchievedMinWidth { get; private set; }

        public bool ValidationSuccess { get; private set; } = false;

        public ValidationAccuracy Accuracy { get; private set; }

        public IEnumerable<Element> Bake(Document doc)
        {
            return null;
        }

        public void Validate()
        {

            var bbx = this.SpatialElement.get_BoundingBox(this.SpatialElement.Document.ActiveView);
            if(bbx == null) return; 
            var advanceBbx = bbx.ToAdvanced();

            var length = advanceBbx.Length.ToMeters();
            var width = advanceBbx.Width.ToMeters();
            var bbxArea = length * width;
            var roomArea = this.SpatialElement.Area.ToUnit(UnitTypeId.SquareMeters);
            if (bbxArea == roomArea)
                this.Accuracy = ValidationAccuracy.Accurate;
            else
                this.Accuracy = ValidationAccuracy.Not_Accurate;

            this.AchievedMinWidth = length < width ? length : width;

            this.ValidationSuccess = this.AchievedMinWidth >= this.RequiredMinWidth;
        }
    }
}
