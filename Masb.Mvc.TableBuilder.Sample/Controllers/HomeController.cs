using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Masb.Mvc.TableBuilder.Sample.Models;

namespace Masb.Mvc.TableBuilder.Sample.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/

        public ActionResult Index()
        {
            return this.View(new TableViewModel
                             {
                                 Rows = new List<RowViewModel>
                                        {
                                            new RowViewModel
                                            {
                                                PersonName = "Miguel Angelo",
                                                BirthDate = new DateTime(1984, 05, 04)
                                            },
                                            new RowViewModel
                                            {
                                                PersonName = "Maria Luiza",
                                                BirthDate = new DateTime(1986, 09, 27)
                                            },
                                        }
                             });
        }
    }
}