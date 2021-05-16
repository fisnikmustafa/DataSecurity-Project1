using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ConsoleCeaser
{
    class Program
    {
       //Metoda Main 
        static void Main(string[] args)
        {
            string path = "", key = "";

            Console.WriteLine("Shenoni shtegun e text fajllit: (psh: C:\\Users\\DELL\\Desktop\\test.txt)");
            path = Console.ReadLine(); //lexo shtegun nga useri

            if (File.Exists(path)) // nese shtegu ekziston
            {
                string plaintext = File.ReadAllText(path); //lexo textin nga text file
                Console.WriteLine("Shenoni celesin: "); 
                key = Console.ReadLine(); //key merr vleren e celesit te dhene nga useri 

                Console.WriteLine(); // rresht i zbrazet

                string ciphertext = Encrypt(plaintext, int.Parse(key)); // thirret metoda Encrypt dhe i jepet vlera e saj ciphertext
                Console.WriteLine("Teksti i enkriptuar eshte: " + ciphertext); // shfaq tekstin e enkriptuar
                Analizo(ciphertext); // therret metoden Analizo
            }

            else // nese shtegu nuk ekziton shfaqet mesazhi me poshte
            {
                Console.WriteLine("Shtegu qe keni shenuar nuk ekziston!");
                Console.ReadLine(); // e kemi perdorur qe te mos nderprehet menjeher console app
            }
            
        }

        //Metoda ToNumber mundeson kthimin e pozites se karakterit 
        static int ToNumber(char c)
        {
            return c - 'A';
        }

        //Metoda Encrypt perdoret per enkriptimin e textit i cili mirret nga text-file i dhene nga useri
        static string Encrypt(string path, int key)
        {
            StringBuilder sbCiphertext = new StringBuilder(path); // permes StringBuilder kemi mundesi nderrimi te vlerave te karatereve  

            for (int i = 0; i < path.Length; i++) //per me ju qas cdo karakteri te stringut
            {
                char ch = path[i]; //ndan secilin karater nga stringu
                if (Char.IsUpper(ch)) // nese karakteri eshte shkronje e madhe
                {
                    int posCh = ch - 'A';       // gjetja e pozites se karakterit
                    posCh = (posCh + key) % 26; // vendosja e pozites se re se karakterit 

                    ch = (char)(posCh + 'A'); // kthejme poziten e re ne karakter
                    sbCiphertext[i] = ch; // zevendesojme karakterin ne poziten perkatese
                }

                // kryen te njejten pune si me larte vetem se per shkronja te vogla
                if (Char.IsLower(ch))
                {
                    int posCh = ch - 'a';
                    posCh = (posCh + key) % 26;

                    ch = (char)(posCh + 'a');
                    sbCiphertext[i] = ch;
                }

            }
            return sbCiphertext.ToString(); // kthejme rezultatin e fituar si string
        }


        //Metoda Decrypt mundeson dekriptimin e tekstit te enkriptuar
        //Ecuria e punes eshte pakashume e ngjashme si metoda Encrypt me disa dallime 
        static string Decrypt(string ciphertext, int key)
        {

            StringBuilder sbDecryptedtext = new StringBuilder(ciphertext);
            for (int i = 0; i < ciphertext.Length; i++)
            {
                char ch = ciphertext[i];
                if (Char.IsUpper(ch))
                {
                    int posCh = ch - 'A'; // kthejme poziten e karakterit 
                    posCh = (posCh - key + 26) % 26; // vendosja e re e pozites se karakterit (shtojme +26 si shkak qe
                                                     // kur del numri negativ, te fitojme numer pozitiv)

                    ch = (char)(posCh + 'A');
                    sbDecryptedtext[i] = ch;
                }

                else if (Char.IsLower(ch))
                {
                    int posCh = ch - 'a';
                    posCh = (posCh - key + 26) % 26;

                    ch = (char)(posCh + 'a');
                    sbDecryptedtext[i] = ch;
                }
            }
            return sbDecryptedtext.ToString();
        }




        public const string ENGLISH_ALPHABET = "ABCDEFGHIJKLMNOPQRSTUVWXYZ"; // inicalizimi i alfabetit anglisht

        //Metoda array Numero
        static int[] Numero(string message) 
        {
            int[] lettercount = new int[ENGLISH_ALPHABET.Length]; // array Lettercount te kete 26 anetar
            foreach (char letter in message) //per cdo karatker(shkronje) qe eshte ne tekst
            {
                for (int i = 0; i < ENGLISH_ALPHABET.Length; i++)
                {
                    if (letter == ENGLISH_ALPHABET[i]) // nese karakteri eshte ne alfabetin anglisht
                                                       
                    {
                        int position = ToNumber(letter); // gjendet pozita e karakterit 
                        lettercount[position] += 1; // rritet counti i karakterit te  per 1 
                    }
                }
            }
            return lettercount;
        }

        //Metoda Analizo mundeson gjetjen e celesit sipas frekuencave te shkronjave ne tekst
        static void Analizo(string ciphertext)
        {
            string message = ciphertext.ToUpper(); // e bon message me shkronja te medha
            int max = 0;
            int pozita = 0;

            int[] letterCount = Numero(message); //i jep letterCount array numrin e antareve permes numero

            for (int i = 0; i < letterCount.Length; i++)
            {
                if (letterCount[i] > max)
                {
                    max = letterCount[i];
                    pozita = i;
                }

            }

            int key = pozita - ToNumber('E'); //gjetja e celesit e formules:
                                // celesi = pozita e karakterit me te perdorur ne ciphertext(whitespaces) - vlera e E 

            string tekstiDekriptuar = Decrypt(ciphertext,key); //thirrja e metodes Decrypt dhe vendosja e vleres se
                                                              // tij ne 'tekstiDekriptuar'

            Console.WriteLine(); // rresht i zbrazet
            Console.WriteLine("Teksti i dekriptuar eshte: " + tekstiDekriptuar); // shfaq tekstin e dekriptuar
            Console.WriteLine(); // rresht i zbrazet
            Console.WriteLine("Shkronja qe u perserite me se shumti eshte: " + ENGLISH_ALPHABET[pozita]); //shkronja me e frekuentuar
            Console.WriteLine(); // rresht i zbrazet
            Console.WriteLine("Ajo u perserite: " + letterCount[pozita] + " here"); // sa here perseritet shkronja me e frekuentuar
            Console.WriteLine(); // rresht i zbrazet
            Console.WriteLine("Celesi i perdorur eshte: " + key); // shfaq celesin

            Console.ReadLine(); //e kemi perdorur qe mos te nderprehet console app 
        }
    }

}