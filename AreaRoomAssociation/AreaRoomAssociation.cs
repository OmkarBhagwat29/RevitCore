using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;
using RevitCore.Extensions.Selection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevitCore.AreaRoomAssociation
{
    public class AreaRoomAssociation
    {


        public AreaRoomAssociation(List<ModelLine> apartmentBoundaryLines, List<SpatialElement>spatialElements) {
            ApartmentBoundaryLines = apartmentBoundaryLines;
            SpatialElements = spatialElements;

            this.InIt();
        }

        public List<ModelLine> ApartmentBoundaryLines { get; }

        public CurveArray ApartmentBoundary { get; } = new CurveArray();

        private readonly XYZ Min, Max;

        public List<SpatialElement> SpatialElements { get; }

        private void InIt()
        {
            this.ApartmentBoundaryLines.ForEach(mL => this.ApartmentBoundary.Append(mL.GeometryCurve as Line));

            
            foreach (var item in this.ApartmentBoundary)
            {
                var c = item as Line;
            }

        }

        public bool IsRoomInsideApartment(SpatialElement spatialElement)
        {

            var locPt = spatialElement.Location as LocationPoint;

            if(locPt == null) return false;

             // not on same plane
             if(locPt.Point.Z !=
                this.ApartmentBoundary.get_Item(0).GetEndPoint(0).Z) return false;

            foreach (var item in this.ApartmentBoundary)
            {
                Line ln = item as Line;

                var startPt = ln.GetEndPoint(0);
                var endPt = ln.GetEndPoint(1);

                
            }

            return true;
        }

        public static AreaRoomAssociation CreateAreaRoomAssociationBySelection(UIDocument uiDoc)
        {
            var areaRoomElements = uiDoc.PickElements((e) =>  e is SpatialElement ||
            e.GetType() == typeof(Area),
                PickElementOptionFactory.CreateCurrentDocumentOption());

            //distinguish 
            var areas = areaRoomElements.Where(e=>e.GetType()==typeof(Area))
                .Cast<Area>()
                .ToList();

            var rooms = areaRoomElements.Where(e => e is SpatialElement);

            // Specify boundary options
            SpatialElementBoundaryOptions options = new SpatialElementBoundaryOptions();
            options.SpatialElementBoundaryLocation = SpatialElementBoundaryLocation.Center;

            foreach (var area in areas)
            {
                var loops = area.GetBoundarySegments(options);
            }

            return null;
        }
    }
}
