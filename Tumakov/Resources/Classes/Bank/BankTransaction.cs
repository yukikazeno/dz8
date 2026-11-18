namespace Tumakov
{
    public class BankTransaction
    {
        public readonly DateTime transactionDate;
        public readonly decimal transactionAmount;

        public BankTransaction(decimal amount)
        {
            transactionDate = DateTime.Now;
            transactionAmount = amount;
        }
    }
}
