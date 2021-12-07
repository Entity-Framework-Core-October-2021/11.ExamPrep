using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace BookShop.DataProcessor.ExportDto
{
    [XmlType("Book")]
    public class ExportOldestBookDto
    {
        [XmlElement("Name")]
        public string Name { get; set; }

        [XmlElement("Date")]
        public string Date { get; set; }


        [XmlAttribute("Pages")]
        public int Pages { get; set; }
    }
}

//Id - integer, Primary Key
//Name - text with length [3, 30]. (required)
//Genre - enumeration of type Genre, with possible values (Biography = 1, Business = 2, Science = 3) (required)
//Price - decimal in range between 0.01 and max value of the decimal
//Pages – integer in range between 50 and 5000
//PublishedOn - date and time (required)
//AuthorsBooks - collection of type AuthorBook