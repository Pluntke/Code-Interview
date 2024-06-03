
using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;
using Test;
using Test.Classes;

const string itemsFilePath = "C:/Users/plunt/Downloads/aufgabe/items.csv";
const string ordersFilePath = "C:/Users/plunt/Downloads/aufgabe/orders.csv";

var items = ReadCsv<Items>(itemsFilePath);
var orders = ReadCsv<Orders>(ordersFilePath);

Console.Write("Bitte geben Sie die Auftragsnummer ein: ");
string orderId = Console.ReadLine();
if (!int.TryParse(orderId, out _))
{
    Console.WriteLine("Ungültige Auftragsnummer. Bitte geben Sie eine gültige Zahl ein.");
    return;
}
GenerateInvoice(orderId, items, orders);
    
    static List<T> ReadCsv<T>(string filePath)
{
    using (var reader = new StreamReader(filePath))
    using (var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture) { Delimiter = ";" }))
    {
        return new List<T>(csv.GetRecords<T>());
    }
}



static void GenerateInvoice(string orderId, List<Items> items, List<Orders> orders)
{


    int result = Int32.Parse(orderId);
    var orderLines = orders.FindAll(order => order.OrderID == result);

    if (orderLines.Count == 0)
    {
        Console.WriteLine($"Keine Bestellungen gefunden für Auftragsnummer {orderId}");
        return;
    }

    var invoiceLines = new List<Invoice>();

    foreach (var orderLine in orderLines)
    {
        var item = items.Find(i => i.Id == orderLine.ItemID);
        if (item != null)
        {
            var total_price = item.Price * orderLine.Quantity;
            invoiceLines.Add(new Invoice
            {
                SKU = item.SKU,
                Description = item.Description,
                Price = item.Price.ToString(),
                Quantity = orderLine.Quantity,
                TotalPrice = total_price.ToString()
            });
        }
        else
        {
            Console.WriteLine($"Artikel mit ID {orderLine.ItemID} nicht in items.csv gefunden");
        }
    }

    if (invoiceLines.Count > 0)
    {

        DisplayInvoice(invoiceLines);

        var invoiceFilePath = $"C:/Users/plunt/Downloads/aufgabe/{orderId}.csv";
        WriteCsv(invoiceFilePath, invoiceLines);
        Console.WriteLine($"Rechnung {invoiceFilePath} erfolgreich erstellt.");
    }
}

static void DisplayInvoice(List<Invoice> invoiceLines)
{
    Console.WriteLine("\nRechnung:");
    Console.WriteLine("{0,-10} {1,-25} {2,12} {3,10} {4,12}", "SKU", "Artikelbeschreibung", "Einzelpreis", "Menge", "Gesamtpreis");
    foreach (var line in invoiceLines)
    {
        Console.WriteLine("{0,-10} {1,-25} {2,12} {3,10} {4,12}", line.SKU, line.Description, line.Price, line.Quantity, line.TotalPrice);
    }
}
static void WriteCsv<T>(string filePath, List<T> records)
{
    using (var writer = new StreamWriter(filePath))
    using (var csv = new CsvWriter(writer, new CsvConfiguration(CultureInfo.InvariantCulture) { Delimiter = ";" }))
    {
        csv.WriteRecords(records);
    }
}
