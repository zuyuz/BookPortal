using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace BookPortal.Domain.Entities
{
    public class Book
    {
        public Book() { }

        public Book(string path)
        {
            Id = 0;
            Title = "NewTitle";
            Author = "NewAuthor";
            Country = "Ukraine";
            Language = "UKR";
            Genre = "NewGenre";
            Publisher = "NewPublisher";
            Published = DateTime.Now;
            PathToFile = path;
        }

        [HiddenInput(DisplayValue = false)]
        public int Id { get; set; }
        [Required(ErrorMessage = "Please enter a title")]
        public string Title { get; set; }
        [Required(ErrorMessage = "Please enter Author")]
        public string Author { get; set; }
        [Required(ErrorMessage = "Please enter Country")]
        public string Country { get; set; }
        [Required(ErrorMessage = "Please enter language")]
        public string Language { get; set; }
        [Required(ErrorMessage = "Please enter genre")]
        public string Genre { get; set; }
        public string Publisher { get; set; }
        public DateTime Published { get; set; }
        public string PathToFile { get; set; }
    }
}
