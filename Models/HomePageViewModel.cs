using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SimplestTwilio.Models
{
    public class HomePageViewModel
    {
        public List<RecipientList> TopRecipientLists { get; set; }
        public List<Message> TopMessages { get; set; }
        public List<History> TopHistory { get; set; }
    }
}
