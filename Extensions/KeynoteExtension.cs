using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevitCore.Extensions
{
    public static class KeynoteExtension
    {
        public static ExternalResourceLoadStatus LoadKeynoteFile(this Document doc, string keynoteFile)
        {
            // Create the ExternalResourceReference
            ModelPath modelPath = ModelPathUtils.ConvertUserVisiblePathToModelPath(keynoteFile);
            ExternalResourceReference extResourceRef = ExternalResourceReference.CreateLocalResource(doc,
                new ExternalResourceType(Guid.NewGuid()),
                modelPath, PathType.Absolute);
            // Set the keynote file path
            KeynoteTable keynoteTable = KeynoteTable.GetKeynoteTable(doc);

            KeyBasedTreeEntriesLoadResults loadResults = new KeyBasedTreeEntriesLoadResults();

            ExternalResourceLoadStatus status = keynoteTable.LoadFrom(extResourceRef, loadResults);

            return status;
        }
    }
}
