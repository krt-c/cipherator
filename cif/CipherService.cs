using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace cif
{
	/// <summary>
	/// 
	/// </summary>
	public static class CipherService
	{
		private const int KeySize = 128;
		private const int DerivationIterations = 1000;

		public static string Encrypt(string plainText, string passPhrase)
		{
			var saltStringBytes = Generate256BitsOfRandomEntropy();
			var ivStringBytes = Generate256BitsOfRandomEntropy();
			var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
			using (var password = new Rfc2898DeriveBytes(passPhrase, saltStringBytes, DerivationIterations))
			{
				var keyBytes = password.GetBytes(KeySize / 8);
				using (var symmetricKey = new RijndaelManaged())
				{
					symmetricKey.BlockSize = 128;
					symmetricKey.Mode = CipherMode.CBC;
					symmetricKey.Padding = PaddingMode.PKCS7;
					using (var encryptor = symmetricKey.CreateEncryptor(keyBytes, ivStringBytes))
					{
						using (var memoryStream = new MemoryStream())
						{
							using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
							{
								cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
								cryptoStream.FlushFinalBlock();
								var cipherTextBytes = saltStringBytes;
								cipherTextBytes = cipherTextBytes.Concat(ivStringBytes).ToArray();
								cipherTextBytes = cipherTextBytes.Concat(memoryStream.ToArray()).ToArray();
								memoryStream.Close();
								cryptoStream.Close();
								return Convert.ToBase64String(cipherTextBytes);
							}
						}
					}
				}
			}
		}

		private static byte[] Generate256BitsOfRandomEntropy()
		{
			var randomBytes = new byte[16];
			using (var rngCsp = new RNGCryptoServiceProvider())
			{
				rngCsp.GetBytes(randomBytes);
			}
			return randomBytes;
		}
	}
}