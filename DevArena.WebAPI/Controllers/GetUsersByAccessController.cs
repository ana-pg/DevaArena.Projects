using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using DevArena.Library;
using Microsoft.AspNetCore.Mvc;

namespace DevArena.WebAPI.Controllers
{
    [Route("api/[controller]")]
    public class GetUsersByAccessController : Controller
    {
        private readonly string ConnString =
            "Server = (localdb)\\MSSQLLocalDB;Database=IdentityServer4;Trusted_Connection=True;MultipleActiveResultSets=true";


        public GetUsersByAccessController()
        {
        }

        private IEnumerable<string> GetForRole(long? role)
        {
            IList<string> result = new List<string>();
            using (var conn = new SqlConnection(ConnString))
            {
                var comm = new SqlCommand($"SELECT * FROM Users WHERE Role >= {role}");
                comm.Connection = conn;
                
                conn.Open();

                var reader = comm.ExecuteReader();
                while (reader.Read())
                {
                    result.Add("Username: " + reader["Username"] + "/ Role: " + reader["Role"] ??
                               reader["Role"].ToString());
                }
            }

            return result;
        }

        [AuthorizeAccess(RoleEnum.Administrator)]
        [HttpGet, Route("/admin")]
        public IEnumerable<string> GetForAdministrator()
        {
            return GetForRole(1);
        }

        [AuthorizeAccess(RoleEnum.Guest)]
        [HttpGet, Route("/guest")]
        public IEnumerable<string> GetForGuest()
        {
            return GetForRole(2);
        }

        // GET api/values
        //[HttpGet]
        //public IEnumerable<string> Get()
        //{
        //    return new string[] { "value1", "value2" };
        //}

        //// GET api/values/5
        //[HttpGet("{id}")]
        //public string Get(int id)
        //{
        //    return "value";
        //}

        //// POST api/values
        //[HttpPost]
        //public void Post([FromBody]string value)
        //{
        //}

        //// PUT api/values/5
        //[HttpPut("{id}")]
        //public void Put(int id, [FromBody]string value)
        //{
        //}

        //// DELETE api/values/5
        //[HttpDelete("{id}")]
        //public void Delete(int id)
        //{
        //}
    }
}
