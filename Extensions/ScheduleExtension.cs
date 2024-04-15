

namespace RevitCore.Extensions
{
    public static class ScheduleExtension
    {
        public static ViewSchedule CreateScheduleByCategory(this Document doc,BuiltInCategory category,
            List<ElementId> parameters, string scheduleName = null)
        {

            var vs = ViewSchedule.CreateSchedule(doc, new ElementId(category));

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
