using System;
using System.Collections.Generic;
using System.Text;

namespace RevitCore.Extensions.Selection
{
    public static class PickElementOptionFactory
    {
        public static CurrentDocumentOption CreateCurrentDocumentOption()=>new CurrentDocumentOption();
        public static LinkDocumentOption CreateLinkableDocumentOption() => new LinkDocumentOption();    

        public static BothDocumentOption CreateBothDocumentOption() => new BothDocumentOption();

    }
}
