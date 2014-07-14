using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Masb.Mvc.TableBuilder.Models;

namespace Masb.Mvc.TableBuilder.Controllers
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
                                                BirthData = new DateTime(1984, 05, 04)
                                            },
                                            new RowViewModel
                                            {
                                                PersonName = "Maria Luiza",
                                                BirthData = new DateTime(1986, 09, 27)
                                            },
                                        }
                             });
        }
    }
}