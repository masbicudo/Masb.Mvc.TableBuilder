using System;

namespace Masb.Mvc.TableBuilder.Sample.Models
{
    public class RowViewModel
    {
        public string PersonName { get; set; }

        public DateTime BirthDate { get; set; }

        public GenderKinds Gender { get; set; }

        public string MaleProp { get; set; }

        public string FemaleProp { get; set; }

        public TimeSpan Age
        {
            get { return DateTime.Now - this.BirthDate; }
            set { this.BirthDate = DateTime.Now - value; }
        }
    }

    public enum GenderKinds
    {
        Male,
        Female,
    }
}