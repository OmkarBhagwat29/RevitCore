

namespace RevitCore.Extensions
{
    public static class ScheduleExtension
    {
        public static ViewSchedule CreateScheduleByCategory(this Document doc,BuiltInCategory category,
            List<ElementId> parameters, string scheduleName = null, ElementId areaSchemeId = null)
        {
            ViewSchedule vs;

            if (category != BuiltInCategory.OST_Areas)
                vs = ViewSchedule.CreateSchedule(doc, new ElementId(category));
            else
            {
                if (areaSchemeId == null)
                    throw new ArgumentNullException("no area scheme provided");

                vs = ViewSchedule.CreateSchedule(doc, new ElementId(category), areaSchemeId);
            }

            doc.Regenerate();

            vs.AddRegularFieldsToSchedules(parameters);

            if (scheduleName != null && scheduleName != string.Empty)
                vs.Name = scheduleName;

            return vs;
        }

        private static void AddRegularFieldsToSchedules(this ViewSchedule viewSchedule, List<ElementId> paramIds)
        {
            ScheduleDefinition definition = viewSchedule.Definition;
            var schedulableFields = definition.GetSchedulableFields()
                .Where(sf => paramIds.Contains(sf.ParameterId));

            schedulableFields.ToList().ForEach(sf => definition.AddField(sf));
        }

    }
}
