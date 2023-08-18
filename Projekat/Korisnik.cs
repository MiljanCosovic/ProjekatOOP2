using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Text.RegularExpressions;

namespace Projekat
{
    public class Korisnik
    {

        public int ID { get; private set; }

        public string Username { get; private set; }
        
        public string UserTip { get; private set; }


        private const string ConnectionString = "Data Source=DESKTOP-1D72NJG;Initial Catalog=c#projekat;Integrated Security=True";

        public string Prijava(string unetiUsername, string password, string userLogin)
        {
            if (string.IsNullOrWhiteSpace(unetiUsername) || string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(userLogin))
            {
                return "Molimo vas popunite sva polja.";
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM login WHERE username = @username AND lozinka = @lozinka AND userLogin = @userLogin", conn);
                    cmd.Parameters.AddWithValue("@username", unetiUsername);
                    cmd.Parameters.AddWithValue("@lozinka", password);
                    cmd.Parameters.AddWithValue("@userLogin", userLogin);

                    int count = (int)cmd.ExecuteScalar();

                    if (count > 0)
                    {
                        ID = IDKorisnika(unetiUsername);
                        Username = unetiUsername;
                        UserTip = userLogin;

                        if (userLogin == "admin")
                        {
                            Admin a = new Admin();
                            a.Show();
                        }
                        else if (userLogin == "user")
                        {
                            User u = new User();
                            u.Show();
                           
                        }
                        
                        return "Uspješno ste prijavljeni!";
                    }
                    else
                    {
                       
                        bool TacanUsername = ZauzetUsername(unetiUsername);
                        bool TacanaLozinka = ProveriLozinku(unetiUsername, password);
                        bool TacanNalog = ProveriUserRole(unetiUsername, userLogin);

                        if (!TacanUsername)
                        {
                            return "Pogrešno korisničko ime.";
                        }
                        else if (!TacanaLozinka)
                        {
                            return "Pogrešna lozinka.";
                        }
                        else if (!TacanNalog)
                        {
                            return "Pogrešna uloga.";
                        }
                        else
                        {
                            return "Nepoznata greška pri prijavi.";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return "Došlo je do greške pri prijavi: " + ex.Message;
            }
        }

        public static int IDKorisnika(string username)
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();

                string query = "SELECT UserID FROM Registracija WHERE UsernameR = @username";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@username", username);
                    object result = cmd.ExecuteScalar();

                    if (result != null && result != DBNull.Value && int.TryParse(result.ToString(), out int id))
                    {
                        return id;
                    }

                    return -1;
                }
            }
        }
     



        private bool ZauzetUsername(string username)
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();

                string checkUsernameQuery = "SELECT COUNT(*) FROM login WHERE username = @username";

                using (SqlCommand cmd = new SqlCommand(checkUsernameQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@username", username);
                    int usernameCount = (int)cmd.ExecuteScalar();

                    if (usernameCount > 0)
                        return true;
                }
            }

            return false;
        }

        private bool ProveriLozinku(string username, string password)
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();

                string checkPasswordQuery = "SELECT COUNT(*) FROM login WHERE username = @username AND lozinka = @lozinka";

                using (SqlCommand cmd = new SqlCommand(checkPasswordQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@username", username);
                    cmd.Parameters.AddWithValue("@lozinka", password);
                    int passwordCount = (int)cmd.ExecuteScalar();

                    if (passwordCount > 0)
                        return true;
                }
            }

            return false;
        }

        private bool ProveriUserRole(string username, string userRole)
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();

                string checkUserRoleQuery = "SELECT COUNT(*) FROM login WHERE username = @username AND userLogin = @userLogin";

                using (SqlCommand cmd = new SqlCommand(checkUserRoleQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@username", username);
                    cmd.Parameters.AddWithValue("@userLogin", userRole);
                    int userRoleCount = (int)cmd.ExecuteScalar();

                    if (userRoleCount > 0)
                        return true;
                }
            }

            return false;
        }

        public string Registracija(string ime, string prezime, string email, string username, string lozinka, string potvrdaLozinke)
        {
            if (string.IsNullOrWhiteSpace(ime) || string.IsNullOrWhiteSpace(prezime) || string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(lozinka) || string.IsNullOrWhiteSpace(potvrdaLozinke))
            {
                return "Molimo vas popunite sva polja.";
            }

            if (lozinka != potvrdaLozinke)
            {
                return "Lozinka i potvrda lozinke se ne poklapaju.";
            }
            else
            {
                if (!IsValidEmail(email))
                {
                    return "Unesite validnu e-mail adresu.";
                }

               
                if (!JakaSifra(lozinka))
                {
                    return "Lozinka mora da sadrži bar 8 karaktera, velika i mala slova i brojeve.";
                }
            }

            if (ZauzetEmail(email))
            {
                return "Korisnik sa istim email-om imenom već postoji.";
            }

            if (ZauzetUser(username))
            {
                return "Korisnik sa istim korisničkim imenom već postoji.";
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();

                    string insertQuery = "INSERT INTO Registracija (Ime, Prezime, Email, UsernameR, Lozinka) " +
                        "VALUES (@ime, @prezime, @email, @username, @lozinka)";

                    using (SqlCommand cmd = new SqlCommand(insertQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@ime", ime);
                        cmd.Parameters.AddWithValue("@prezime", prezime);
                        cmd.Parameters.AddWithValue("@email", email);
                        cmd.Parameters.AddWithValue("@username", username);
                        cmd.Parameters.AddWithValue("@lozinka", lozinka);

                        cmd.ExecuteNonQuery();
                    }

                    string insertLoginQuery = "INSERT INTO login (username, lozinka, userLogin) VALUES (@username, @lozinka, @userLogin)";

                    using (SqlCommand cmd = new SqlCommand(insertLoginQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@username", username);
                        cmd.Parameters.AddWithValue("@lozinka", lozinka);
                        cmd.Parameters.AddWithValue("@userLogin", "user");

                        cmd.ExecuteNonQuery();
                    }

                    return "Uspešno ste se registrovali! Možete se sada ulogovati i koristiti aplikaciju!";
                }
            }
            catch (Exception ex)
            {
                return "Došlo je do greške pri registraciji: " + ex.Message;
            }
        }

        private bool ZauzetEmail(string email)
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();

                string checkEmailQuery = "SELECT COUNT(*) FROM Registracija WHERE Email = @email";

                using (SqlCommand cmd = new SqlCommand(checkEmailQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@email", email);
                    int emailCount = (int)cmd.ExecuteScalar();

                    if (emailCount > 0)
                        return true;
                }
            }

            return false;
        }

        private bool ZauzetUser(string username)
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();

                string checkUsernameQuery = "SELECT COUNT(*) FROM Registracija WHERE UsernameR = @username";

                using (SqlCommand cmd = new SqlCommand(checkUsernameQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@username", username);
                    int usernameCount = (int)cmd.ExecuteScalar();

                    if (usernameCount > 0)
                        return true;
                }
            }

            return false;
        }

        private bool IsValidEmail(string email)
        {  
            string pattern = @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$";
            return Regex.IsMatch(email, pattern);
        }

        private bool JakaSifra(string password)
        {
            bool hasLength = password.Length >= 8;
            bool hasUpperCase = password.Any(c => char.IsUpper(c));
            bool hasLowerCase = password.Any(c => char.IsLower(c));
            bool hasDigit = password.Any(c => char.IsDigit(c));

            return hasLength && hasUpperCase && hasLowerCase && hasDigit;
        }
    }
}
