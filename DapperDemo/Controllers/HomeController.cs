using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace DapperDemo.Controllers
{
    public class HomeController : Controller
    {
        public class Person
        {
            public string FirstName { get; set; }
            public string MiddleName { get; set; }
            public string LastName { get; set; }
        }

        public class Email
        {
            public string EmailAddress { get; set; }

        }

        public class Phone
        {
            public string PhoneNumber { get; set; }

        }

        class PersonData
        {
            public Person Person { get; set; }
            public IList<Email> Emails { get; set; }
            public IList<Phone> Phones { get; set; }
        }

        [Route("api/persons")]
        public async Task<IActionResult> Index()
        {
            IDbConnection connection = new SqlConnection(@"Data Source=CODEHAKS\MSSQL2019;Initial Catalog=AdventureWorks2019;integrated security=true");

            var q = "SELECT TOP(10) FirstName,MiddleName,LastName FROM [Person].[Person]";

            var persons = (await connection.QueryAsync<Person>(q)).ToList();

            return Ok(persons);
        }


        [Route("api/person/{id}")]
        public async Task<IActionResult> Get(int id)
        {
            IDbConnection connection = new SqlConnection(@"Data Source=CODEHAKS\MSSQL2019;Initial Catalog=AdventureWorks2019;integrated security=true");

            var selectedId = id;

            var sql = $@"select * from [Person].[Person] where BusinessEntityID = {id}
                         select * from [Person].[EmailAddress] where BusinessEntityID = {id}
                         select * from [Person].[PersonPhone] where BusinessEntityID = {id}";

            var result = new PersonData();

            using (var multi = connection.QueryMultiple(sql, new { id = selectedId }))
            {
                var person = multi.Read<Person>().Single();
                var emails = multi.Read<Email>().ToList();
                var phones = multi.Read<Phone>().ToList();

                result.Person = person;
                result.Emails = emails.ToList();
                result.Phones = phones.ToList();
            }

            return Ok(result);
        }
    }
}
