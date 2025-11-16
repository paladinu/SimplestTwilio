using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SimplestTwilio.Models
{
    public class History
    {
        public int HistoryId { get; set; }
        public int MessageId { get; set; }
        public int RecipientListId { get; set; }
    }
}
