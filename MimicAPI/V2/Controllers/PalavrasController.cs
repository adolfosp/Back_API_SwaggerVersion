using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MimicAPI.V2.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/palavras")]
    [ApiVersion("2.0")]
    [ApiExplorerSettings(GroupName = "v2")]
    public class PalavrasController : ControllerBase
    {



        [HttpGet("", Name = "ObterTodasPalavras")]
        public string ObterTodasPalavras()
        {
            return "Versão 2.0";
        }
    }
}
