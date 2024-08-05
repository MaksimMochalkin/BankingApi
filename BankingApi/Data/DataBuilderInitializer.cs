namespace RestaurantBooking.BusinesApi.Data
{
    using BankingApi.Data;
    using BankingApi.Data.Entities;
    using Microsoft.Extensions.DependencyInjection;

    public class DataBuilderInitializer
    {
        public static void Init(IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetRequiredService<BankingAppDbContext>();

            context.Clients.AddRange(new List<Client>
            {
                new Client
                {
                    Id = Guid.NewGuid(),
                    Email = "test1@example.com",
                    FirstName = "John",
                    LastName = "Doe",
                    PersonalId = "123456789",
                    MobileNumber = "1234567890",
                    Sex = "Male",
                    ProfilePhoto = "Photo",
                    Address = new Address
                    {
                        Id = Guid.NewGuid(),
                        Country = "country",
                        City = "city",
                        Street = "street",
                        ZipCode = "zipcode"
                    },
                    Accounts = new List<Account>
                    {
                        new Account
                        {
                            AccountNumber = "1234567890",
                            Balance = 0,
                        }
                    }
                },
                new Client
                {
                    Id = Guid.NewGuid(),
                    Email = "test2@example.com",
                    FirstName = "Jane",
                    LastName = "Doe",
                    PersonalId = "987654321",
                    MobileNumber = "0987654321",
                    Sex = "Female",
                    ProfilePhoto = "Photo",
                    Address = new Address
                    {
                        Id = Guid.NewGuid(),
                        Country = "country",
                        City = "city",
                        Street = "street",
                        ZipCode = "zipcode"
                    },
                    Accounts = new List<Account>
                    {
                        new Account
                        {
                            AccountNumber = "1234567890",
                            Balance = 0,
                        }
                    }
                },
                new Client
                {
                    Id = Guid.NewGuid(),
                    Email = "test3@example.com",
                    FirstName = "Update",
                    LastName = "Doe",
                    PersonalId = "987654322",
                    MobileNumber = "0987654322",
                    Sex = "Female",
                    ProfilePhoto = "Photo",
                    Address = new Address
                    {
                        Id = Guid.NewGuid(),
                        Country = "country",
                        City = "city",
                        Street = "street",
                        ZipCode = "zipcode"
                    },
                    Accounts = new List<Account>
                    {
                        new Account
                        {
                            AccountNumber = "1234567890",
                            Balance = 0,
                        }
                    }
                },
                new Client
                {
                    Id = Guid.NewGuid(),
                    Email = "test4@example.com",
                    FirstName = "Delete",
                    LastName = "Doe",
                    PersonalId = "987654323",
                    MobileNumber = "0987654323",
                    Sex = "Female",
                    ProfilePhoto = "Photo",
                    Address = new Address
                    {
                        Id = Guid.NewGuid(),
                        Country = "country",
                        City = "city",
                        Street = "street",
                        ZipCode = "zipcode"
                    },
                    Accounts = new List<Account>
                    {
                        new Account
                        {
                            AccountNumber = "1234567890",
                            Balance = 0,
                        }
                    }
                }
            });
            context.SaveChanges();
        }
    }
}
