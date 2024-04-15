

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
    }
}
