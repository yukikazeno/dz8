namespace Tumakov
{
    internal class FinalAccountInfo
    {
        public ulong AccountNumber { get; set; }
        public List<BankTransaction> TransactionHistory { get; set; }
    }
}
