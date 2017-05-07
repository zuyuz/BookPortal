using BookPortal.Domain;
using BookPortal.Domain.Abstract;
using BookPortal.Domain.Concrete;
using BookPortal.Domain.Entities;
using System;

namespace BookPortal.DbGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            IBookRepository repo = new EFBookRepository();
            Console.WriteLine(repo.Books.ToString());
        }
    }
}
