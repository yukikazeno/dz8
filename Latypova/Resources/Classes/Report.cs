namespace Latypova
{
    public class Report
    {
        public string Text { get; }
        public DateTime CompletionDate { get; }
        public Person Executor { get; }

        public Report(string text, DateTime completionDate, Person executor)
        {
            Text = text;
            CompletionDate = completionDate;
            Executor = executor;
        }
    }
}
