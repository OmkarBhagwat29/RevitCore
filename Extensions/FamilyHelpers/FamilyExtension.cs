using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevitCore.Extensions.FamilyHelpers
{
    public static class FamilyExtension
    {
        public static IEnumerable<FamilyInstance> GetFamilyInstances(this Family family, Document doc)
        {
            var instances = doc.GetInstancesOfCategory(family.FamilyCategory.BuiltInCategory,
                (e) =>
                {
                    if (e is not FamilyInstance fi)
                        return false;

                    return fi.Symbol.FamilyName == family.Name;
                })
                .Cast<FamilyInstance>();    

            return instances;
        }
    }
}
