namespace Latypova
{
    public class ProjectTask
    {
        public string Description { get; }
        public DateTime StartDate { get; }
        public DateTime Deadline { get; }
        public Person Initiator { get; }
        public Person Executor { get; private set; }
        public TaskStatus Status { get; private set; }

        public ReportPeriodicity Periodicity { get; }
        public int? MonthlyReportDay { get; }
        public int RequiredReportsCount { get; }
        public List<Report> ApprovedReports { get; } = new List<Report>();
        public Report CurrentReportOnReview { get; private set; }

        public ProjectTask(
            string description,
            DateTime startDate,
            DateTime deadline,
            Person initiator,
            Person executor,
            ReportPeriodicity periodicity = ReportPeriodicity.OneTime,
            int? monthlyReportDay = null)
        {
            if (deadline < startDate)
                throw new ArgumentException("Срок окончания задачи не может быть раньше даты начала.");

            if (periodicity == ReportPeriodicity.Monthly && (monthlyReportDay == null || monthlyReportDay < 1 || monthlyReportDay > 31))
                throw new ArgumentException("Для ежемесячной задачи нужно указать корректный день месяца (от 1 до 31).");

            Description = description;
            StartDate = startDate.Date;
            Deadline = deadline.Date;
            Initiator = initiator;
            Executor = executor;
            Periodicity = periodicity;
            MonthlyReportDay = monthlyReportDay;
            Status = TaskStatus.Assigned;

            RequiredReportsCount = CalculateRequiredReports();
        }

        private int CalculateRequiredReports()
        {
            if (Periodicity == ReportPeriodicity.OneTime)
                return 1;

            int count = 0;

            if (Periodicity == ReportPeriodicity.EveryWorkDay)
            {
                for (DateTime date = StartDate; date <= Deadline; date = date.AddDays(1))
                {
                    if (date.DayOfWeek != DayOfWeek.Saturday && date.DayOfWeek != DayOfWeek.Sunday)
                        count++;
                }
            }
            else if (Periodicity == ReportPeriodicity.Weekly)
            {
                int totalDays = (Deadline - StartDate).Days + 1;
                count = (int)Math.Ceiling(totalDays / 7.0);
            }
            else if (Periodicity == ReportPeriodicity.Monthly)
            {
                DateTime currentMonth = new DateTime(StartDate.Year, StartDate.Month, 1);
                DateTime endMonth = new DateTime(Deadline.Year, Deadline.Month, 1);

                while (currentMonth <= endMonth)
                {
                    int daysInMonth = DateTime.DaysInMonth(currentMonth.Year, currentMonth.Month);
                    int actualDay = Math.Min(MonthlyReportDay.Value, daysInMonth);
                    DateTime reportDate = new DateTime(currentMonth.Year, currentMonth.Month, actualDay);

                    if (reportDate >= StartDate && reportDate <= Deadline)
                        count++;

                    currentMonth = currentMonth.AddMonths(1);
                }
            }

            return Math.Max(1, count);
        }

        public void TakeInWork(Person person)
        {
            if (Executor != person)
                throw new InvalidOperationException("Взять задачу в работу может только назначенный исполнитель.");
            if (Status != TaskStatus.Assigned)
                throw new InvalidOperationException("Взять в работу можно только задачу в статусе 'Назначена'.");

            Status = TaskStatus.InProgress;
            Console.WriteLine($"Задача '{Description}' \n{person.Name} взял(а) задачу в работу (требуется отчетов: {RequiredReportsCount}).\n");
        }

        public void DelegateTo(Person currentExecutor, Person newExecutor)
        {
            if (Executor != currentExecutor)
                throw new InvalidOperationException("Делегировать задачу может только текущий исполнитель.");

            Executor = newExecutor;
            Status = TaskStatus.Assigned;
            Console.WriteLine($"Задача '{Description}' \n{currentExecutor.Name} делегировал(а) задачу {newExecutor.Name}. \nСтатус: Назначена.\n");
        }

        public void Reject(Person currentExecutor)
        {
            if (Executor != currentExecutor)
                throw new InvalidOperationException("Отклонить задачу может только текущий исполнитель.");

            Console.WriteLine($"Задача '{Description}' \n{currentExecutor.Name} отклонил(а) задачу. \nИсполнитель снят.\n");
            Executor = null;
            Status = TaskStatus.Assigned;
        }

        public void Reassign(Person initiator, Person newExecutor)
        {
            if (Initiator != initiator)
                throw new InvalidOperationException("Переназначить задачу может только её инициатор.");
            if (Executor != null)
                throw new InvalidOperationException("Переназначить можно только отклоненную задачу без исполнителя.");

            Executor = newExecutor;
            Status = TaskStatus.Assigned;
            Console.WriteLine($"Задача '{Description}' \nИнициатор {initiator.Name} назначил задачу {newExecutor.Name}.");
        }

        public void SubmitReport(Person executor, string reportText)
        {
            if (Executor != executor)
                throw new InvalidOperationException("Отправить отчет может только текущий исполнитель.");
            if (Status != TaskStatus.InProgress)
                throw new InvalidOperationException("Сдать отчет можно только по задаче в статусе 'В работе'.");

            CurrentReportOnReview = new Report(reportText, DateTime.Now, executor);
            Status = TaskStatus.UnderReview;
            int reportNumber = ApprovedReports.Count + 1;
            Console.WriteLine($"Задача '{Description}' \n{executor.Name} отправил(а) отчет #{reportNumber}/{RequiredReportsCount}: \"{reportText}\"\n");
        }

        public void ReviewReport(Person initiator, bool isApproved)
        {
            if (Initiator != initiator)
                throw new InvalidOperationException("Проверять отчет может только инициатор задачи.");
            if (Status != TaskStatus.UnderReview)
                throw new InvalidOperationException("Задача не находится на проверке.");

            if (isApproved)
            {
                ApprovedReports.Add(CurrentReportOnReview);
                CurrentReportOnReview = null;

                if (ApprovedReports.Count >= RequiredReportsCount)
                {
                    Status = TaskStatus.Completed;
                    Console.WriteLine($"Задача '{Description}' \nОтчет утвержден ({ApprovedReports.Count}/{RequiredReportsCount}). \nЗадача полностью выполнена.\n");
                }
                else
                {
                    Status = TaskStatus.InProgress;
                    Console.WriteLine($"Задача '{Description}' \nОтчет утвержден ({ApprovedReports.Count}/{RequiredReportsCount}). \nЗадача остается в работе до следующего отчета.\n");
                }
            }
            else
            {
                CurrentReportOnReview = null;
                Status = TaskStatus.InProgress;
                Console.WriteLine($"Задача '{Description}' \nИнициатор {initiator.Name} вернул отчет на доработку. \nСтатус: В работе.\n");
            }
        }
    }
}
