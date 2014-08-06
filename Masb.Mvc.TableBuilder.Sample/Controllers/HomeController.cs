using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Masb.Mvc.TableBuilder.Sample.Models;

namespace Masb.Mvc.TableBuilder.Sample.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return this.View(
                new TableViewModel
                {
                    Rows = new List<RowViewModel>
                    {
                        new RowViewModel
                        {
                            PersonName = "Miguel Angelo",
                            BirthDate = new DateTime(1984, 05, 04),
                            Gender = GenderKinds.Male,
                            MaleProp = "Male prop"
                        },
                        new RowViewModel
                        {
                            PersonName = "Maria Luiza",
                            BirthDate = new DateTime(1986, 09, 27),
                            Gender = GenderKinds.Female,
                            FemaleProp = "Female prop"
                        },
                    }
                });
        }

        public ActionResult Empty()
        {
            return this.View(
                "Index",
                new TableViewModel
                {
                    Rows = new List<RowViewModel>()
                });
        }

        public ActionResult Null()
        {
            return this.View(
                "Index",
                new TableViewModel());
        }

        public ActionResult NoSections()
        {
            return this.View(
                new TableViewModel
                {
                    Rows = new List<RowViewModel>
                    {
                        new RowViewModel
                        {
                            PersonName = "Miguel Angelo",
                            BirthDate = new DateTime(1984, 05, 04),
                            Gender = GenderKinds.Male,
                            MaleProp = "Male prop"
                        },
                        new RowViewModel
                        {
                            PersonName = "Maria Luiza",
                            BirthDate = new DateTime(1986, 09, 27),
                            Gender = GenderKinds.Female,
                            FemaleProp = "Female prop"
                        },
                    }
                });
        }

        public ActionResult TestMvcEditorTemplate()
        {
            return this.View(
                new TestNodeModel
                {
                    Name = "Root",
                    Children = new[]
                    {
                        new TestNodeModel { Name = "NodeA", },
                        new TestNodeModel { Name = "NodeB", },
                    }
                });
        }

        [HttpPost]
        public ActionResult TestMvcEditorTemplate(TestNodeModel viewModel)
        {
            return this.View(viewModel);
        }
    }
}