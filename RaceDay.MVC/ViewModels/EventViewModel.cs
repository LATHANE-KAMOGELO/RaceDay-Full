
namespace RaceDay.MVC.ViewModels
{
    public class EventViewModel
    {
        public int EventId { get; set; }
        public string EventName { get; set; } = "";
        public string? Description { get; set; }
        public DateTime EventDate { get; set; }
        public string Location { get; set; } = "";
        public decimal Distance { get; set; }
        public string EventType { get; set; } = "";
        public bool IsPublished { get; set; }


    }
}
