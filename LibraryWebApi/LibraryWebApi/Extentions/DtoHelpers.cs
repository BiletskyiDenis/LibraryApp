using Library.Data.Models;
using LibraryWebApi.Models;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace LibraryWebApi.Extentions
{
    public static class DtoHelpers
    {
        /// <summary>
        /// Data transfer object
        /// </summary>
        /// <typeparam name="T">Book, Journal, Brochure</typeparam>
        /// <param name="asset"></param>
        /// <returns></returns>
        public static T Dto<T>(this LibraryAsset asset) where T : class, new()
        {
            if (asset == null)
            {
                return null;
            }

            var dest = new T();
            var typeSource = asset.GetType();
            var typeDest = dest.GetType();
            var fieldsSource = typeSource.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var fieldsDest = typeDest.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var fieldDest in fieldsDest)
            {
                foreach (var fieldSource in fieldsSource)
                {
                    if (fieldDest.Name == fieldSource.Name)
                    {
                        fieldDest.SetValue(dest, fieldSource.GetValue(asset));
                        break;
                    }
                }
            }

            return dest;
        }

        public static IEnumerable<T> Dto<T>(this IEnumerable<LibraryAsset> asset) where T : class, new()
        {
            return asset.Select(s => s.Dto<T>());
        }

        public static IEnumerable<DtoRecentlyAdded> DtoRecentlyAdded(this IEnumerable<LibraryAsset> asset, int maxChars = 300)
        {
            return asset.Reverse().Take(3).Select(s => new DtoRecentlyAdded()
            {
                Id = s.Id,
                Title = s.Title,
                Description = SetMaxChars(s.Description, maxChars),
                ImageUrl = s.ImageUrl
            });
        }
        private static string SetMaxChars(this string rawString, int maxChars)
        {
            if (rawString==null||rawString.Length < maxChars)
                return rawString;
           
            return rawString.Substring(0, maxChars) + "...";
        }
    }

}
