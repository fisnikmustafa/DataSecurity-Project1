using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Security.Cryptography;
using MySqlX.XDevAPI;

namespace UDPServer
{
    class User
    {

        private static string connString = "Server=localhost;Port=3307;Database=siguridb;Uid=root;Pwd=;";


        private static string generateSalt()
        {
            Random rnd = new Random(DateTime.Now.Millisecond);
            return rnd.Next(100000, 1000000).ToString();

        }

        public static string generateSaltedHash(string password, string salt)
        {
            string passwordSalt = password + salt;
            SHA1CryptoServiceProvider objHash = new SHA1CryptoServiceProvider();

            byte[] bytePasswordSalt = Encoding.UTF8.GetBytes(passwordSalt);
            byte[] byteSaltedPasswordHash = objHash.ComputeHash(bytePasswordSalt);

            return Convert.ToBase64String(byteSaltedPasswordHash);
        }

        public static void insert(string firstname, string lastname, string username, string email, string password)
        {
            MySqlConnection conn = new MySqlConnection(connString);

            try
            {
                conn.Open();

                string salt = generateSalt();
                string saltedHashPassword = generateSaltedHash(password, salt);

                string strCommand = "insert into users(firstname, lastname, username, password, salt) values(" +
                    "'" + firstname + "','" + lastname + "','" + username + "','" + saltedHashPassword + "','" + salt + "')";

                MySqlCommand sqlCommand = new MySqlCommand(strCommand, conn);

                int retValue = sqlCommand.ExecuteNonQuery();

                if (retValue > 0)
                {
                    Console.WriteLine("Te dhenat u ruajten me sukses!");
                }
                else
                {
                    Console.WriteLine("Ruajtja deshtoi!");
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("Lidhja me databazen deshtoi!\n" + ex.Message);
            }
            finally
            {
                conn.Close();
            }
        }


        public static string getSalt(string username)
        {
            string salt = "";

            MySqlConnection conn = new MySqlConnection(connString);

            MySqlCommand sqlCommandd = conn.CreateCommand();
            sqlCommandd.CommandText = "SELECT salt from users WHERE username ='" + username + "';";

            try
            {
                conn.Open();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lidhja me databazen deshtoi!\n" + ex.Message);
                return null;
            }

            MySqlDataReader reader = sqlCommandd.ExecuteReader();

            while (reader.Read())
            {
                salt = reader.ToString();
            }

            return salt;
        }

        public static bool isUser(string username, string password)
        {
            string salt = getSalt(username);
            string saltPassword = password + salt;

            byte[] byteSaltPassword = Encoding.UTF8.GetBytes(saltPassword);

            SHA1CryptoServiceProvider objHash = new SHA1CryptoServiceProvider();

            byte[] byteSaltedHashPassword = objHash.ComputeHash(byteSaltPassword);
            string saltedHashPassowrd = Convert.ToBase64String(byteSaltedHashPassword);


            MySqlConnection conn = new MySqlConnection(connString);

            MySqlCommand sqlCommandd = conn.CreateCommand();
            sqlCommandd.CommandText = "SELECT * from users WHERE username ='" + username + "' AND password='" + saltedHashPassowrd + "';";

            try
            {
                conn.Open();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lidhja me databazen deshtoi!\n" + ex.Message);
                return false;
            }

            MySqlDataReader reader = sqlCommandd.ExecuteReader();

            if (!reader.Read())
            {
                return false;
            } else
            {
                return true;
            }
        }



        public static string[] getUserInfo(string username, string password)
        {
            string salt = getSalt(username);
            string saltPassword = password + salt;

            byte[] byteSaltPassword = Encoding.UTF8.GetBytes(saltPassword);

            SHA1CryptoServiceProvider objHash = new SHA1CryptoServiceProvider();

            byte[] byteSaltedHashPassword = objHash.ComputeHash(byteSaltPassword);
            string saltedHashPassowrd = Convert.ToBase64String(byteSaltedHashPassword);


            MySqlConnection conn = new MySqlConnection(connString);

            MySqlCommand sqlCommandd = conn.CreateCommand();
            sqlCommandd.CommandText = "SELECT firstname, lastname, username, email from fiekusers where username = '" + username + "' AND password='" + saltedHashPassowrd + "';";

            try
            {
                conn.Open();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lidhja me databazen deshtoi!\n" + ex.Message);
                return null;
            }

            MySqlDataReader reader = sqlCommandd.ExecuteReader();

            if (!reader.Read())
            {
                reader.Close();
                return null;
            }

            string[] str = new string[4];
            while (reader.Read())
            {
                str[0] = reader.GetValue(0).ToString();
                str[1] = reader.GetValue(1).ToString();
                str[2] = reader.GetValue(2).ToString();
                str[3] = reader.GetValue(3).ToString();
            }
            reader.Close();

            return str;

        }




    }
}
