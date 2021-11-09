using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Otus.Teaching.Concurrency.Import.Core.Repositories;
using Otus.Teaching.Concurrency.Import.Core.Entities;


namespace OtusREST.Controllers
{

    [ApiController]
    //[Route("[controller]")]
    [Route("Status")]
    public class MyStatusController : Controller
    {

        [HttpGet]
        public string Get()
        {
            return "This is status report page";
        }
    }
}
