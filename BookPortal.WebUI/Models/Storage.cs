using BookPortal.Domain.Abstract;
using BookPortal.Domain.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace BookPortal.WebUI.Models
{
    public class Storage
    {
        public static Book SaveUploadedFile(string path,
                                      HttpPostedFileBase sourceFile)
        {
            string name = Path.GetFileName(sourceFile.FileName);
            sourceFile.SaveAs(path + @"\" + name);

            Book newBook = new Book(path + @"\" + name);
            newBook.Title = name;
            return newBook;
        }
    }
}