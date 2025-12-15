
namespace Common.Contracts
{
    public class TotoNotificationRecord
    {
        public int Id { get; set; }
        public string Title { get; set; }

        // Parameterless constructor required by MassTransit
        public TotoNotificationRecord() { }

        public TotoNotificationRecord(int id, string title)
        {
            Id = id;
            Title = title;
        }
    }

}
