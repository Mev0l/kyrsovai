using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace Сryptographer.models
{
    public class Cryptographer : INotifyPropertyChanged
    {

        protected string _leftText;
        protected string _rightText;
        protected int _shift;
        public bool isLeftOriginal = true;
        private bool isLeftStarter = false;
        private bool isRightStarter = false;
        private bool switched = false;
        private int _cryptoMode;
        private string _aesKey;
        private string _aesKeyStatus = "ERR";
        public string LeftText
        {
            get { return _leftText; }
            set 
            {
                _leftText = value; OnPropertyChanged(nameof(LeftText));
                if (!switched) { 
                isLeftStarter = true;
                    if (!isRightStarter) EncryptDecrypt();
                isLeftStarter = false;
                }
            }
        }
        public string RightText
        {
            get { return _rightText; }
            set 
            { 
                _rightText = value; OnPropertyChanged(nameof(RightText));
                if (!switched) { 
                isRightStarter = true;
                    if (!isLeftStarter) EncryptDecrypt();
                isRightStarter = false;
                 }
            }
        }

        public int Shift 
        {
            get { return _shift; }
            set { _shift = value; OnPropertyChanged(nameof(Shift));}
        }
        public int CryptoMode 
        {
            get { return _cryptoMode; }
            set { _cryptoMode = value; OnPropertyChanged(nameof(CryptoMode)); }
        }
        public string AESKey 
        {
            get { return _aesKey; }
            set { _aesKey = value; OnPropertyChanged(nameof(AESKey));  CheckAESKeyStatus(); }
        }
        public string AESKeyStatus 
        {
            get {return _aesKeyStatus;}
            set { _aesKeyStatus = value; OnPropertyChanged(nameof(AESKeyStatus)); }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

        private bool CheckAESKeyStatus() 
        {
            bool result = false;
            if (AESKey == null) return result;
            if (AESKey.Length == 16 || AESKeyStatus.Length == 24 || AESKey.Length == 32) result = true;

            if (result) AESKeyStatus = "OK";
            else AESKeyStatus = "ERR";


            return result;
        }
        public void SwitchTexts() 
        {
            switched = true;
            string tmp = LeftText;
            LeftText = RightText;
            RightText = tmp;
            switched = false;
        }

        public void EncryptDecrypt() 
        {
            switch (CryptoMode)
            {
                case 0:
                    CaesarEncryptDecrypt();
                    break;
                case 1:
                    if (CheckAESKeyStatus()) AESEncryptDecrypt();
                    break;
            }
        }




        //AES

        private void AESEncryptDecrypt() 
        {
            if (isLeftStarter)
            {
                if (isLeftOriginal && LeftText != string.Empty)
                {
                    RightText = AESEncrypt(LeftText, AESKey);
                }
                if (!isLeftOriginal && LeftText != string.Empty)
                {
                    try
                    {
                        RightText = AESDecrypt(LeftText, AESKey);
                    }
                    catch { RightText = "AES Decrypt error"; }
                }
            }
            if (isRightStarter)
            {
                if (isLeftOriginal && RightText != string.Empty)
                {
                    try
                    {
                        LeftText = AESDecrypt(RightText, AESKey);
                    }
                    catch { LeftText = "AES Decrypt error"; }
                }
                if (!isLeftOriginal && RightText != string.Empty) LeftText = AESEncrypt(RightText, AESKey);
            }
        }




        public void AESGenerateRandomKey() 
        {
            AESKey = GenerateRandomKeyAES(32);
        }

        static string GenerateRandomKeyAES(int length)
        {
            const string validChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            StringBuilder sb = new StringBuilder();

            using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
            {
                byte[] randomBytes = new byte[length];
                rng.GetBytes(randomBytes);

                foreach (byte b in randomBytes)
                {
                    sb.Append(validChars[b % validChars.Length]);
                }
            }

            return sb.ToString();
        }



        static string AESEncrypt(string plainText, string key)
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Encoding.UTF8.GetBytes(key);
                aesAlg.IV = GenerateRandomIV(aesAlg.BlockSize / 8);

                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(plainText);
                        }
                    }

                    return Convert.ToBase64String(aesAlg.IV.Concat(msEncrypt.ToArray()).ToArray());
                }
            }
        }

        static string AESDecrypt(string cipherText, string key)
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Encoding.UTF8.GetBytes(key);
                aesAlg.IV = Convert.FromBase64String(cipherText).Take(aesAlg.BlockSize / 8).ToArray();

                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msDecrypt = new MemoryStream(Convert.FromBase64String(cipherText).Skip(aesAlg.BlockSize / 8).ToArray()))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            return srDecrypt.ReadToEnd();
                        }
                    }
                }
            }
        }

        static byte[] GenerateRandomIV(int size)
        {
            byte[] iv = new byte[size];
            using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(iv);
            }
            return iv;
        }
    



    //Caesar
    void CaesarEncryptDecrypt() 
        {
            if (isLeftStarter)
            {
                if (isLeftOriginal)
                {
                    RightText = CaesarEncryptString(LeftText, false);
                }
                if (!isLeftOriginal) RightText = CaesarEncryptString(LeftText, true);
            }
            if (isRightStarter)
            {
                if (isLeftOriginal)
                {
                    LeftText = CaesarEncryptString(RightText, true);
                }
                if (!isLeftOriginal) LeftText = CaesarEncryptString(RightText, false);
            }
        }



        private string CaesarEncryptString(string str, bool decrypt) 
        {
            StringBuilder builder = new();
            //string result = string.Empty;
            foreach (char chr in str) 
            {
                builder.Append(CaesarEncryptChar(chr, decrypt));
            }
           return builder.ToString();
        }

        private char CaesarEncryptChar(char chr, bool decrypt) 
        {
            char result = chr;
            chr = chr.ToString().ToUpper()[0];
            bool isUpper = true;
            if (chr != result) isUpper = false;
            int shift = Shift;
            if (decrypt) shift *= -1;
            if (rusAlphabet.ContainsValue(chr))
            {
                shift += rusAlphabet.FirstOrDefault(x => x.Value == chr).Key;
                while (shift > 33) 
                {
                    shift -= 33;
                }
                if (shift <= 0) shift = 33 + shift;
                result = rusAlphabet[shift];
            }
            else if (engAlphabet.ContainsValue(chr))
            {
                shift += engAlphabet.FirstOrDefault(x => x.Value == chr).Key;
                while (shift > 26)
                {
                    shift -= 26;
                }
                if (shift <= 0) shift = 26 + shift;
                result = engAlphabet[shift];
            }
            if (!isUpper) result = result.ToString().ToLower()[0];
            return result;
        }
       



        private Dictionary<int, char> rusAlphabet = new Dictionary<int, char>
        {
            {1, 'А'},
            {2, 'Б'},
            {3, 'В'},
            {4, 'Г'},
            {5, 'Д'},
            {6, 'Е'},
            {7, 'Ё'},
            {8, 'Ж'},
            {9, 'З'},
            {10, 'И'},
            {11, 'Й'},
            {12, 'К'},
            {13, 'Л'},
            {14, 'М'},
            {15, 'Н'},
            {16, 'О'},
            {17, 'П'},
            {18, 'Р'},
            {19, 'С'},
            {20, 'Т'},
            {21, 'У'},
            {22, 'Ф'},
            {23, 'Х'},
            {24, 'Ц'},
            {25, 'Ч'},
            {26, 'Ш'},
            {27, 'Щ'},
            {28, 'Ъ'},
            {29, 'Ы'},
            {30, 'Ь'},
            {31, 'Э'},
            {32, 'Ю'},
            {33, 'Я'}
        };

        private Dictionary<int, char> engAlphabet = new Dictionary<int, char>
        {
            {1, 'A'},
            {2, 'B'},
            {3, 'C'},
            {4, 'D'},
            {5, 'E'},
            {6, 'F'},
            {7, 'G'},
            {8, 'H'},
            {9, 'I'},
            {10, 'J'},
            {11, 'K'},
            {12, 'L'},
            {13, 'M'},
            {14, 'N'},
            {15, 'O'},
            {16, 'P'},
            {17, 'Q'},
            {18, 'R'},
            {19, 'S'},
            {20, 'T'},
            {21, 'U'},
            {22, 'V'},
            {23, 'W'},
            {24, 'X'},
            {25, 'Y'},
            {26, 'Z'}
        };


    }




}
