using System;
using System.Collections.Generic;
using System.Text;

namespace QuickReserve.Models
{
    public class Table
    {
        public string TableId { get; set; }
        public int TableNumber { get; set; }
        public int Capacity { get; set; }
        public string AvailabilityStatus { get; set; }
        public TableLocation Location { get; set; }
    }

    public class TableLocation
    {
        public int StartRow { get; set; }
        public int StartColumn { get; set; }
        public int EndRow { get; set; }
        public int EndColumn { get; set; }
    }
}
