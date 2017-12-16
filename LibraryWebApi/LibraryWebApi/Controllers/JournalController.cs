using Library.Data;
using Library.Data.Models;
using Library.Service;
using LibraryWebApi.Extentions;
using LibraryWebApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace LibraryWebApi.Controllers
{
    public class JournalController : BaseApiController
    {
        
        // GET: api/Journal
        public IEnumerable<DtoJournal> Get()
        {
            return assets.GetJournals().Dto<DtoJournal>();
        }

        // GET: api/Journal/5
        public DtoJournal Get(int id)
        {
            var asset = assets.GetById(id);
            if (!(asset is Journal))
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            return asset.Dto<DtoJournal>();
        }


        [HttpPost]
        public async Task<HttpResponseMessage> Post(HttpRequestMessage request)
        {
            if (!request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }

            var data = await Request.Content.ParseMultipartAsync();
            var asset = data.GetAsset<Journal>();
            byte[] file = null;

            if (data.Files.Count > 0)
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
            var asset = data.GetAsset<Journal>();

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
