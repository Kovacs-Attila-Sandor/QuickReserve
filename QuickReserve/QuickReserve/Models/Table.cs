﻿using System;
using System.Collections.Generic;
using System.Text;

namespace QuickReserve.Models
{
    public class Table
    {
        public int TableId { get; set; }
        public int TableNumber { get; set; }
        public int Capacity { get; set; }
        public string AvailabilityStatus { get; set; }

        public Table() { }
    }
}
