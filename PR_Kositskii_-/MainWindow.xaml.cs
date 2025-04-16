using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Xml;
using Newtonsoft.Json;

namespace CashRegisterWPF
{
    public partial class MainWindow : Window
    {
        private readonly CashRegisterApp.AuthService authService;

        public MainWindow()
        {
            InitializeComponent();
            authService = new CashRegisterApp.AuthService();
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            // Ваша логика авторизации
        }
    }

}

namespace CashRegisterWPF
{
    public partial class RegisterWindow : Window
    {
        private readonly CashRegisterApp.AuthService _authService;

        public RegisterWindow(CashRegisterApp.AuthService authService)
        {
            InitializeComponent();
            _authService = authService;
        }

        private void InitializeComponent()
        {
            throw new NotImplementedException();
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            // Реализация регистрации
        }
    }
}
namespace CashRegisterApp
{
    // Класс для представления пользователя
    public class User
    {
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public string Salt { get; set; }
        public string Role { get; set; } // "admin" или "cashier"
    }

    // Класс для представления товара
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }

    // Класс для представления позиции в чеке
    public class ReceiptItem
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public decimal Total => Price * Quantity;
    }

    // Класс для представления чека
    public class Receipt
    {
        public int ReceiptNumber { get; set; }
        public DateTime DateTime { get; set; }
        public string CashierName { get; set; }
        public List<ReceiptItem> Items { get; set; } = new List<ReceiptItem>();
        public decimal TotalAmount { get; set; }
    }

    // Класс для работы с авторизацией
    public class AuthService
    {
        private List<User> users;
        private readonly string usersFile = "users.json";
        private User currentUser;

        public AuthService()
        {
            LoadUsers();
        }

        private void LoadUsers()
        {
            if (File.Exists(usersFile))
            {
                string json = File.ReadAllText(usersFile);
                users = JsonConvert.DeserializeObject<List<User>>(json);
            }
            else
            {
                // Создаем администратора по умолчанию
                users = new List<User>();
                Register("admin", "admin123", "admin");
                SaveUsers();
            }
        }

        private void SaveUsers()
        {
            string json = JsonConvert.SerializeObject(users, Newtonsoft.Json.Formatting.Indented);
            File.WriteAllText(usersFile, json);
        }

        // Генерация соли для пароля
        private string GenerateSalt()
        {
            var rng = new RNGCryptoServiceProvider();
            var saltBytes = new byte[16];
            rng.GetBytes(saltBytes);
            return Convert.ToBase64String(saltBytes);
        }

        // Хеширование пароля с солью
        private string HashPassword(string password, string salt)
        {
            using (var sha256 = SHA256.Create())
            {
                var saltedPassword = password + salt;
                var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(saltedPassword));
                return Convert.ToBase64String(bytes);
            }
        }

        // Регистрация нового пользователя
        public bool Register(string username, string password, string role)
        {
            if (users.Exists(u => u.Username == username))
            {
                Console.WriteLine("Пользователь с таким именем уже существует!");
                return false;
            }

            var salt = GenerateSalt();
            var passwordHash = HashPassword(password, salt);

            users.Add(new User
            {
                Username = username,
                PasswordHash = passwordHash,
                Salt = salt,
                Role = role
            });

            SaveUsers();
            Console.WriteLine("Пользователь успешно зарегистрирован!");
            return true;
        }

        // Вход в систему
        public bool Login(string username, string password)
        {
            var user = users.Find(u => u.Username == username);
            if (user == null)
            {
                Console.WriteLine("Пользователь не найден!");
                return false;
            }

            var inputHash = HashPassword(password, user.Salt);
            if (inputHash != user.PasswordHash)
            {
                Console.WriteLine("Неверный пароль!");
                return false;
            }

            currentUser = user;
            Console.WriteLine($"Добро пожаловать, {username}!");
            return true;
        }

        // Выход из системы
        public void Logout()
        {
            currentUser = null;
            Console.WriteLine("Вы вышли из системы.");
        }

        // Получение текущего пользователя
        public User GetCurrentUser()
        {
            return currentUser;
        }

        // Проверка роли текущего пользователя
        public bool IsAdmin()
        {
            return currentUser?.Role == "admin";
        }
    }

    // Основной класс кассового аппарата
    public class CashRegister
    {
        private List<Product> products;
        private List<Receipt> receipts;
        private int receiptCounter = 1;
        private readonly string productsFile = "products.json";
        private readonly string receiptsFile = "receipts.json";
        private readonly AuthService authService;

        public CashRegister(AuthService authService)
        {
            this.authService = authService;
            LoadProducts();
            LoadReceipts();
        }

        // Загрузка товаров из JSON файла
        private void LoadProducts()
        {
            if (File.Exists(productsFile))
            {
                string json = File.ReadAllText(productsFile);
                products = JsonConvert.DeserializeObject<List<Product>>(json);
            }
            else
            {
                products = new List<Product>
                {
                    new Product { Id = 1, Name = "Хлеб", Price = 50, Quantity = 100 },
                    new Product { Id = 2, Name = "Молоко", Price = 80, Quantity = 50 },
                    new Product { Id = 3, Name = "Яйца", Price = 120, Quantity = 30 }
                };
                SaveProducts();
            }
        }

        // Сохранение товаров в JSON файл
        private void SaveProducts()
        {
            string json = JsonConvert.SerializeObject(products, Newtonsoft.Json.Formatting.Indented);
            File.WriteAllText(productsFile, json);
        }

        // Загрузка чеков из JSON файла
        private void LoadReceipts()
        {
            if (File.Exists(receiptsFile))
            {
                string json = File.ReadAllText(receiptsFile);
                receipts = JsonConvert.DeserializeObject<List<Receipt>>(json);
                if (receipts.Count > 0)
                {
                    receiptCounter = receipts[receipts.Count - 1].ReceiptNumber + 1;
                }
            }
            else
            {
                receipts = new List<Receipt>();
            }
        }

        // Сохранение чеков в JSON файл
        private void SaveReceipts()
        {
            string json = JsonConvert.SerializeObject(receipts, Newtonsoft.Json.Formatting.Indented);
            File.WriteAllText(receiptsFile, json);
        }

        // Отображение списка товаров
        public void DisplayProducts()
        {
            Console.WriteLine("Доступные товары:");
            Console.WriteLine("ID | Название | Цена | Количество");
            foreach (var product in products)
            {
                Console.WriteLine($"{product.Id} | {product.Name} | {product.Price} руб. | {product.Quantity}");
            }
        }

        // Создание нового чека
        public Receipt CreateNewReceipt()
        {
            var currentUser = authService.GetCurrentUser();
            if (currentUser == null)
            {
                Console.WriteLine("Необходимо войти в систему!");
                return null;
            }

            var receipt = new Receipt
            {
                ReceiptNumber = receiptCounter++,
                DateTime = DateTime.Now,
                CashierName = currentUser.Username
            };
            return receipt;
        }

        // Добавление товара в чек
        public void AddProductToReceipt(Receipt receipt, int productId, int quantity)
        {
            var product = products.Find(p => p.Id == productId);
            if (product == null)
            {
                Console.WriteLine("Товар не найден!");
                return;
            }

            if (product.Quantity < quantity)
            {
                Console.WriteLine($"Недостаточно товара на складе! Доступно: {product.Quantity}");
                return;
            }

            // Уменьшаем количество товара на складе
            product.Quantity -= quantity;

            // Добавляем товар в чек
            var receiptItem = new ReceiptItem
            {
                ProductId = product.Id,
                ProductName = product.Name,
                Price = product.Price,
                Quantity = quantity
            };

            receipt.Items.Add(receiptItem);
            receipt.TotalAmount += receiptItem.Total;

            Console.WriteLine($"Добавлено: {product.Name} x{quantity} = {receiptItem.Total} руб.");
        }

        // Печать чека
        public void PrintReceipt(Receipt receipt)
        {
            Console.WriteLine("\n=== ЧЕК ===");
            Console.WriteLine($"Номер: {receipt.ReceiptNumber}");
            Console.WriteLine($"Дата: {receipt.DateTime}");
            Console.WriteLine($"Кассир: {receipt.CashierName}");
            Console.WriteLine("\nТовары:");
            foreach (var item in receipt.Items)
            {
                Console.WriteLine($"{item.ProductName} x{item.Quantity} - {item.Price} руб./шт. = {item.Total} руб.");
            }
            Console.WriteLine($"\nИТОГО: {receipt.TotalAmount} руб.");
            Console.WriteLine("============\n");
        }

        // Завершение работы с чеком (сохранение)
        public void FinalizeReceipt(Receipt receipt)
        {
            receipts.Add(receipt);
            SaveProducts();
            SaveReceipts();
            Console.WriteLine("Чек сохранен!");
        }

        // Отображение истории чеков
        public void DisplayReceiptsHistory()
        {
            var currentUser = authService.GetCurrentUser();
            if (currentUser == null) return;

            Console.WriteLine("\nИстория чеков:");
            foreach (var receipt in receipts)
            {
                // Админ видит все чеки, кассир - только свои
                if (authService.IsAdmin() || receipt.CashierName == currentUser.Username)
                {
                    Console.WriteLine($"Чек #{receipt.ReceiptNumber} от {receipt.DateTime} - Кассир: {receipt.CashierName} - Сумма: {receipt.TotalAmount} руб.");
                }
            }
        }

        // Добавление нового товара (только для админа)
        public void AddNewProduct(string name, decimal price, int quantity)
        {
            if (!authService.IsAdmin())
            {
                Console.WriteLine("Недостаточно прав для выполнения этой операции!");
                return;
            }

            int newId = products.Count > 0 ? products[products.Count - 1].Id + 1 : 1;
            products.Add(new Product
            {
                Id = newId,
                Name = name,
                Price = price,
                Quantity = quantity
            });

            SaveProducts();
            Console.WriteLine("Товар успешно добавлен!");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var authService = new AuthService();
            var cashRegister = new CashRegister(authService);
            Receipt currentReceipt = null;

            while (true)
            {
                var currentUser = authService.GetCurrentUser();
                if (currentUser == null)
                {
                    // Меню авторизации
                    Console.WriteLine("\nМеню авторизации:");
                    Console.WriteLine("1. Вход");
                    Console.WriteLine("2. Регистрация (только для администратора)");
                    Console.WriteLine("0. Выход");

                    Console.Write("Выберите действие: ");
                    var authChoice = Console.ReadLine();

                    switch (authChoice)
                    {
                        case "1":
                            Console.Write("Введите имя пользователя: ");
                            var username = Console.ReadLine();
                            Console.Write("Введите пароль: ");
                            var password = Console.ReadLine();
                            authService.Login(username, password);
                            break;
                        case "2":
                            Console.Write("Введите имя пользователя: ");
                            var newUsername = Console.ReadLine();
                            Console.Write("Введите пароль: ");
                            var newPassword = Console.ReadLine();
                            Console.Write("Введите роль (admin/cashier): ");
                            var role = Console.ReadLine();
                            authService.Register(newUsername, newPassword, role);
                            break;
                        case "0":
                            return;
                        default:
                            Console.WriteLine("Некорректный выбор!");
                            break;
                    }
                }
                else
                {
                    // Основное меню
                    Console.WriteLine($"\nМеню кассового аппарата (Пользователь: {currentUser.Username}, Роль: {currentUser.Role})");
                    Console.WriteLine("1. Показать товары");
                    Console.WriteLine("2. Новый чек");
                    Console.WriteLine("3. Добавить товар в чек");
                    Console.WriteLine("4. Печать чека");
                    Console.WriteLine("5. Завершить чек");
                    Console.WriteLine("6. История чеков");

                    if (authService.IsAdmin())
                    {
                        Console.WriteLine("7. Добавить новый товар");
                    }

                    Console.WriteLine("8. Выйти из системы");
                    Console.WriteLine("0. Выход");

                    Console.Write("Выберите действие: ");
                    var choice = Console.ReadLine();

                    switch (choice)
                    {
                        case "1":
                            cashRegister.DisplayProducts();
                            break;
                        case "2":
                            currentReceipt = cashRegister.CreateNewReceipt();
                            if (currentReceipt != null)
                            {
                                Console.WriteLine("Создан новый чек");
                            }
                            break;
                        case "3":
                            if (currentReceipt == null)
                            {
                                Console.WriteLine("Сначала создайте новый чек!");
                                break;
                            }
                            Console.Write("Введите ID товара: ");
                            if (!int.TryParse(Console.ReadLine(), out int productId))
                            {
                                Console.WriteLine("Некорректный ID!");
                                break;
                            }
                            Console.Write("Введите количество: ");
                            if (!int.TryParse(Console.ReadLine(), out int quantity) || quantity <= 0)
                            {
                                Console.WriteLine("Некорректное количество!");
                                break;
                            }
                            cashRegister.AddProductToReceipt(currentReceipt, productId, quantity);
                            break;
                        case "4":
                            if (currentReceipt == null || currentReceipt.Items.Count == 0)
                            {
                                Console.WriteLine("Чек пуст или не создан!");
                                break;
                            }
                            cashRegister.PrintReceipt(currentReceipt);
                            break;
                        case "5":
                            if (currentReceipt == null || currentReceipt.Items.Count == 0)
                            {
                                Console.WriteLine("Чек пуст или не создан!");
                                break;
                            }
                            cashRegister.PrintReceipt(currentReceipt);
                            cashRegister.FinalizeReceipt(currentReceipt);
                            currentReceipt = null;
                            break;
                        case "6":
                            cashRegister.DisplayReceiptsHistory();
                            break;
                        case "7":
                            if (authService.IsAdmin())
                            {
                                Console.Write("Введите название товара: ");
                                var name = Console.ReadLine();
                                Console.Write("Введите цену товара: ");
                                if (!decimal.TryParse(Console.ReadLine(), out decimal price))
                                {
                                    Console.WriteLine("Некорректная цена!");
                                    break;
                                }
                                Console.Write("Введите количество товара: ");
                                if (!int.TryParse(Console.ReadLine(), out int newQuantity))
                                {
                                    Console.WriteLine("Некорректное количество!");
                                    break;
                                }
                                cashRegister.AddNewProduct(name, price, newQuantity);
                            }
                            else
                            {
                                Console.WriteLine("Некорректный выбор!");
                            }
                            break;
                        case "8":
                            authService.Logout();
                            currentReceipt = null;
                            break;
                        case "0":
                            return;
                        default:
                            Console.WriteLine("Некорректный выбор!");
                            break;
                    }
                }
            }
        }
    }
}
