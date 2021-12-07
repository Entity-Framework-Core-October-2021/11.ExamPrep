namespace BookShop.DataProcessor
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;
    using BookShop.Data.Models;
    using BookShop.Data.Models.Enums;
    using BookShop.DataProcessor.ImportDto;
    using Data;
    using Newtonsoft.Json;
    using ValidationContext = System.ComponentModel.DataAnnotations.ValidationContext;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid data!";

        private const string SuccessfullyImportedBook
            = "Successfully imported book {0} for {1:F2}.";

        private const string SuccessfullyImportedAuthor
            = "Successfully imported author - {0} with {1} books.";

        public static string ImportBooks(BookShopContext context, string xmlString)
        {
            StringBuilder sb = new StringBuilder();

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ImportBookDto[]), new XmlRootAttribute("Books"));

            ImportBookDto[] bookDtos = (ImportBookDto[])xmlSerializer.Deserialize(new StringReader(xmlString));

            List<Book> books = new List<Book>();

            foreach (var bookDto in bookDtos)
            {
                if (!IsValid(bookDto))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                var isValidDateTime = DateTime.TryParseExact(bookDto.PublishedOn, "MM/dd/yyyy", 
                    CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime publishedOn);

                if (!isValidDateTime)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                Book b = new Book()
                {
                    Name = bookDto.Name,
                    Genre = (Genre)bookDto.Genre,
                    Price = bookDto.Price,
                    Pages = bookDto.Pages,
                    PublishedOn = publishedOn
                };

                books.Add(b);
                sb.AppendLine(String.Format(SuccessfullyImportedBook,bookDto.Name, bookDto.Price));
            }

            context.Books.AddRange(books);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        public static string ImportAuthors(BookShopContext context, string jsonString)
        {
            StringBuilder sb = new StringBuilder();

            ImportAuthorDto[] authorDtos = JsonConvert.DeserializeObject<ImportAuthorDto[]>(jsonString);

            List<Author> authors = new List<Author>();
            List<AuthorBook> authorBooks = new List<AuthorBook>();

            int[] books = context.Books.Select(x => x.Id).ToArray();

            foreach (var authorDto in authorDtos)
            {
                if (!IsValid(authorDto))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                if (authors.Any(x=>x.Email == authorDto.Email))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                Author author = new Author()
                {
                    FirstName = authorDto.FirstName,
                    LastName = authorDto.LastName,
                    Phone = authorDto.Phone,
                    Email = authorDto.Email
                };

                int booksCount = 0;

                foreach (var bookId in authorDto.Books)
                {
                    if (bookId.Id == null || !books.Contains((int)bookId.Id))
                    {
                        continue;
                    }

                    AuthorBook authorBook = new AuthorBook()
                    {
                        Author = author,
                        BookId = (int)bookId.Id
                    };

                    authorBooks.Add(authorBook);
                    booksCount++;
                }

                if (booksCount != 0)
                {
                    authors.Add(author);
                    sb.AppendLine(string.Format(SuccessfullyImportedAuthor, $"{author.FirstName} {author.LastName}", booksCount));
                }
                else
                {
                    sb.AppendLine(ErrorMessage);
                }
            }

            context.Authors.AddRange(authors);
            context.AuthorsBooks.AddRange(authorBooks);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        private static bool IsValid(object dto)
        {
            var validationContext = new ValidationContext(dto);
            var validationResult = new List<ValidationResult>();

            return Validator.TryValidateObject(dto, validationContext, validationResult, true);
        }
    }
}