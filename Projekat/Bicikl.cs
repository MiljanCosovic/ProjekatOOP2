using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projekat
{
    public class Bicikl
    {
        private const string ConnectionString = "Data Source=DESKTOP-1D72NJG;Initial Catalog=c#projekat;Integrated Security=True";
        public int Id { get; set; }
        public string Opis { get; set; }
        public float Cena { get; set; }
        public byte[] Slika { get; set; }
        public string Status { get; set; }

        public Bicikl(int id, string opis, float cena, byte[] slika, string status)
        {
            Id = id;
            Opis = opis;
            Cena = cena;
            Slika = slika;
            Status = status;
        }
        public Bicikl()
        {
            
        }


        public void DodajBicikl(string opis, float cena, byte[] slika, string status)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();

                    string query = "INSERT INTO Bicikli (Opis, Cena, Slika, Status) VALUES (@Opis, @Cena, @Slika, @Status)";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Opis", opis);
                        cmd.Parameters.AddWithValue("@Cena", cena);
                        cmd.Parameters.AddWithValue("@Slika", slika);
                        cmd.Parameters.AddWithValue("@Status", status);

                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Došlo je do greške prilikom upisa u bazu: " + ex.Message);
            }
        }


       

    }
}
