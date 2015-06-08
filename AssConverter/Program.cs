using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace AssConverter
{
	class Program
	{
		static void Main(string[] args)
		{
			for (int index = 18; index < 51; index++)
			{
				var file = "";

				if (args.Length == 0)
					file = "C:/Users/Devin/Desktop/stuff/subs/Shirokuma Cafe/[Kamigami] Shirokuma Cafe - " 
						+ (index < 10 ? "0" + index.ToString() : index.ToString())
						+ " [1280x720 x264 AAC Sub(GB,BIG5,JP)_track5_jpn.ass";
				else
					file = args[0];

				var streamReader = File.OpenText(file);

				var fileString = "";

				var fileStringBuilder = new StringBuilder();

				var lookingForDialog = true;

				while (!streamReader.EndOfStream)
				{
					var tmp = streamReader.ReadLine();

					if (lookingForDialog)
					{
						if (tmp.Contains("Dialogue:"))
						{
							lookingForDialog = false;
						}
					}

					if (!lookingForDialog)
						fileStringBuilder.Append(tmp);
				}

				streamReader.Close();

				fileString = fileStringBuilder.ToString();

				var dialogueSplit =
					fileString.Split(
						new string[] { "Dialogue:" },
						StringSplitOptions.RemoveEmptyEntries);

				//9 commas
				fileStringBuilder.Clear();
				for (int i = 0; i < dialogueSplit.Length; i++)
				{
					fileStringBuilder.Append(removeEffects(after9thComma(dialogueSplit[i])));
					fileStringBuilder.Append(System.Environment.NewLine);
					fileStringBuilder.Append(System.Environment.NewLine);
				}

				file = file.Replace(".ass", ".txt");

				File.WriteAllText(file, fileStringBuilder.ToString());
			}
		}

		public static string after9thComma(string str)
		{
			var commaCount = 0;
			for (int i = 0; i < str.Length; i++)
			{
				if (commaCount < 9)
				{
					if (str[i] == ',')
					{
						commaCount++;

					}
				}
				else
				{
					return str.Substring(i);
				}
			}

			return "";
		}

		public static string removeEffects(string str)
		{
			return convertNewLine(removeParanOps(str));
		}

		public static string removeParanOps(string str)
		{
			int paranIndex = 0;
			var foundOpen = false;
			var foundClose = false;

			for (int i = 0; i < str.Length; i++)
			{
				if (!foundOpen)
				{
					if (str[i] == '{')
					{
						foundOpen = true;
						paranIndex = i;
					}
				}
				if (!foundClose)
				{
					if (str[i] == '}')
					{
						foundClose = true;
					}
				}
				else
				{
					if (paranIndex != 0)
						return removeParanOps(str.Substring(0, paranIndex) + str.Substring(i));
					
					return removeParanOps(str.Substring(i));
				}
			}

			return str;
		}

		public static string convertNewLine(string str)
		{
			return str.Replace("\\n", Environment.NewLine).Replace("\\N", Environment.NewLine);
		}
	}
}
