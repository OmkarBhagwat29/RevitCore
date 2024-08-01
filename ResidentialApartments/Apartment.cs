using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using RevitCore.Extensions;
using RevitCore.ResidentialApartments.Rooms;
using RevitCore.ResidentialApartments.Validation;


namespace RevitCore.ResidentialApartments
{

    public abstract class Apartment
    {

        public string Name { get; set; }


        public string Description { get; set; }


        public int Occupancy { get; set; }


        public abstract ApartmentType Type { get; }


        public Area AreaBoundary { get; set; }

        //public Solid ApartmentSolid { get; set; }


        public List<RoomBase> Rooms { get; } = [];

        public  List<Element> ApartmentElements { get; } = [];

        public List<ISpatialValidation> ApartmentValidationData { get; } = [];

        public virtual void Validate()
        {
            //apartment level validation
            this.ApartmentValidationData.ForEach(d => d.Validate());


            foreach (var rm in this.Rooms)
            {
                foreach (var vD in rm.RoomValidationData)
                {
                   vD.Validate();
                }
            }
        }

        public virtual void Bake(Document doc)
        {
                foreach (var apartment in ApartmentValidationData)
                {
                    apartment.Bake(doc);
                }

                foreach (var room in Rooms)
                {
                    foreach (var validationData in room.RoomValidationData)
                    {
                        validationData.Bake(doc);
                    }
                }
        }

        public virtual void AddValidationData(ISpatialValidation apartmentValidation) => this.ApartmentValidationData.Add(apartmentValidation);

        public virtual RoomBase AddRoom(RoomBase roomBase, Func<RoomBase, bool> validate = null)
        {
            validate ??= (r) => true;

            if (validate(roomBase))
            {
                this.Rooms.Add(roomBase);
                return this.Rooms.Last();
            }
            return null;
        }

        public virtual RoomBase FindRoom(Func<List<RoomBase>, RoomBase> find)
        {
            return find(this.Rooms);
        }

        public List<FamilyInstance> GetSpecificEntities(Category category)
        {
            return this.ApartmentElements.Cast<FamilyInstance>().Where(e=>e.Category == category).ToList();
        }

        public List<FamilyInstance> GetSpecificEntities(BuiltInCategory category)
        {
#if REVIT2022
            return this.ApartmentElements.Cast<FamilyInstance>().Where(e => (BuiltInCategory)e.Category.Id.IntegerValue == category).ToList(); 
#else
            return this.ApartmentElements.Cast<FamilyInstance>().Where(e => e.Category.BuiltInCategory == category).ToList();
#endif
        }


        public bool IsWindowAssociatedToApartment(
            FamilyInstance element, double brickTolerance = 1.64042) //brick tolerance in feet
        {
            if (element.Location == null)
                return false;

            var loc = element.Location as LocationPoint;

            if (loc == null) return false;

            var areaBoundarySegments = this.AreaBoundary.GetBoundarySegments(new SpatialElementBoundaryOptions()
            { SpatialElementBoundaryLocation = SpatialElementBoundaryLocation.Center });

            double min = double.MaxValue;
            double requiredDistance = double.MaxValue;
            foreach (var segments in areaBoundarySegments)
            {
                foreach (var seg in segments)
                {
                    var cV = seg.GetCurve();

                    if (cV == null)
                        continue;

                    requiredDistance = cV.Distance(loc.Point);

                    if (requiredDistance < min)
                    {
                        min = requiredDistance;

                        if (requiredDistance <= brickTolerance)
                            return true;
                    }
                }
            }

            return false;
        }

    }
}
