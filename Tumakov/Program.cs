namespace Tumakov
{
    internal class Program
    {
        static public BankAccount CreateAcc()
        {
            Console.WriteLine("Выберите тип счета: \n1 - Сберегательный \n2 - Кредитный \n3 - Накопительный \n4 - Депозитный \n5 - Расчетный");
            AccType accType = (AccType)(int.TryParse(Console.ReadLine(), out int ty) ? ty : 0);
            Console.Clear();
            Console.Write("Введите текущий баланс: ");
            decimal balance = decimal.TryParse(Console.ReadLine(), out decimal bal) ? bal : 0;
            Console.Clear();
            BankAccount accOut = new BankAccount(accType, balance);
            return accOut;
        }

        static void Task1()
        {
            BankAccount testik = CreateAcc();
            byte isRunning = 1;
            while (isRunning == 1)
            {
                Console.WriteLine("Выберите действие: \n1 - Положить деньги на счет \n2 - Снять деньги со счета \n3 - Просмотреть информацию о счете \n4 - Выйти \n5 - Создать файл с историей транзакций");
                string action = Console.ReadLine();
                Console.Clear();
                switch (action)
                {
                    case "1":
                        Console.Write("Введите сумму для пополнения: ");
                        decimal putAmount = decimal.TryParse(Console.ReadLine(), out decimal put) ? put : 0;
                        testik.PutMoney(putAmount);
                        break;
                    case "2":
                        Console.Write("Введите сумму для снятия: ");
                        decimal takeAmount = decimal.TryParse(Console.ReadLine(), out decimal take) ? take : 0;
                        testik.TakeMoney(takeAmount);
                        break;
                    case "3":
                        testik.DisplayInfo();
                        break;
                    case "4":
                        isRunning = 0;
                        break;
                    case "5":
                        testik.Dispose();
                        break;
                    default:
                        Console.WriteLine("Некорректный ввод. Попробуйте снова.");
                        break;
                }
            }
        }

        static void Task2()
        {
            Song mySong = new Song();
            mySong.PrintInfo();
        }

        static void Main(string[] args)
        {
            byte runFlag = 1;
            while (runFlag == 1)
            {
                Console.WriteLine("Введите индекс задания: \n0. Выход \n1. Задание 1 \n2. Задание 2 \n3. Задание 3 \n4. Задание 4 \n5. Домашнее задание 1 \n6. Домашнее задание 2");
                string choice = Console.ReadLine();
                switch (choice)
                {
                    case "0":
                        runFlag = 0;
                        break;
                    case "1":
                        Task1();
                        break;
                    case "2":
                        Task2();
                        break;
                    default:
                        Console.WriteLine("Неверный выбор. Пожалуйста, попробуйте снова.");
                        break;
                }
            }
        }
    }
}