using System.ComponentModel.DataAnnotations;

namespace Masb.Mvc.TableBuilder.Sample.Models
{
    public class TestNodeModel
    {
        [Required]
        public string Name { get; set; }

        public TestNodeModel[] Children { get; set; }
    }
}