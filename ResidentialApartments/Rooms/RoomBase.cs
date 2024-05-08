
using Autodesk.Revit.DB.Architecture;
using RevitCore.ResidentialApartments.Validation;

namespace RevitCore.ResidentialApartments.Rooms
{
    public abstract class RoomBase
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public double MinimumArea { get; set; }
        public double MinimumWidth { get; set; }

        public ElementId RevitRoomElementId { get; }

        public abstract Room Room { get; }

        public List<ISpatialValidation> RoomValidationData { get; } = [];

        public virtual void AddValidationData(ISpatialValidation apartmentValidation) => this.RoomValidationData.Add(apartmentValidation);
    }
}
