﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using Library.Data.Models;
using Library.Data;
using System.Web;
using System.IO;
using System.Web.Hosting;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Library.Service
{
    public class LibraryService : ILibraryService
    {
        private readonly LibraryContext context;
        private readonly FileDataBase fileHandler;
        private readonly string imageStorePath = @"~/img/assets/";
        private readonly string imageSmallStorePath = @"~/img/assets_small/";

        public LibraryService(LibraryContext context)
        {
            this.context = context;
            this.fileHandler = new FileDataHandler();
        }

        public IEnumerable<LibraryAsset> GetAll()
        {
            return context.LibraryAssets.ToList();
        }

        public IEnumerable<LibraryAsset> GetSelected(int[] selected)
        {
            return GetAll().Where(s => selected.Contains(s.Id)).ToList();
        }

        public LibraryAsset GetById(int? id)
        {
            return GetAll().FirstOrDefault(asset => asset.Id == id);
        }

        public IEnumerable<Book> GetBooks()
        {
            return context.Books.ToList();
        }

        public IEnumerable<Journal> GetJournals()
        {
            return context.Journals.ToList();
        }

        public IEnumerable<Brochure> GetBrochures()
        {
            return context.Brochures.ToList();
        }

        public string GetISBN(int id)
        {
            var isBook = context.LibraryAssets.OfType<Book>()
                .Where(d => d.Id == id).Any();

            if (isBook)
            {
                return context.Books.FirstOrDefault(b => b.Id == id).ISBN;
            }

            return string.Empty;
        }

        public int GetPages(int id)
        {
            var isBook = context.LibraryAssets.OfType<Book>()
                .Where(d => d.Id == id).Any();

            if (isBook)
            {
                return context.Books.FirstOrDefault(b => b.Id == id).Pages;
            }

            return 0;
        }

        public string GetAuthor(int id)
        {
            var isBook = context.LibraryAssets.OfType<Book>()
                .Where(d => d.Id == id).Any();

            if (isBook)
            {
                return context.Books.FirstOrDefault(b => b.Id == id).Author;
            }

            return string.Empty;
        }

        public AssetType GetType(int? id)
        {
            var isBook = context.LibraryAssets.OfType<Book>()
                        .Where(d => d.Id == id);

            var isJournal = context.LibraryAssets.OfType<Journal>()
            .Where(d => d.Id == id);

            var isBrochure = context.LibraryAssets.OfType<Brochure>()
            .Where(d => d.Id == id);

            if (isBook.Any())
                return AssetType.Book;

            if (isJournal.Any())
                return AssetType.Journal;

            return AssetType.Brochure;
        }

        public IEnumerable<string> GetAllTypes()
        {
            return Enum.GetNames(typeof(AssetType));
        }

        public string GetFrequency(int id)
        {
            if (context.Journals.Any(j => j.Id == id))
            {
                return context.Journals.FirstOrDefault(j => j.Id == id).Frequency;
            }
            return string.Empty;
        }

        public bool DeleteAsset(int id)
        {
            var asset = context.LibraryAssets.Find(id);
            if (asset == null)
            {
                return false;
            }

            DeleteImage(asset);
            context.LibraryAssets.Remove(asset);

            return true;
        }

        public void AddAsset(LibraryAsset asset, byte[] file)
        {
            if (file != null)
            {
                try
                {
                    asset.ImageUrl = UploadImage(asset, file);
                }
                catch (Exception)
                {
                    asset.ImageUrl = "none";
                }
            }

            context.LibraryAssets.Add(asset);
        }

        public void Save()
        {
            context.SaveChanges();
        }

        public void UpdateAsset(LibraryAsset asset, byte[] file)
        {
            if (file != null)
            {
                DeleteImage(asset);
                asset.ImageUrl = UploadImage(asset, file);
            }
            context.Entry(asset).State = EntityState.Modified;
        }

        public string UploadImage(LibraryAsset asset, byte[] file)
        {
            var fileName = GetImageFileName(asset);
            var fileExt = ".jpg";
            if (file != null)
            {
                using (var stream = new MemoryStream(file))
                {
                    var pathBig = HostingEnvironment.MapPath(imageStorePath + fileName + fileExt);
                    var pathSmall = HostingEnvironment.MapPath(imageSmallStorePath + fileName + fileExt);
                    new ImageHandler(stream, 250).ImgSource.Save(pathBig);
                    new ImageHandler(stream, 50).ImgSource.Save(pathSmall);
                }
            }
            return fileName + fileExt;

        }

        public bool UploadData(byte[] file, string filename)
        {
            var fileExt = Path.GetExtension(filename);

            if (file == null)
            {
                return false;
            }
            try
            {
                using (var stream = new MemoryStream(file))
                {
                    if (fileExt == ".xml")
                    {
                        RestoreAssetFromXml(stream);
                    }
                    else
                    {
                        RestoreAssetFromTxt(stream);
                    }

                }
            }
            catch (Exception e)
            {
                var rr = e;
                return false;
            }

            return true;
        }

        private void RestoreAssetFromXml(Stream stream)
        {
            var doc = XDocument.Load(stream);
            if (doc.Elements().FirstOrDefault().Name == null)
                return;

            if (doc.Elements().FirstOrDefault().Name == "List")
            {
                var assets = fileHandler.RestoreAssetsListFromXml(doc);
                if (assets == null)
                    return;

                foreach (var item in assets)
                {
                    if (item != null)
                    {
                        AddAsset(item, null);
                    }
                }

                Save();
            }
            else
            {
                var asset = fileHandler.RestoreAssetFromXml(doc);
                if (asset != null)
                {
                    AddAsset(asset, null);
                    Save();
                }
            }
        }

        private void RestoreAssetFromTxt(Stream stream)
        {
            TextReader tr = new StreamReader(stream);
            var data = tr.ReadToEnd();
            if (data.Substring(1, 4) == "List")
            {
                var assets = fileHandler.RestoreAssetsListFromTxt(data);
                if (assets == null)
                    return;
                foreach (var item in assets)
                {
                    if (item != null)
                    {
                        AddAsset(item, null);
                    }
                }

                Save();
            }
            else
            {
                var asset = fileHandler.RestoreAssetFromTxt(data);
                if (asset != null)
                {
                    AddAsset(asset, null);
                    Save();
                }
            }
        }

        public void DeleteImage(LibraryAsset asset)
        {
            var pathBig = HostingEnvironment.MapPath(imageStorePath + asset.ImageUrl);
            var pathSmall = HostingEnvironment.MapPath(imageSmallStorePath + asset.ImageUrl);
            if (asset.ImageUrl == "none")
                return;

            if (File.Exists(pathBig))
                File.Delete(pathBig);

            if (File.Exists(pathSmall))
                File.Delete(pathSmall);

            asset.ImageUrl = "none";
        }

        public string GetImageFileName(LibraryAsset asset)
        {
            var dest = string.Empty;
            var rn = new Random();

            var raw = new string[]
            {
                asset.Title,
                asset.Publisher,
                asset.Year.ToString(),
            };
            foreach (var item in raw)
            {
                var tmpItem = Regex.Replace(item, @"[^a-zA-z0-9]+", String.Empty);
                if (tmpItem.Length > 40)
                {
                    tmpItem = tmpItem.Substring(0, 40);
                }
                dest += tmpItem;
            }
            return dest + rn.Next(1000, 9999).ToString();
        }

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    context.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
