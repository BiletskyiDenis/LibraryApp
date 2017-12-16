using Library.Data;
using Library.Service;
using LibraryWebApi.Models.Catalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Results;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Library.Data.Models;
using System.Net;
using LibraryWebApi.Extentions;
using System.Threading.Tasks;
using System.Net.Http;
using LibraryWebApi.Models;
using System.IO;
using System.Text.RegularExpressions;
using System.Net.Http.Headers;

namespace LibraryWebApi.Controllers
{
    public class AssetController : BaseApiController
    {
        [Route("api/RecentlyAdded/{type}")]
        [HttpPost]
        public IEnumerable<DtoRecentlyAdded> RecentlyAdded(string type)
        {
            var assetType = type.ToLower();

            if (assetType == "book")
            {
                return assets.GetBooks().DtoRecentlyAdded();
            }

            if (assetType == "journal")
            {
                return assets.GetJournals().DtoRecentlyAdded();
            }

            if (assetType == "brochure")
            {
                return assets.GetBrochures().DtoRecentlyAdded();
            }

            throw new HttpResponseException(HttpStatusCode.NotFound);

        }

        [Route("api/Details/{id}")]
        [HttpPost]
        public DtoDetailModel Details(int id)
        {
            var asset = assets.GetById(id);
            if (asset == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }
            var type = assets.GetType(id).ToString().ToLower();

            var dto = asset.Dto<DtoDetailModel>();
            dto.Type = type;
            return dto;
        }

        // GET: api/Asset
        public IEnumerable<AssetIndexListingModel> Get()
        {
            var assetModels = assets.GetAll();
            var listingResult = assetModels.Select(
                result => new AssetIndexListingModel
                {
                    Id = result.Id,
                    Title = result.Title,
                    Author = assets.GetAuthor(result.Id),
                    ImageUrl = result.ImageUrl,
                    Price = result.Price,
                    NumberOfCopies = result.NumbersOfCopies,
                    Type = assets.GetType(result.Id).ToString(),
                    Publisher = result.Publisher
                });

            return listingResult;
        }

        public string Get(int id)
        {
            return "";
        }

        // POST: api/Asset
        public async Task<HttpResponseMessage> Post(HttpRequestMessage request)
        {
            if (!request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }

            var data = await Request.Content.ParseMultipartAsync();

            if (!(data.Files.Count > 0))
            {
                throw new HttpResponseException(HttpStatusCode.NoContent);
            }

            assets.UploadData(data.Files["file"].File, data.Files["file"].FileName);

            return new HttpResponseMessage(HttpStatusCode.OK);
        }

        // DELETE: api/Asset/5
        public IHttpActionResult Delete(int id)
        {
            if (!assets.DeleteAsset(id))
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            assets.Save();

            return Ok();
        }

        [Route("api/DownloadData")]
        [HttpPost]
        public HttpResponseMessage DownloadData([FromBody]DownloadFileModel Dfile)
        {

            var asset = assets.GetById(Dfile.Id);
            if (asset == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            FileDataBase fileData = new FileDataHandler();

            byte[] downFile = fileData.TryGetFile(asset, Dfile.Type);

            if (downFile == null)
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }

            var filename = Regex.Replace(asset.Title, @"[^a-zA-z0-9]+", String.Empty) + "." + Dfile.Type;
            return GetFile(downFile, filename);
        }

        [Route("api/DownloadSelected")]
        [HttpPost]
        public HttpResponseMessage DownloadSelected([FromBody]DownloadSelectedModel selectedItems)
        {

            if (selectedItems == null || selectedItems.Id.Length == 0)
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }

            if (selectedItems.Id.Length == 1)
            {
                return DownloadData(new DownloadFileModel { Id = selectedItems.Id[0], Type = selectedItems.Type });
            }

            var tmpAssets = assets.GetSelected(selectedItems.Id);

            FileDataBase fileData = new FileDataHandler();
            var downFile = new byte[0];

            downFile = fileData.TryGetListDataFile(tmpAssets, selectedItems.Type);

            if (downFile == null)
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }

            var filename = "ListData." + selectedItems.Type;

            return GetFile(downFile, filename);
        }

        private HttpResponseMessage GetFile(byte[] downFile, string filename)
        {
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Content = new StreamContent(new MemoryStream(downFile));
            response.Content.Headers.Add("Access-Control-Expose-Headers", "fileName");
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
            {
                FileName = filename
            };
            response.Content.Headers.ContentDisposition.FileName = filename;
            response.Content.Headers.ContentLength = downFile.Length;
            response.Headers.Add("fileName", filename);

            return response;
        }
    }
}
