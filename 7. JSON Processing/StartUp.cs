using CarDealer.Data;
using CarDealer.DTOs.Export;
using CarDealer.DTOs.Import;
using CarDealer.Models;
using Castle.Core.Resource;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Globalization;
using System.IO;

namespace CarDealer
{
    public class StartUp
    {
        public static void Main()
        {
            var context = new CarDealerContext();

            string json = File.ReadAllText("../../../Datasets/sales.json");
            string result = GetCarsFromMakeToyota(context);

            Console.WriteLine(result);
        }

        public static string ImportSuppliers(CarDealerContext context, string inputJson)
        {
            ImportSupplierDto[] supplierDtos =
                JsonConvert.DeserializeObject<ImportSupplierDto[]>(inputJson);

            ICollection<Supplier> validSuppliers = new HashSet<Supplier>();

            foreach (var supplierDto in supplierDtos)
            {
                if(string.IsNullOrEmpty(supplierDto.Name))
                {
                    continue;
                }

                var supplier = new Supplier()
                {
                    Name = supplierDto.Name,
                    IsImporter = supplierDto.IsImporter
                };

                validSuppliers.Add(supplier);
            }

            context.Suppliers.AddRange(validSuppliers);
            context.SaveChanges();

            return $"Successfully imported {validSuppliers.Count}.";
        }

        public static string ImportParts(CarDealerContext context, string inputJson)
        {
            ImportPartDto[] partDtos = 
                JsonConvert.DeserializeObject<ImportPartDto[]>(inputJson);

            ICollection<int> validSupplierIds = context.Suppliers
                .AsNoTracking()
                .Select(s => s.Id)
                .ToList();

            ICollection<Part> validParts = new HashSet<Part>();


            foreach (var partDto in partDtos)
            {
                if(partDto.Name == null)
                {
                    continue;
                }   

                if(partDto.SupplierId.HasValue == false ||
                    validSupplierIds.Contains(partDto.SupplierId.Value) == false)
                {
                    continue;
                }

                var part = new Part()
                {
                    Name = partDto.Name,    
                    Price = partDto.Price,
                    Quantity = partDto.Quantity,
                    SupplierId = partDto.SupplierId.Value
                }; 

                validParts.Add(part);
            }

            context.Parts.AddRange(validParts);
            context.SaveChanges();

            return $"Successfully imported {validParts.Count}.";
        }

        public static string ImportCars(CarDealerContext context, string inputJson)
        {
            ImportCarDto[] carDtos = 
                JsonConvert.DeserializeObject<ImportCarDto[]>(inputJson);

            ICollection<Car> validCars = new HashSet<Car>();

            foreach (var carDto in carDtos)
            {
                if(carDto.Make == null || carDto.Model == null)
                {
                    continue;
                }

                Car car = new Car()
                {
                    Model = carDto.Model,
                    Make = carDto.Make,
                    TraveledDistance = carDto.TraveledDistance,
                };


                foreach (var partId in carDto.Parts)
                {
                    if(car.PartsCars.Any(cp => cp.PartId == partId) == false)
                    {
                        PartCar partCar = new PartCar()
                        {
                            PartId = partId,
                            CarId = car.Id
                        };

                        car.PartsCars.Add(partCar);
                    }
               
                }

                validCars.Add(car);
            }

            context.Cars.AddRange(validCars);
            context.SaveChanges();


            return $"Successfully imported {validCars.Count}.";
        }

        public static string ImportCustomers(CarDealerContext context, string inputJson)
        {
            ImportCustomerDto[] customerDtos =
                JsonConvert.DeserializeObject<ImportCustomerDto[]>(inputJson);

            ICollection<Customer> validCustomers = new HashSet<Customer>();

            foreach (var customerDto in customerDtos)
            {
                if(customerDto.Name == null || customerDto.BirthDate == null)
                {
                    continue;
                }

                Customer customer = new Customer()
                {
                    Name = customerDto.Name,
                    BirthDate = DateTime.Parse(customerDto.BirthDate, CultureInfo.InvariantCulture),
                    IsYoungDriver = customerDto.IsYoungDriver
                };

                validCustomers.Add(customer);
            }

            context.Customers.AddRange(validCustomers);
            context.SaveChanges();

            return $"Successfully imported {validCustomers.Count}.";
        }

        public static string ImportSales(CarDealerContext context, string inputJson)
        {
            ImportSaleDto[] saleDtos =
                JsonConvert.DeserializeObject<ImportSaleDto[]>(inputJson);

            ICollection<Sale> validSales = new HashSet<Sale>();

            foreach (var saleDto in saleDtos)
            {
                if(saleDto.CarId == null || saleDto.CustomerId == null)
                {
                    continue;
                }

                Sale sale = new Sale()
                {
                    CarId = saleDto.CarId,
                    CustomerId = saleDto.CustomerId,
                    Discount = saleDto.Discount
                };

                validSales.Add(sale);
            }

            context.Sales.AddRange(validSales);
            context.SaveChanges();

            return $"Successfully imported {validSales.Count}.";
        }


        public static string GetOrderedCustomers(CarDealerContext context)
        {
            ExportCustomerDto[] customers = context.Customers
                .AsNoTracking()
                .OrderBy(c => c.BirthDate)
                .ThenBy(c => c.IsYoungDriver == false)
                .Select(c => new ExportCustomerDto()
                {
                    Name = c.Name,
                    BirthDate = c.BirthDate.ToString("dd/MM/yyyy"),
                    IsYoungDriver = c.IsYoungDriver
                })
                .ToArray();

            return JsonConvert.SerializeObject(customers, Formatting.Indented);
        }


        public static string GetCarsFromMakeToyota(CarDealerContext context)
        {
            ExportToyotaCarDto[] cars = context.Cars
                .AsNoTracking()
                .Where(c => c.Make == "Toyota")
                .OrderBy(c => c.Model)
                .ThenByDescending(c => c.TraveledDistance)
                .Select(c => new ExportToyotaCarDto()
                {
                    Id = c.Id,
                    Make = c.Make,
                    Model = c.Model,
                    TraveledDistance = c.TraveledDistance
                })
                .ToArray();


            return JsonConvert.SerializeObject(cars, Formatting.Indented);
        }

    }
}
