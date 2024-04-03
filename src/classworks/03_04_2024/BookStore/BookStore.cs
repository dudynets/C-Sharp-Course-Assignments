namespace BookStore;

public class Book
{
    public int Id { get; }
    public string AuthorSurname { get; set; }
    public string Title { get; set; }
    public double Price { get; set; }

    public Book(int id, string authorSurname, string title, double price)
    {
        Id = id;
        AuthorSurname = authorSurname;
        Title = title;
        Price = price;
    }

    public override string ToString()
    {
        return $"Id: {Id}, Author surname: {AuthorSurname}, Title: {Title}, Price: {Price}";
    }
}

public class Buyer
{
    public int Id { get; }
    public string Surname { get; set; }
    public string Country { get; set; }

    public Buyer(int id, string surname, string country)
    {
        Id = id;
        Surname = surname;
        Country = country;
    }

    public override string ToString()
    {
        return $"Id: {Id}, Surname: {Surname}, Country: {Country}";
    }
}

public class Order
{
    public int BuyerId { get; }
    public int BookId { get; }
    public int Quantity { get; }

    public Order(int buyerId, int bookId, int quantity)
    {
        BuyerId = buyerId;
        BookId = bookId;
        Quantity = quantity;
    }
}

public class BookStore
{
    public List<Book> books = new List<Book>();
    public List<Buyer> buyers = new List<Buyer>();
    public List<Order> orders = new List<Order>();

    public void LoadData()
    {
        books.Add(new Book(1, "Tolkien", "The Lord of the Rings", 10.0));
        books.Add(new Book(2, "Rowling", "Harry Potter", 15.0));
        books.Add(new Book(3, "Martin", "A Game of Thrones", 20.0));

        buyers.Add(new Buyer(1, "Smith", "USA"));
        buyers.Add(new Buyer(2, "Johnson", "UK"));
        buyers.Add(new Buyer(3, "Brown", "Canada"));

        orders.Add(new Order(1, 1, 2));
        orders.Add(new Order(1, 1, 2));
        orders.Add(new Order(1, 3, 1));
        orders.Add(new Order(2, 2, 1));
        orders.Add(new Order(3, 3, 3));
    }

    public List<Dictionary<string, string>> GetBuyerOrders(string buyerSurname)
    {
        var buyer = buyers.Find(b => b.Surname == buyerSurname);
        if (buyer == null)
        {
            throw new Exception($"Buyer with surname {buyerSurname} not found");
        }

        var buyerOrders = orders.Where(o => o.BuyerId == buyer.Id).ToList();
        var result = new List<Dictionary<string, string>>();
        var uniqueBooks = new HashSet<int>();

        foreach (var order in buyerOrders)
        {
            if (uniqueBooks.Contains(order.BookId))
            {
                continue;
            }
            uniqueBooks.Add(order.BookId);

            var book = books.Find(b => b.Id == order.BookId);
            var totalPrice = $"{book?.Price * buyerOrders.Where(o => o.BookId == order.BookId).Sum(o => o.Quantity) ?? 0} UAH";
            var bookDict = new Dictionary<string, string> {
                {"authorSurname", book?.AuthorSurname ?? "Unknown"},
                {"bookTitle", book?.Title ?? "Unknown"},
                {"totalPrice", totalPrice}
            };
            result.Add(bookDict);
        }

        return result;
    }

    public List<KeyValuePair<string, string>> GetCountriesStats()
    {
        var result = new List<KeyValuePair<string, string>>();
        var countries = buyers.Select(b => b.Country).Distinct();

        foreach (var country in countries)
        {
            var countryBuyers = buyers.Where(b => b.Country == country).ToList();
            var countryOrders = orders.Where(o => countryBuyers.Any(b => b.Id == o.BuyerId)).ToList();
            var totalPrice = countryOrders.Sum(o =>
            {
                var book = books.Find(b => b.Id == o.BookId);
                return book?.Price * o.Quantity ?? 0;
            });
            result.Add(new KeyValuePair<string, string>(country, $"{totalPrice} UAH"));
        }

        return result;
    }

    public List<KeyValuePair<string, string>> GetAuthorStats(string authorSurname)
    {
        var result = new List<KeyValuePair<string, string>>();
        var authorBooks = books.Where(b => b.AuthorSurname == authorSurname).ToList();

        foreach (var book in authorBooks)
        {
            var bookOrders = orders.Where(o => o.BookId == book.Id).ToList();
            var totalPrice = bookOrders.Sum(o => o.Quantity * book.Price);
            result.Add(new KeyValuePair<string, string>(book.Title, $"{totalPrice} UAH"));
        }

        return result;
    }
}

public class Program
{
    public static void Main()
    {
        var bookStore = new BookStore();
        bookStore.LoadData();

        var buyerOrders = bookStore.GetBuyerOrders("Smith");
        Console.WriteLine("\nBuyer orders (Smith):");
        foreach (var order in buyerOrders)
        {
            Console.WriteLine($"- \"{order["bookTitle"]}\" by {order["authorSurname"]}, total price: {order["totalPrice"]}");
        }

        var countriesStats = bookStore.GetCountriesStats();
        Console.WriteLine("\nCountries stats:");
        foreach (var stat in countriesStats)
        {
            Console.WriteLine($"- {stat.Key}: {stat.Value}");
        }

        var authorStats = bookStore.GetAuthorStats("Tolkien");
        Console.WriteLine("\nAuthor stats (Tolkien):");
        foreach (var stat in authorStats)
        {
            Console.WriteLine($"- {stat.Key}: {stat.Value}");
        }
    }
}
