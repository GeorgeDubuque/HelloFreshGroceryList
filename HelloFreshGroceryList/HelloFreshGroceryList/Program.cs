using System;
using System.Net.Http;
using System.Threading.Tasks;
using HtmlAgilityPack;
using System.Collections.Generic;
using HtmlAgilityPack;
using ScrapySharp.Extensions;
using ScrapySharp.Network;
using System.Threading;
using System.Text;
using System.IO;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace HelloFreshGroceryList
{
    class Program
    {
        public class Ingredient
        {
            public string name;
            public string denom;
            public double amount = 0;
        }
        static void Main(string[] args)
        {

            Dictionary<string, Ingredient> ingredients = new Dictionary<string, Ingredient>();

            Console.WriteLine("Enter Filepath: ");
            string filePath = Console.ReadLine();

            var urls = File.ReadAllLines(filePath);

            foreach(var url in urls) 
            {
                ScrapingBrowser _browser = new ScrapingBrowser();
                _browser.Encoding = Encoding.UTF8;
                WebPage page = _browser.NavigateToPage(new Uri(url));
                HtmlNode root = page.Html;
                string rootString = root.InnerHtml;
                var nodes = root.SelectNodes("//div[@class='fela-_1qz307e']");
                foreach(var ingredientNode in nodes)
                {
                    var measurementIngredientNodes = ingredientNode.SelectNodes("p");
                    var measurementDenom = measurementIngredientNodes[0].InnerText.Split(" ");
                    var amountString = measurementDenom[0];
                    var denomString = "";
                    if(measurementDenom.Length > 1)
                    {
                        denomString = measurementDenom[1];
                    }
                    var ingredientText = measurementIngredientNodes[1].InnerText;

                    double amount = Program.ConvertUnicodeStringToDecimal(amountString);

                    if (ingredients.ContainsKey(ingredientText))
                    {
                        ingredients[ingredientText].amount += amount;
                    }
                    else
                    {
                        Ingredient ing = new Ingredient
                        {
                            name = ingredientText,
                            denom = denomString,
                            amount = amount
                        };
                        ingredients.Add(ingredientText, ing);
                    }
                }
            }

            foreach(var ingredient in ingredients.Values)
            {
                Console.WriteLine(ingredient.name + " " + ingredient.amount + " " + ingredient.denom);
            }
        }

        public static double ConvertUnicodeStringToDecimal(string amountString)
        {
            string parsedString = "";
            for(int i = 0; i < amountString.Length; i++)
            {
                char currChar = amountString[i];
                if(currChar != '.')
                {
                    parsedString += CharUnicodeInfo.GetNumericValue(currChar);
                }
                else
                {
                    parsedString += currChar;
                }
            }

            if(parsedString.Length > 0)
            {
                return double.Parse(parsedString);
            }
            else
            {
                return 0;
            }

        }

    }
}
