using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace KanbanBack.Controllers
{
    [Route("Kanban")]
    public class KanbanController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}