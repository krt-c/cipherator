using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace ktc.cif
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
				switch (args[0])
				{
					case "set":
						{
							var setCommand = new Command("set")
						{
							new Option("-secret", "The secret required to encryption.") {Required = true, Argument = new Argument<string>()},
						};

							setCommand.Handler = CommandHandler.Create<string>(Set);

							await setCommand.InvokeAsync(args);

							command.Add(setCommand);
							break;
						}
					case "get":
						{
							var getCommand = new Command("get") { Handler = CommandHandler.Create(Get) };

							await getCommand.InvokeAsync(args);

							command.Add(getCommand);
							break;
						}
					case "encrypt":
						{
							var encryptCommand = new Command("encrypt")
						{
							new Option("-secret", "The secret required to encryption.") {Argument = new Argument<string>()},
							new Option("-text", "The text to encrypt.") {Required = true, Argument = new Argument<string>()}
						};

							encryptCommand.Handler = CommandHandler.Create<string, string>(Encrypt);

							await encryptCommand.InvokeAsync(args);

							command.Add(encryptCommand);
							break;
						}
					case "decrypt":
						{
							var decryptCommand = new Command("decrypt")
						{
							new Option("-secret", "The secret required to for decryption."){Argument = new Argument<string>()},
							new Option("-cipher", "The cipher to decrypt to text.") {Required = true, Argument = new Argument<string>()}
						};

							decryptCommand.Handler = CommandHandler.Create<string, string>(Decrypt);

							await decryptCommand.InvokeAsync(args);


							command.Add(decryptCommand);
							break;
						}
					default:
						Console.WriteLine("Invalid sub command was specified. Specify if you want to encrypt or decrypt text or else set or get secret key.");
						break;
				}
			}
			catch (NullReferenceException)
			{
				Console.WriteLine("Invalid sub command was specified. Specify if you want to encrypt or decrypt text or else set or get secret key.");
			}
		}

		public static void Encrypt(string secret, string text)
		{
			if (secret != null)
			{
				var cipher = CipherService.Encrypt(text, secret);
				Console.WriteLine($"The cipher is: {cipher}");
			}
			else
			{
				var s = Environment.GetEnvironmentVariable("cif-secret", EnvironmentVariableTarget.User);
				if (s == null) Console.WriteLine("Something went wrong while encrypting the text. Secret not provided or configured.....");
				else
				{
					var cipher = CipherService.Encrypt(text, s);
					Console.WriteLine($"The cipher is: {cipher}");
				}
			}
		}

		public static void Decrypt(string secret, string cipher)
		{
			try
			{
				if (secret != null)
				{
					var text = CipherService.Decrypt(cipher, secret);
					Console.WriteLine($"The text is: {text}");
				}
				else
				{
					var s = Environment.GetEnvironmentVariable("cif-secret", EnvironmentVariableTarget.User);
					if (s == null)
						Console.WriteLine("Something went wrong while decrypting the cipher. Secret not provided or configured.....");
					else
					{
						var text = CipherService.Decrypt(cipher, s);
						Console.WriteLine($"The text is: {text}");
					}
				}
			}
			catch (CryptographicException)
			{
				Console.WriteLine("Something went wrong while decrypting the cipher....");
			}
		}

		public static void Set(string secret)
		{
			Environment.SetEnvironmentVariable("cif-secret", secret, EnvironmentVariableTarget.User);

			Console.WriteLine($"Secret configured. - Key is  {Environment.GetEnvironmentVariable("cif-secret", EnvironmentVariableTarget.User)}");
		}

		public static void Get()
		{
			Console.WriteLine($"Secret Key is  {Environment.GetEnvironmentVariable("cif-secret", EnvironmentVariableTarget.User)}");
		}
	}
}
