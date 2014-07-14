using System;

namespace Masb.Mvc.TableBuilder.Sample.Models
{
    public class RowViewModel
    {
        public string PersonName { get; set; }

        public DateTime BirthDate { get; set; }

        public TimeSpan Age
        {
            get { return DateTime.Now - this.BirthDate; }
            set { this.BirthDate = DateTime.Now - value; }
        }
    }
}