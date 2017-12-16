using Library.Data;
using Library.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace LibraryWebApi.Controllers
{
    public class BaseApiController : ApiController
    {
        protected readonly ILibraryService assets;

        public BaseApiController()
        {
            this.assets = new LibraryService(new LibraryContext("name=LibraryDB"));
        }
    }
}
