﻿namespace Examples
{
	using System;
	using System.IO;
	using SmartyStreets;
	using SmartyStreets.USZipCodeApi;

	public class USZipCodeMultipleLookupsExample
	{
		public static void Run()
		{
			var authId = Environment.GetEnvironmentVariable("SMARTY_AUTH_ID");
			var authToken = Environment.GetEnvironmentVariable("SMARTY_AUTH_TOKEN");
			var client = new ClientBuilder(authId, authToken).Build();

			var lookup1 = new Lookup
			{
				ZipCode = "12345"
			};

			var lookup2 = new Lookup
			{
				City = "Phoenix",
				State = "Arizona"
			};

			var lookup3 = new Lookup("cupertino", "CA", "95014"); // You can also set these with arguments

			var batch = new Batch();

			try
			{
				batch.Add(lookup1);
				batch.Add(lookup2);
				batch.Add(lookup3);

				client.Send(batch);
			}
			catch (BatchFullException)
			{
				Console.WriteLine("Error. The batch is already full.");
			}
			catch (SmartyException ex)
			{
				Console.WriteLine(ex.Message);
				Console.WriteLine(ex.StackTrace);
			}
			catch (IOException ex)
			{
				Console.WriteLine(ex.StackTrace);
			}

			for (var i = 0; i < batch.Count; i++)
			{
				var result = batch[i].Result;
				Console.WriteLine("Lookup " + i + ":\n");

				if (result.Status != null)
				{
					Console.WriteLine("Status: " + result.Status);
					Console.WriteLine("Reason: " + result.Reason);
					continue;
				}

				var cityStates = result.CityStates;
				Console.WriteLine(cityStates.Length + " City and State match" + ((cityStates.Length == 1) ? ":" : "es:"));

				foreach (var cityState in cityStates)
				{
					Console.WriteLine("City: " + cityState.City);
					Console.WriteLine("State: " + cityState.State);
					Console.WriteLine("Mailable City: " + cityState.MailableCity);
					Console.WriteLine();
				}

				var zipCodes = result.ZipCodes;
				Console.WriteLine(zipCodes.Length + " ZIP Code match" + ((cityStates.Length == 1) ? ":" : "es:"));

				foreach (var zipCode in zipCodes)
				{
					Console.WriteLine("ZIP Code: " + zipCode.ZipCode);
					Console.WriteLine("County: " + zipCode.CountyName);
					Console.WriteLine("Latitude: " + zipCode.Latitude);
					Console.WriteLine("Longitude: " + zipCode.Longitude);
					Console.WriteLine();
				}
				Console.WriteLine("***********************************");
			}
		}
	}
}