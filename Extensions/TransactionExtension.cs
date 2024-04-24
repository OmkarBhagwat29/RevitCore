

using Autodesk.Revit.UI;

namespace RevitCore.Extensions
{
    public static class TransactionExtension
    {
        public static void UseTransaction(this Document doc, Action doAction, string transactionName = "Default")
        {

            using (var t = new Transaction(doc,transactionName))
            {
                try
                {
                    t.Start();
                    doAction.Invoke();
                    t.Commit();
                }
                catch
                {
                    t.RollBack();
                }
            }
        }

        public static TReturn UseTransaction<TReturn>(this Document doc, Func<TReturn> doAction,
            string transactionName = "Default")
        {
            TReturn output = default;

            using (Transaction t = new Transaction(doc, transactionName))
            {
                try
                {
                    t.Start();
                    output = doAction.Invoke();
                    t.Commit();
                }
                catch
                {
                    t.RollBack();
                }
            }

            return output;
        }
    }
}
