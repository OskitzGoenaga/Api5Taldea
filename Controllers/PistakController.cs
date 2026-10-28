using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PistakController : ControllerBase
    {
        // GET: api/<PistakController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // DELETE api/<PistakController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }

        // POST api/<PistakController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<PistakController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // SOFT DELETE api/<PistakController>/5
        [HttpDelete("{id}")]
        public void SoftDelete(int id)
        {
            try
            {

                using var con = new MySqlConnection(ConnectionStrings);

                con.Open();


                using var transaction = con.BeginTransaction();


                string selectSql = "SELECT id FROM pista WHERE id = @PistaId FOR UPDATE";

                using var selectCmd = new MySqlCommand(selectSql, con, transaction);

                selectCmd.Parameters.AddWithValue("@PistaId", pistaId);

                using var reader = selectCmd.ExecuteReader();


                if (reader.HasRows)

                {

                    Console.WriteLine("\nPista hori jada erreserbatuta dago ordu eta data horretan");

                    reader.Close();

                    transaction.Rollback();

                    return;

                }


                reader.Close();


                Console.WriteLine("Sakatu ENTER erreserba egiteko: ");

                Console.ReadLine();


                string insertSql = " erreserba (pista_id, bazkidea_id, data, ordua) VALUES (@PistaId, @BazkideId, @Data, @Ordua)";

                using var insertCmd = new MySqlCommand(insertSql, con, transaction);

                insertCmd.Parameters.AddWithValue("@PistaId", pistaId);

                insertCmd.Parameters.AddWithValue("@BazkideId", bazkideId);

                insertCmd.Parameters.AddWithValue("@Data", data);

                insertCmd.Parameters.AddWithValue("@Ordua", ordua);


                insertCmd.ExecuteNonQuery();


                transaction.Commit();

                arrakasta = true;

                Console.WriteLine("Erreserba egoki exekutatu da\n");

            }

            catch (Exception ex)

            {

                Console.WriteLine($"\nErrorea transakzioan: {ex.Message}");


                if (saiakera == maxSaiakera)

                {

                    Console.WriteLine("Ezin izan da erreserba egin saiakera guztien ondoren.\n");

                }

                else
                {

                    Console.WriteLine("Berriro saiatzen...\n");

                    Thread.Sleep(2000);

                }

            }

        }

    }

}

            }
        }
    }
}
