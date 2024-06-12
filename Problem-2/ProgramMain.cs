using CsvHelper;
using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace U16A2Library
{
    class Program
    {
        static void Main(string[] args)
        {
            // File paths for both csv files
            string filePath = "U16A2Task2Data.csv";
            string newFilePath = "UpdatedBooksOutput.csv";

            using (var reader = new StreamReader(filePath))
            using (var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)))
            {
                csv.Context.RegisterClassMap<BookMap>();

                var records = csv.GetRecords<Book>().ToList();

                int RecordsCount = 0;
                // ID assigner for the books 
                ICodeAssigner IDAssigner = new MD5CodeAssigner();

                using (var writer = new StreamWriter(newFilePath))
                using (var csvw = new CsvWriter(writer, CultureInfo.InvariantCulture))
                {
                    // Book details that will be displayed
                    foreach (var record in records)
                    {
                        record.ID = IDAssigner.UpdateBook(record);

                        csvw.WriteField(record.ID);
                        csvw.WriteField(record.Name);
                        csvw.WriteField(record.Title);
                        csvw.WriteField(record.Location);
                        csvw.WriteField(record.Publisher);
                        csvw.WriteField(record.Date);
                        
                        // Records continously being added
                        csvw.NextRecord();
                        RecordsCount++;
                    }
                }
                // The output of how many lines have been read and records created
                Console.WriteLine($"{records.Count} lines have been read, {RecordsCount} records have been created.");
            }

            Console.ReadLine();
        }

         public class Book
        {
            public string Name { get; set; }
            public string Title { get; set; }
            public string Location { get; set; }
            public string Publisher { get; set; }
            public string Date { get; set; }
            public string ID { get; set; }  // Unique code being assigned to every book
        }

        // ID assigner interface
        public interface ICodeAssigner
        {
            string UpdateBook(Book book);
        }

        public class MD5CodeAssigner : ICodeAssigner
        {
            public string UpdateBook(Book book)
            {
                string bookString = $"{book.Name}-{book.Title}-{book.Location}-{book.Publisher}-{book.Date}";

                using (MD5 md5 = MD5.Create())
                {
                    byte[] inputBytes = Encoding.ASCII.GetBytes(bookString);
                    byte[] hashBytes = md5.ComputeHash(inputBytes);

                    StringBuilder sb = new StringBuilder();
                    for (int i = 0; i < hashBytes.Length; i++)
                    {
                        sb.Append(hashBytes[i].ToString("X2"));
                    }

                    string shortHash = "AX" + sb.ToString().Substring(sb.Length - 6);

                    return shortHash;
                }
            }
        }
        // Mapping for each csv field 
        public sealed class BookMap : ClassMap<Book>
        {
            public BookMap()
            {
                Map(m => m.Name).Index(0);
                Map(m => m.Title).Index(1);
                Map(m => m.Location).Index(2);
                Map(m => m.Publisher).Index(3);
                Map(m => m.Date).Index(4);
            }
        }
    }
}