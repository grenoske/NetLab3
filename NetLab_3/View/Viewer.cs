using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PL.AE;


namespace PL.View
{
    public class Viewer
    {
        public static void Show(string str)
        {
            Console.WriteLine(str);
        }
        public static void ShowWelcomeMessage()
        {
            Console.WriteLine("--- Welcome to Warehouse! ---");
        }
        public static void ShowHomeCommandList()
        {
            Console.WriteLine("--- Avaible Commands:  ---");
            foreach (var item in dicNCom)
            {
                Console.WriteLine(item.Value.ToString() + " - " + item.Key.ToString());
            }
            Console.WriteLine("-----------------------");
        }

        public static void ShowLoginForm()
        {
            Console.WriteLine("--- Log in ---");
        }

        public static void ShowRegForm()
        {
            Console.WriteLine("--- Registration ---");
        }

        public static void ShowRegistrationValidate()
        {
            Console.WriteLine("- Registration complete, please choose login command -");
        }
        public static void ShowLogin()
        {
            Console.WriteLine("Your Login: ");
        }

        public static void ShowPassword()
        {
            Console.WriteLine("Your Password: ");
        }

        public static void ShowWelcomeCustomer()
        {
            Console.WriteLine("--- Welcome Customer ---");
        }

        public static void ShowCostumerCommandList()
        {
            Console.WriteLine("--- Avaible Commands:  ---");
            foreach (var item in dicCCom)
            {
                Console.WriteLine(item.Value.ToString() + " - " + item.Key.ToString());
            }
            Console.WriteLine("-----------------------");
        }

        public static void ShowCreateProduct()
        {
            Console.WriteLine("--- Create Product ---");
            Console.WriteLine("--- Provide us information about product that you want to buy ---");
            Console.WriteLine("--- In format: 'Name;Company;Price' ---\n Example: Hammers;BestMetal;300");
        }

        public static void FormatErr()
        {
            Console.WriteLine("Please input in the following format");
        }

        public static void AcceptInfo()
        {
            Console.WriteLine("All correct, info is accepted");
        }

        public static void ShowMakeOrder2()
        {
            Console.WriteLine("--- Provide us more additional information about order ---");
            Console.WriteLine("--- In format: 'ProductId;Quantity;Address' ---\n Example: 1;5;Bulvar30b");
        }

        public static void ShowOrderErr()
        {
            Console.WriteLine("-Order is incorrect!");
        }
        public static void ShowOrderSuc()
        {
            Console.WriteLine("+Order is send to Warehouse");
        }

        public static void ShowProductSuc()
        {
            Console.WriteLine("+Order is send to Warehouse");
        }

        public static void ShowWelcomeAdmin()
        {
            Console.WriteLine("--- Welcome Admin ---");
        }

        public static void ShowAdminCommandList()
        {
            Console.WriteLine("--- Avaible Commands:  ---");
            foreach (var item in dicACom)
            {
                Console.WriteLine(item.Value.ToString() + " - " + item.Key.ToString());
            }
            Console.WriteLine("-----------------------");
        }

        public static void ShowInputUserOrderId()
        {
            Console.WriteLine("-- Input User's Order id ---");
        }

        public static void ShowInputWareHouseOrderId()
        {
            Console.WriteLine("-- Input Warehouse's Order id ---");
        }
    }
}
