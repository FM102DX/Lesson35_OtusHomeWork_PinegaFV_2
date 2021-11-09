using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Otus.Teaching.Concurrency.Import.Core.Repositories;
using Otus.Teaching.Concurrency.Import.Core.Entities;
using Otus.Teaching.Concurrency.Import.Core.ViewModels;
using Otus.Teaching.Concurrency.Import.DataAccess.Data;
using Otus.Teaching.Concurrency.Import.Core.Service;

namespace OtusREST.Controllers
{

    [ApiController]
    [Route("/")]
    public class Log : Controller
    {
        
        IGenericRepository<ConsoleToApiMessage> _repo;

        public Log(IGenericRepository<ConsoleToApiMessage> repo)
        {
            _repo = repo;
        }
        
        [HttpGet]
        public IActionResult Get()
        {
            var viewModel = new LogViewModel()
            {
                MyMessageList = _repo.GetAllItems()
            };
            return View("LogIndexView", viewModel);
        }

        [HttpPost]
        public ActionResult AddUser(ConsoleToApiMessage msg)
        {
            CommonOperationResult rez = _repo.AddItem(msg);
            return Ok(rez);
            
        }
    }
}
