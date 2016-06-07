using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleTranslatorTest
{
    class Program
    {
        static SimpleTranslate.SimpleTranslate.SimpleTranslator tr;

        static void Main(string[] args)
        {
            Console.WriteLine("Starting up");
            Console.WriteLine("Creating obj");
            tr = new SimpleTranslate.SimpleTranslate.SimpleTranslator("ImageBots", "zhUKtlKJ+pbw6u/4p82jKsXMsPcF5EF5+zXfZMS7+wY=");
            Console.WriteLine("Translating");
            Translate();
            Console.ReadKey();
        }

        static async Task Translate()
        {
            try
            {
                Console.WriteLine("Fn Invoked");
                var res = await tr.Translate("Hello", "en", "ru");
                Console.WriteLine("Result is {0}", res);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: {0}", e.ToString());
            }
        }
    }
}
