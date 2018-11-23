using Luval.Orm.DataAnnotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace data_collector.Models
{
    [Table(Name = "Staging_TFS")]
    public class TFSItem
    {
        [AutoIncrement, Key]
        public int RowId { get; set; }
        public long Id { get; set; }
        public string TeamProject { get; set; }
        public string AreaPath { get; set; }
        public string WorkItemType { get; set; }
        public string Title { get; set; }
        public string AssignedTo { get; set; }
        public string State { get; set; }
        public string Tags { get; set; }
        public string BoardColumn { get; set; }
        public string BoardLane { get; set; }
        public string ChangedBy { get; set; }
        public DateTime? ChangedDate { get; set; }
        public DateTime? ClosedDate { get; set; }
        public string ClosingComments { get; set; }
        public string Description { get; set; }
        public float Effort { get; set; }
        public DateTime? FinishedDate { get; set; }
        public string Priority { get; set; }
        public string Reason { get; set; }
        public float RemainingWork { get; set; }
        public string ReproSteps { get; set; }
        public string Severity { get; set; }
        public DateTime? StartDate { get; set; }
        public string Resolution { get; set; }
        public DateTime? TargetDate { get; set; }
        public string ItearationPath { get; set; }
    }
}
