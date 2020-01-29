using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ktc.cif
{
	/// <summary>
	/// Main program
	/// </summary>
	public class Program
	{
		private const string KeyId = "CIF_SECRET";

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
							new Option("-text", "The text to encrypt.") {Argument = new Argument<string>()},
							new Option("-file", "The text file path containing a list of strings to encrypt. Each line will be encrypted separately.") {Argument = new Argument<string>()}
						};

							encryptCommand.Handler = CommandHandler.Create<string, string, string>(Encrypt);

							await encryptCommand.InvokeAsync(args);

							command.Add(encryptCommand);
							break;
						}
					case "decrypt":
						{
							var decryptCommand = new Command("decrypt")
						{
							new Option("-secret", "The secret required to for decryption."){Argument = new Argument<string>()},
							new Option("-cipher", "The cipher to decrypt to text.") {Argument = new Argument<string>()},
							new Option("-file", "The text file path containing a list of cipher to decrypt. Each line will be decrypted separately.") {Argument = new Argument<string>()}
						};

							decryptCommand.Handler = CommandHandler.Create<string, string, string>(Decrypt);

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

		public static void Encrypt(string secret, string text, string file)
		{
			var passPhrase = secret ?? Environment.GetEnvironmentVariable("cif-secret", EnvironmentVariableTarget.User);

			if (passPhrase != null)
			{
				if (text != null)
				{
					var cipher = CipherService.Encrypt(text, passPhrase);
					Console.WriteLine($"The cipher is: {cipher}");
				}
				else if (file != null)
				{
					Console.WriteLine("The ciphers are:");
					foreach (var line in File.ReadLines(file, Encoding.UTF8))
					{
						var cipher = CipherService.Encrypt(line, passPhrase);
						Console.WriteLine($"{cipher}");
					}
				}
				else
				{
					Console.WriteLine("Text to be encrypted was not provided.");
				}
			}
			else
			{
				Console.WriteLine("Something went wrong while encrypting the text. Secret not provided or configured.....");
			}
		}

		public static void Decrypt(string secret, string cipher, string file)
		{
			try
			{
				var passPhrase = secret ?? Environment.GetEnvironmentVariable("cif-secret", EnvironmentVariableTarget.User);

				if (passPhrase != null)
				{
					if (cipher != null)
					{
						var text = CipherService.Decrypt(cipher, passPhrase);
						Console.WriteLine($"The text is: {text}");
					}
					else if (file != null)
					{
						Console.WriteLine("The ciphers are:");
						foreach (var line in File.ReadLines(file, Encoding.UTF8))
						{
							var text = CipherService.Decrypt(line, passPhrase);
							Console.WriteLine($"{text}");
						}
					}
					else
					{
						Console.WriteLine("Text to be decrypted was not provided.");
					}
				}
				else
				{
					Console.WriteLine("Something went wrong while encrypting the text. Secret not provided or configured.....");
				}
			}
			catch (CryptographicException)
			{
				Console.WriteLine("Something went wrong while decrypting the cipher....");
			}
		}

		public static void Set(string secret)
		{

			Environment.SetEnvironmentVariable(KeyId, secret, EnvironmentVariableTarget.User);

			Console.WriteLine($"Secret configured. - Key is  {Environment.GetEnvironmentVariable(KeyId, EnvironmentVariableTarget.User)}");
		}

		public static void Get()
		{
			Console.WriteLine($"Secret Key is  {Environment.GetEnvironmentVariable(KeyId, EnvironmentVariableTarget.User)}");
		}
	}
}
