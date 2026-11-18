using System.Text.Json;

namespace Tumakov
{
    public class BankAccount
    {
        private static ulong _accountCounter = 1000000000000000;
        private Queue<BankTransaction> _transactionHistory = new Queue<BankTransaction>();
        private readonly ulong _accNumber;
        private AccType _accType;
        private decimal _balance;

        public ulong AccNumber
        {
            get { return _accNumber; }
        }

        public AccType AccType
        {
            get { return _accType; }
        }

        public decimal Balance
        {
            get { return _balance; }
        }

        public BankAccount() {_accNumber = GetAutoNumber();}

        public BankAccount(AccType AccType, decimal Balance)
        {
            _accNumber = GetAutoNumber();
            _accType = AccType;
            _balance = Balance;
        }

        public BankAccount(AccType AccType)
        {
            _accNumber = GetAutoNumber();
            _accType = AccType;
        }

        public BankAccount(decimal Balance)
        {
            _accNumber = GetAutoNumber();
            _balance = Balance;
        }

        private static ulong GetAutoNumber()
        {
            _accountCounter++;
            return _accountCounter;
        }

        public void DisplayInfo()
        {
            Console.WriteLine($"Account Number: {_accNumber} \nAccount Type: {_accType} \nBalance: {_balance} руб.");
        }

        public void PutMoney(decimal amount)
        {
            _balance += amount;
            BankTransaction transaction = new BankTransaction(amount);
            _transactionHistory.Enqueue(transaction);
        }

        public void TakeMoney(decimal amount)
        {
            if (_balance >= amount)
            {
                _balance -= amount;
                BankTransaction transaction = new BankTransaction(-1 * amount);
                _transactionHistory.Enqueue(transaction);
            }
            else
            {
                Console.WriteLine("Недостаточно средств на счете.");
            }
        }

        public void TransferMoney(BankAccount targetAccount, decimal amount)
        {
            if (_balance >= amount)
            {
                this.TakeMoney(amount);
                targetAccount.PutMoney(amount);
            }

            else
            {
                Console.WriteLine("Недостаточно средств на счете для перевода.");
            }
        }

        public void Dispose()
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                IncludeFields = true,
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            List<BankTransaction> transactionList = new List<BankTransaction>();

            while (_transactionHistory.Count > 0)
            {
                transactionList.Add(_transactionHistory.Peek());
                _transactionHistory.Dequeue();
            }

            FinalAccountInfo finalInfo = new FinalAccountInfo
            {
                AccountNumber = _accNumber,
                TransactionHistory = transactionList
            };

            string directoryPath = Path.Combine(Directory.GetCurrentDirectory(), "TransactionHistory");
            Directory.CreateDirectory(directoryPath);

            if (Directory.Exists(directoryPath))
            {
                string filePath = Path.Combine(directoryPath, $"TransactionHistory_{_accNumber}.json");
                string finalJson = JsonSerializer.Serialize(finalInfo, options);
                File.Create(filePath).Close();
                File.WriteAllText(filePath, finalJson);
            }
            GC.SuppressFinalize(this);
        }
    }
}