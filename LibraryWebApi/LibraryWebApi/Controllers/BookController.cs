using Library.Data.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Reflection;
using System.Diagnostics;
using System.Net.Http.Formatting;
using System.Threading.Tasks;
using LibraryWebApi.Extentions;
using Library.Service;
using Library.Data;
using LibraryWebApi.Models;

namespace LibraryWebApi.Controllers
{
    public class BookController : ApiController
    {
        private readonly ILibraryService assets;

        public BookController()
        {
            this.assets = new LibraryService(new LibraryContext("name=LibraryDB"));
        }

        // GET: api/Book
        public IEnumerable<DtoBook> Get()
        {
            return assets.GetBooks().Dto<DtoBook>();
        }

        // GET: api/Book/5
        public DtoBook Get(int id)
        {
            var asset = assets.GetById(id);
            if (!(asset is Book))
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            return asset.Dto<DtoBook>();
        }


        [HttpPost]
        public async Task<HttpResponseMessage> Post(HttpRequestMessage request)
        {
            if (!request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }

            var data = await Request.Content.ParseMultipartAsync();
            var asset = data.GetAsset<Book>();
            byte[] file = null;

            if (data.Files.Count>0)
            {
                file = data.Files["file"].File;
            }

            assets.AddAsset(asset, file);
            assets.Save();
            return new HttpResponseMessage(HttpStatusCode.OK);
        }



        // PUT: api/Book/5
        [HttpPut]
        public async Task<HttpResponseMessage> Put(HttpRequestMessage request)
        {
            if (!request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }

            var data = await Request.Content.ParseMultipartAsync();
            var asset = data.GetAsset<Book>();

            byte[] file = null;

            if (data.Files.Count > 0)
            {
                file = data.Files["file"].File;
            }

            assets.UpdateAsset(asset, file);

            assets.Save();
            return new HttpResponseMessage(HttpStatusCode.OK);

        }

    }
}
