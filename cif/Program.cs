using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace cif
{
	class Program
	{

		/// <summary>
		/// cif is a tool to encrypt or decrypt any text by using a secret key.
		/// </summary>
		/// <param name="args"></param>
		public static async Task Main(string[] args)
		{
			var command = new RootCommand();
			try
			{
				if (args[0] == "encrypt")
				{
					var encryptCommand = new Command("encrypt")
					{
						new Option("-s", "The secret required to encryption.") {Required = true, Argument = new Argument<string>()},
						new Option("-t", "The text to encrypt.") {Required = true, Argument = new Argument<string>()}
					};

					encryptCommand.Handler = CommandHandler.Create<string, string>(Encrypt);

					await encryptCommand.InvokeAsync(args);

					command.Add(encryptCommand);
				}

				if (args[0] == "decrypt")
				{

					var decryptCommand = new Command("decrypt")
					{
						new Option("-s", "The secret required to for decryption."){Required = true, Argument = new Argument<string>()},
						new Option("-c", "The cipher to decrypt to text.") {Required = true, Argument = new Argument<string>()}
					};

					decryptCommand.Handler = CommandHandler.Create<string, string>(Decrypt);

					await decryptCommand.InvokeAsync(args);


					command.Add(decryptCommand);
				}
			}
			catch (NullReferenceException)
			{
				Console.WriteLine("Invalid sub command was specified. Specify if you want to encrypt or decrypt.");
			}
		}

		public static void Encrypt(string secret, string text)
		{
			var cipher = CipherService.Encrypt(text, secret);

			Console.WriteLine($"The cipher is: {cipher}");
		}

		public static void Decrypt(string secret, string cipher)
		{
			try
			{
				var text = CipherService.Decrypt(cipher, secret);

				Console.WriteLine($"The text is: {text}");
			}
			catch (CryptographicException)
			{
				Console.WriteLine("Something went wrong while decrypting the cipher....");
			}
		}
	}
}
