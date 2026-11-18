namespace Latypova
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Person customer = new Person("ООО 'Вектор' (Заказчик)");
            List<Person> team = new List<Person>
            {
                new Person("Алексей (Тимлид)"),
                new Person("Мария"),
                new Person("Дмитрий"),
                new Person("Елена"),
                new Person("Иван"),
                new Person("Ольга"),
                new Person("Сергей"),
                new Person("Анна"),
                new Person("Павел"),
                new Person("Наталья")
            };

            Person teamLead = team[0];
            DateTime today = new DateTime(2026, 1, 1);

            Project project = new Project(
                "Разработка и поддержка ERP-системы",
                today.AddYears(1),
                customer,
                teamLead
            );

            Console.WriteLine($"Создан проект: '{project.Description}' (Статус: {project.Status})\n");

            project.CreateTask(teamLead, "Архитектура БД", today, today.AddDays(10), team[0], ReportPeriodicity.OneTime);
            project.CreateTask(teamLead, "Верстка главной страницы", today, today.AddDays(14), team[1], ReportPeriodicity.OneTime);
            project.CreateTask(teamLead, "Настройка CI/CD", today, today.AddDays(7), team[2], ReportPeriodicity.OneTime);
            project.CreateTask(teamLead, "Ежемесячный аудит безопасности", today, new DateTime(2026, 12, 31), team[3], ReportPeriodicity.Monthly, monthlyReportDay: 15);
            project.CreateTask(teamLead, "Еженедельное тестирование релизов", today, today.AddDays(20), team[4], ReportPeriodicity.Weekly);
            project.CreateTask(teamLead, "Мониторинг логов сервера", new DateTime(2026, 1, 5), new DateTime(2026, 1, 11), team[5], ReportPeriodicity.EveryWorkDay);

            for (int i = 6; i < team.Count; i++)
            {
                project.CreateTask(teamLead, $"Разработка микросервиса #{i}", today, today.AddDays(15), team[i], ReportPeriodicity.OneTime);
            }

            project.StartExecution();

            var task0 = project.Tasks[0];
            var task1 = project.Tasks[1];
            var task2 = project.Tasks[2];
            var monthlyTask = project.Tasks[3];

            task1.DelegateTo(team[1], team[4]);

            task2.Reject(team[2]);
            task2.Reassign(teamLead, team[5]);

            task0.TakeInWork(team[0]);
            task0.SubmitReport(team[0], "Черновая схема БД");
            task0.ReviewReport(teamLead, isApproved: false);
            task0.SubmitReport(team[0], "Финальная схема БД");
            task0.ReviewReport(teamLead, isApproved: true);

            Console.WriteLine("\nДемонстрация периодической задачи (12 ежемесячных отчетов)");
            monthlyTask.TakeInWork(team[3]);
            for (int month = 1; month <= monthlyTask.RequiredReportsCount; month++)
            {
                monthlyTask.SubmitReport(team[3], $"Отчет об аудите за {month}-й месяц");
                monthlyTask.ReviewReport(teamLead, isApproved: true);
            }

            Console.WriteLine("\nВыполнение всех оставшихся задач команды");
            foreach (var task in project.Tasks.Where(t => t.Status != TaskStatus.Completed))
            {
                Person worker = task.Executor;
                task.TakeInWork(worker);

                while (task.Status != TaskStatus.Completed)
                {
                    int currentNum = task.ApprovedReports.Count + 1;
                    task.SubmitReport(worker, $"Отчет #{currentNum} по задаче '{task.Description}'");
                    task.ReviewReport(teamLead, isApproved: true);
                }
            }

            project.TryCloseProject();
        }
    }
}