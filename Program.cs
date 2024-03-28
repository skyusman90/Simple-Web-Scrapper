using CsvHelper;
using HtmlAgilityPack;
using System.Globalization;

namespace WebScrapper
{
    class Program
    {
        static void Main(string[] args)
        {
            var bookLinks = GetBooks("https://books.toscrape.com/catalogue/category/books/sequential-art_5/index.html");
            Console.WriteLine(format: "Found {0} Book links", bookLinks.Count);

            List<Book> books = GetBookDetails(bookLinks);
            foreach(Book book in books)
            {
                Console.WriteLine(book.Title);
            }
            ExportToCSV(books);

            Console.ReadLine();
        }
        static HtmlDocument GetDocument(string url)
        {
            HtmlWeb web = new HtmlWeb();
            HtmlDocument document = web.Load(url);
            return document;
        }
        static List<string> GetBooks(string url)
        {
            List<string> bookLinks = new List<string>();
            HtmlDocument document = GetDocument(url);
            HtmlNodeCollection htmlNodes = document.DocumentNode.SelectNodes(xpath: "//h3/a");

            Uri baseUri = new Uri(uriString: url);

            foreach(HtmlNode htmlNode in htmlNodes)
            {
                string href = htmlNode.Attributes[name: "href"].Value;
                bookLinks.Add(item: new Uri(baseUri, relativeUri: href).AbsoluteUri);
            }

            return bookLinks;
        }
        static List<Book> GetBookDetails(List<string> urls)
        {
            List<Book> books = new List<Book>();
            foreach (string url in urls)
            {
                HtmlDocument document = GetDocument(url);
                string titleXPath = "//h1";
                string priceXPath = "//*[@id=\"content_inner\"]/article/div[1]/div[2]/p[1]";
                Book book = new Book();
                book.Title = document.DocumentNode.SelectSingleNode(xpath: titleXPath).InnerText;
                book.Price = document.DocumentNode.SelectSingleNode(xpath: priceXPath).InnerText;
                books.Add(item: book);
            }

            return books;
        }

        static void ExportToCSV(List<Book> books)
        {
            using (StreamWriter writer = new StreamWriter(path: "E:\\Projects\\WebScrapper\\books.csv"))
            using (CsvWriter csvWriter = new CsvWriter(writer, culture: CultureInfo.InvariantCulture))
            {
                csvWriter.WriteRecords(records: books);
            }
        }
    }
}
