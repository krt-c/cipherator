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
		/// <param name="action">The secret required to encrypt or decrypt.</param>
		/// <param name="args">-secret is the key to encrypt while -text is the text to encrypt.</param>
		public static async Task Main(string action, string[] args = null)
		{

			Console.WriteLine("Welcome to Cif the encryptor and decryptor!");

			var encryptCommand = new RootCommand(action)
			{
				new Option("-secret") {Argument = new Argument<string>()},
				new Option("-text") {Argument = new Argument<string>()}
			};


			encryptCommand.Handler = CommandHandler.Create<string, string>(Encrypt);

			await encryptCommand.InvokeAsync(args);
		}


		public static void Encrypt(string secret, string text)
		{
			var cipher = CipherService.Encrypt(text, secret);

			Console.WriteLine($"The cipher is: {cipher}");
		}
	}
}
