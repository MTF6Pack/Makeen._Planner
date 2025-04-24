namespace Domain
{
    public class Instance
    {
        public Guid Id { get; set; }
        public DateTime OccurrenceDate { get; private set; }
        public bool IsCompleted { get; private set; } = false;

        public Instance(DateTime date)
        {
            OccurrenceDate = date;
        }

        public void MarkAsCompleted(bool wasCompleted = true)
        {
            IsCompleted = wasCompleted;
        }

        public Instance()
        {

        }
    }
}