using Library.Data.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace LibraryWebApi.Extentions
{
    public static class HelpersExtentions
    {
        public static async Task<HttpPostedData> ParseMultipartAsync(this HttpContent postedContent)
        {
            var provider = await postedContent.ReadAsMultipartAsync();

            var fields = new Dictionary<string, HttpPostedField>(StringComparer.InvariantCultureIgnoreCase);
            var files = new Dictionary<string, HttpPostedFile>(StringComparer.InvariantCultureIgnoreCase);

            foreach (var content in provider.Contents)
            {
                var fieldName = content.Headers.ContentDisposition.Name.Trim('"');
                if (!string.IsNullOrEmpty(content.Headers.ContentDisposition.FileName))
                {
                    var file = await content.ReadAsByteArrayAsync();
                    var fileName = content.Headers.ContentDisposition.FileName.Trim('"');
                    files.Add(fieldName, new HttpPostedFile(fieldName, fileName, file));
                }
                else
                {
                    var data = await content.ReadAsStringAsync();
                    fields.Add(fieldName, new HttpPostedField(fieldName, data));
                }
            }

            return new HttpPostedData(fields, files);
        }

        public static T GetAsset<T>(this HttpPostedData data) where T : LibraryAsset
        {
            if (!data.Fields.ContainsKey("asset"))
                return null;

            var raw = data.Fields["asset"].Value;
            return JsonConvert.DeserializeObject<T>(raw);
        }
    }
}