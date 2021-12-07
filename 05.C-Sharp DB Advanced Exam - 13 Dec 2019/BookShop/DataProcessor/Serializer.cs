namespace BookShop.DataProcessor
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml;
    using System.Xml.Serialization;
    using BookShop.Data.Models.Enums;
    using BookShop.DataProcessor.ExportDto;
    using Data;
    using Newtonsoft.Json;
    using Formatting = Newtonsoft.Json.Formatting;

    public class Serializer
    {
        public static string ExportMostCraziestAuthors(BookShopContext context)
        {
            var authors = context.Authors
                                        .Select(x => new
                                        {
                                            AuthorName = x.FirstName + " " + x.LastName,
                                            Books = x.AuthorsBooks
                                                                .OrderByDescending(b => b.Book.Price)
                                                                .Select(b => new
                                                                {
                                                                    BookName = b.Book.Name,
                                                                    BookPrice = b.Book.Price.ToString("F2")
                                                                })
                                                                .ToList()
                                        })
                                        .ToList()
                                        .OrderByDescending(a => a.Books.Count)
                                        .ThenBy(a => a.AuthorName)
                                        .ToList();

            return JsonConvert.SerializeObject(authors, Formatting.Indented);
        }

        public static string ExportOldestBooks(BookShopContext context, DateTime date)
        {
            StringBuilder sb = new StringBuilder();

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ExportOldestBookDto[]), new XmlRootAttribute("Books"));
            XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
            namespaces.Add(string.Empty, string.Empty);

            var books = context.Books
                                    .Where(x => x.PublishedOn < date && x.Genre == Genre.Science)
                                    .ToArray()
                                    .OrderByDescending(x => x.Pages)
                                    .ThenByDescending(x => x.PublishedOn)
                                    .Select(x => new ExportOldestBookDto
                                    {
                                        Name = x.Name,
                                        Date = x.PublishedOn.ToString("d", CultureInfo.InvariantCulture),
                                        Pages = x.Pages
                                    })
                                    .Take(10)
                                    .ToArray();

            xmlSerializer.Serialize(new StringWriter(sb), books, namespaces);

            return sb.ToString().TrimEnd();
        }
    }
}