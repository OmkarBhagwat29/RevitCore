

namespace RevitCore.Extensions
{
    public enum View3DType
    {
        Perspective,
        Isometric
    }
    public static class ViewExtension
    {
        public static ViewPlan CreateViewPlan(this Document doc, ElementId viewFamilyTypeId,
            ElementId levelId, string viewPlanName)
        {
            var vP = ViewPlan.Create(doc, viewFamilyTypeId, levelId);
            vP.Name = viewPlanName;

            return vP;
        }

        public static View3D CreateView3D(this Document doc, View3DType view3dType, string viewName)
        {
            var viewFamilyType = doc.GetElements<ViewFamilyType>(e => e.ViewFamily == ViewFamily.ThreeDimensional)
                .FirstOrDefault();

            if (viewFamilyType == null) throw new ArgumentNullException(nameof(viewFamilyType));

                View3D view;

                if (view3dType == View3DType.Perspective)
                    view = View3D.CreatePerspective(doc, viewFamilyType.Id);
                else
                    view = View3D.CreateIsometric(doc, viewFamilyType.Id);

                view.Name = viewName;

                return view;
        }

        public static ViewDrafting CreateDraftingView(this Document doc, string viewName)
        {

            var viewFamilyType = doc.GetElements<ViewFamilyType>(e => e.ViewFamily == ViewFamily.Drafting)
            .FirstOrDefault();

            if (viewFamilyType == null) throw new ArgumentNullException(nameof(viewFamilyType));

                ViewDrafting view = ViewDrafting.Create(doc, viewFamilyType.Id);

                view.Name = viewName;

                return view;

        }

        public static View CreateLegendView(this Document doc, View existingLegendView, string viewName)
        {
            View newLegendView = doc.GetElement(existingLegendView.Duplicate(ViewDuplicateOption.Duplicate)) as View;
            if(newLegendView == null) throw new ArgumentNullException(nameof(newLegendView));

            newLegendView.Name = viewName;
            return newLegendView;
        }
    }
}
