namespace Latypova
{
    internal class Project
    {
        public string Description { get; }
        public DateTime Deadline { get; }
        public Person Customer { get; }
        public Person TeamLead { get; }
        public ProjectStatus Status { get; private set; }
        public List<ProjectTask> Tasks { get; } = new List<ProjectTask>();

        public Project(string description, DateTime deadline, Person customer, Person teamLead)
        {
            Description = description;
            Deadline = deadline;
            Customer = customer;
            TeamLead = teamLead;
            Status = ProjectStatus.Project;
        }

        public ProjectTask CreateTask(
            Person creator,
            string description,
            DateTime startDate,
            DateTime deadline,
            Person executor,
            ReportPeriodicity periodicity = ReportPeriodicity.OneTime,
            int? monthlyReportDay = null)
        {
            if (Status != ProjectStatus.Project)
                throw new InvalidOperationException("Задачи можно создавать только когда проект находится в статусе 'Проект'.");
            if (creator != TeamLead)
                throw new InvalidOperationException("Задачи по проекту может назначать только ответственный за проект (Тимлид).");

            var task = new ProjectTask(description, startDate, deadline, TeamLead, executor, periodicity, monthlyReportDay);
            Tasks.Add(task);
            Console.WriteLine($"Создана задача: '{description}' \nИсполнитель: {executor.Name}  \nТип: {periodicity} (отчетов: {task.RequiredReportsCount})");
            return task;
        }

        public void DeleteRejectedTask(Person initiator, ProjectTask task)
        {
            if (task.Initiator != initiator)
                throw new InvalidOperationException("Удалить задачу может только её инициатор.");
            if (task.Executor != null)
                throw new InvalidOperationException("Удалить можно только отклоненную задачу без исполнителя.");

            Tasks.Remove(task);
            Console.WriteLine($"Задача '{task.Description}' Удалена инициатором {initiator.Name}.");
            TryCloseProject();
        }

        public void StartExecution()
        {
            if (Tasks.Count == 0)
                throw new InvalidOperationException("Нельзя перевести пустой проект в статус 'Исполнение'.");

            Status = ProjectStatus.InProgress;
            Console.WriteLine($"\nПроект '{Description}' переведен в статус: исполнение\n");
        }

        public void TryCloseProject()
        {
            if (Status == ProjectStatus.InProgress && Tasks.All(t => t.Status == TaskStatus.Completed))
            {
                Status = ProjectStatus.Closed;
                Console.WriteLine($"\nВсе задачи выполнены! \nПроект '{Description}' переведен в статус: закрыт");
            }
        }
    }
}
