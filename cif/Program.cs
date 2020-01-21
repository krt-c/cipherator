using System;
using System.CommandLine;
using System.CommandLine.Invocation;
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

			if (args[0] == "encrypt")
			{
				var encryptCommand = new Command("encrypt")
				{
					new Option("-secret", "The secret required to encryption.") {Argument = new Argument<string>()},
					new Option("-text", "The text to encrypt.") {Argument = new Argument<string>()}
				};

				encryptCommand.Handler = CommandHandler.Create<string, string>(Encrypt);

				await encryptCommand.InvokeAsync(args);

				command.Add(encryptCommand);
			}

			if (args[0] == "decrypt")
			{

				var decryptCommand = new Command("decrypt")
				{
					new Option("-secret", "The secret required to for decryption.") {Argument = new Argument<string>()},
					new Option("-cipher", "The cipher to decrypt to text.") {Argument = new Argument<string>()}
				};

				decryptCommand.Handler = CommandHandler.Create<string, string>(Decrypt);

				await decryptCommand.InvokeAsync(args);


				command.Add(decryptCommand);
			}
		}

		public static void Encrypt(string secret, string text)
		{
			var cipher = CipherService.Encrypt(text, secret);

			Console.WriteLine($"The cipher is: {cipher}");
		}

		public static void Decrypt(string secret, string cipher)
		{
			var text = CipherService.Decrypt(cipher, secret);

			Console.WriteLine($"The text is: {text}");
		}
	}
}
