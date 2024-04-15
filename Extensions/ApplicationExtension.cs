using Autodesk.Revit.ApplicationServices;
using System.IO;


namespace RevitCore.Extensions
{
    public static class ApplicationExtension
    {
        public static DefinitionFile CreateShareParameterFile(this Application app,
            string shareParamFilePath)
        {
            if(app == null)throw new ArgumentNullException(nameof(app));

            if(shareParamFilePath == null) throw new ArgumentNullException(nameof(shareParamFilePath));

            if (File.Exists(shareParamFilePath))
                throw new ArgumentException($"You already have a file under given location: {shareParamFilePath}");

            using (File.Create(shareParamFilePath))
            {
                app.SharedParametersFilename = shareParamFilePath;
            }

            return app.OpenSharedParameterFile();   
        }
    }
}
