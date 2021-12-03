using System;
using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace VaporStore.ImportResults
{
    [XmlType(TypeName = "Purchase")]
    public class PurchaseDto
    {
        [Required]

        [XmlElement]
        public string Type { get; set; }

        [Required]
        [XmlElement]
        [RegularExpression(GlobalConstants.PurchaseKeyRegex)]
        public string Key { get; set; }

        [Required]
        [XmlElement]
        public string Date { get; set; }

        [Required]
        [XmlElement]
        public string Card { get; set; }

        [Required]
        [XmlAttribute(AttributeName = "title")]
        public string Title { get; set; }
    }
}
