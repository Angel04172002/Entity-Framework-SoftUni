using Microsoft.EntityFrameworkCore;
using ProductShop.Data;
using ProductShop.DTOs.Export;
using ProductShop.DTOs.Import;
using ProductShop.Models;


namespace ProductShop
{
    public class StartUp
    {
        public static void Main()
        {
            var context = new ProductShopContext();

            string xml = File.ReadAllText("../../../Datasets/categories-products.xml");

            string result = GetCategoriesByProductsCount(context);
            Console.WriteLine(result);

        }


        public static string ImportUsers(ProductShopContext context, string inputXml)
        {
            XmlHelper xmlHelper = new XmlHelper();
            ImportUserDto[] userDtos = xmlHelper.Deserialize<ImportUserDto[]>(inputXml, "Users");

            ICollection<User> validUsers = new HashSet<User>();

            foreach (var userDto in userDtos)
            {
                if(userDto.FirstName == null || userDto.LastName == null || userDto.Age == null)
                {
                    continue;
                }

                User user = new User()
                {
                    FirstName = userDto.FirstName,
                    LastName = userDto.LastName,
                    Age = userDto.Age,
                };

                validUsers.Add(user);
            }

            context.Users.AddRange(validUsers);
            context.SaveChanges();

            return $"Successfully imported {validUsers.Count}";

        }


        public static string ImportProducts(ProductShopContext context, string inputXml)
        {
            XmlHelper xmlHelper = new XmlHelper();

            ICollection<Product> validProducts = new HashSet<Product>();

            ImportProductDto[] productDtos = xmlHelper.Deserialize<ImportProductDto[]>(inputXml, "Products");

            foreach (var productDto in productDtos)
            {
                if(productDto.Name == null || productDto.BuyerId == null)
                {
                    continue;
                }


                Product product = new Product()
                {
                    Name = productDto.Name,
                    Price = productDto.Price,
                    BuyerId = productDto.BuyerId,
                    SellerId = productDto.SellerId
                };

                validProducts.Add(product);
            }

            context.Products.AddRange(validProducts);
            context.SaveChanges();

            return $"Successfully imported {validProducts.Count}"; 
        }


        public static string ImportCategories(ProductShopContext context, string inputXml)
        {
            XmlHelper xmlHelper = new XmlHelper();

            ImportCategoryDto[] categoryDtos = xmlHelper.Deserialize<ImportCategoryDto[]>(inputXml, "Categories");

            ICollection<Category> validCategories = new HashSet<Category>();

            foreach (var categoryDto in categoryDtos)
            {
                if(categoryDto.Name == null)
                {
                    continue;
                }

                Category category = new Category()
                {
                    Name = categoryDto.Name,
                };


                validCategories.Add(category);
            }

            context.Categories.AddRange(validCategories);
            context.SaveChanges();

            return $"Successfully imported {validCategories.Count}";
        }

        public static string ImportCategoryProducts(ProductShopContext context, string inputXml)
        {
            XmlHelper xmlHelper = new XmlHelper();

            ImportCategoryProductDto[] categoryProductDtos =
                xmlHelper.Deserialize<ImportCategoryProductDto[]>(inputXml, "CategoryProducts");

            ICollection<CategoryProduct> validCategoryProducts = new HashSet<CategoryProduct>();

            ICollection<int> validCategoryIds = context.Categories
                .AsNoTracking()
                .Select(c => c.Id)
                .ToArray();

            ICollection<int> validProductIds = context.Products
                .AsNoTracking()
                .Select(p => p.Id)
                .ToArray();

            foreach (var categoryProductDto in categoryProductDtos)
            {
                if(categoryProductDto.ProductId == null || categoryProductDto.CategoryId == null)
                {
                    continue;
                }

                if (validCategoryIds.Contains(categoryProductDto.CategoryId.Value) == false ||
                    validProductIds.Contains(categoryProductDto.ProductId.Value) == false)
                {
                    continue;
                }


                CategoryProduct categoryProduct = new CategoryProduct()
                {
                    CategoryId = categoryProductDto.CategoryId.Value,
                    ProductId = categoryProductDto.ProductId.Value,
                };

                validCategoryProducts.Add(categoryProduct);
                
            }

            context.CategoryProducts.AddRange(validCategoryProducts);
            context.SaveChanges();

            return $"Successfully imported {validCategoryProducts.Count}";
        }

        public static string GetProductsInRange(ProductShopContext context)
        {
            XmlHelper xmlHelper = new XmlHelper();

            var products = context.Products
                .AsNoTracking()
                .Where(p => p.Price >= 500 && p.Price <= 1000)
                .OrderBy(p => p.Price)
                .Take(10)
                .Select(p => new ExportProductDto()
                {
                    Name = p.Name,
                    Price = p.Price,
                    BuyerFullName = p.Buyer.FirstName + " " + p.Buyer.LastName
                })
                .ToList();

            return xmlHelper.Serialize(products, "Products");

        }


        public static string GetSoldProducts(ProductShopContext context)
        {
            XmlHelper xmlHelper = new XmlHelper();

            var users = context.Users
                .Where(u => u.ProductsSold.Count >= 1)
                .OrderBy(u => u.LastName)
                .ThenBy(u => u.FirstName)
                .Take(5)
                .Select(u => new ExportUserDto()
                {
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    SoldProducts = u.ProductsSold
                        .Select(p => new ExportUserProductDto()
                        {
                            Name = p.Name,
                            Price = p.Price
                        })
                        .ToArray()
                        
                })
                .ToArray();

            return xmlHelper.Serialize(users, "Users");
      
        }

        public static string GetCategoriesByProductsCount(ProductShopContext context)
        {
            XmlHelper xmlHelper = new XmlHelper();

            var categories = context.Categories
                .AsNoTracking()
                .Select(c => new ExportCategoryDto()
                {
                    Name = c.Name,
                    Count = c.CategoryProducts.Count,
                    AveragePrice = c.CategoryProducts.Average(cp => cp.Product.Price),
                    TotalRevenue = c.CategoryProducts.Sum(cp => cp.Product.Price)
                })
                .OrderByDescending(c => c.Count)
                .ThenBy(c => c.TotalRevenue)
                .ToArray();

            return xmlHelper.Serialize(categories, "Categories");

        }

    }
}
