namespace BookStore.UnitTests
{
    public class BookStoreFixture : IDisposable
    {
        public BookStore bookStore { get; private set; }

        public BookStoreFixture()
        {
            bookStore = new BookStore();
            bookStore.books.Add(new Book(1, "Tolkien", "The Lord of the Rings", 10.0));
            bookStore.books.Add(new Book(2, "Rowling", "Harry Potter", 15.0));
            bookStore.books.Add(new Book(3, "Martin", "A Game of Thrones", 20.0));

            bookStore.buyers.Add(new Buyer(1, "Smith", "USA"));
            bookStore.buyers.Add(new Buyer(2, "Johnson", "UK"));
            bookStore.buyers.Add(new Buyer(3, "Brown", "Canada"));

            bookStore.orders.Add(new Order(1, 1, 2));
            bookStore.orders.Add(new Order(1, 1, 2));
            bookStore.orders.Add(new Order(1, 3, 1));
            bookStore.orders.Add(new Order(2, 2, 1));
            bookStore.orders.Add(new Order(3, 3, 3));
        }

        public void Dispose()
        {
            // Clean up the test data
        }
    }

    public class BookStoreTests : IClassFixture<BookStoreFixture>
    {
        BookStoreFixture fixture;

        public BookStoreTests(BookStoreFixture _fixture)
        {
            fixture = _fixture;
        }

        [Fact]
        public void TestGetBuyerOrders()
        {
            // Arrange
            var bookStore = fixture.bookStore;

            // Act
            var buyerOrders = bookStore.GetBuyerOrders("Smith");

            // Assert
            Assert.NotNull(buyerOrders);
            Assert.Equal(2, buyerOrders.Count);
            Assert.Contains(buyerOrders, order => order["authorSurname"] == "Tolkien");
            Assert.Contains(buyerOrders, order => order["authorSurname"] == "Martin");
        }

        [Fact]
        public void TestGetCountriesStats()
        {
            // Arrange
            var bookStore = fixture.bookStore;

            // Act
            var countriesStats = bookStore.GetCountriesStats();

            // Assert
            Assert.NotNull(countriesStats);
            Assert.Equal(3, countriesStats.Count);
            Assert.Contains(countriesStats, stat => stat.Key == "USA" && stat.Value == "60 UAH");
            Assert.Contains(countriesStats, stat => stat.Key == "UK" && stat.Value == "15 UAH");
            Assert.Contains(countriesStats, stat => stat.Key == "Canada" && stat.Value == "60 UAH");
        }

        [Fact]
        public void TestGetAuthorStats()
        {
            // Arrange
            var bookStore = fixture.bookStore;

            // Act
            var authorStats = bookStore.GetAuthorStats("Tolkien");

            // Assert
            Assert.NotNull(authorStats);
            Assert.Single(authorStats);
            Assert.Contains(authorStats, stat => stat.Key == "The Lord of the Rings" && stat.Value == "40 UAH");
        }
    }
}
