using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kanban
{
    public class Task
    {
        public enum eStatus { TODO, DOING, DONE }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public long Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime? EndTime { get; set; }
        public eStatus Status { get; set; }
        public long? BoardId { get; set; }
        public Board Board { get; set; }
        [NotMapped]
        public string EndDateAsString
        {
            get
            {
                if (EndTime == null)
                {
                    return "No end time";
                }
                else
                {
                    return EndTime.Value.ToShortDateString();
                }
            }
        }
    }
}
