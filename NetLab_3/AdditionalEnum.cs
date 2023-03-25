using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PL
{
    public class AE
    {
        public enum userTypes { none, customer, worker, admin }
        public static Dictionary<userTypes, string> dicStats = new Dictionary<userTypes, string>
        {
            { userTypes.customer, "customer" },        
            { userTypes.worker, "worker" },             
            { userTypes.admin, "admin"}                 

        };

        public enum uC { LogIn, Reg, Exit, MakeOrder, ProductList, MyOrders, ListOfWhOrders, ListOfUsersOrders, DeliveryProductToWarehouse, DeliveryProductToUser, AddProduct }
        public static Dictionary<uC, string> dicCom = new Dictionary<uC, string>
        {
            { uC.LogIn, "1" },
            { uC.Reg, "2" },
            { uC.Exit, "q" },
            { uC.MakeOrder, "1" },
            { uC.ProductList, "2" },
            { uC.MyOrders, "3" },
            { uC.AddProduct, "4" },
            { uC.ListOfWhOrders, "1" },
            { uC.ListOfUsersOrders, "2" },
            { uC.DeliveryProductToWarehouse, "3" },
            { uC.DeliveryProductToUser, "4" }

        };

        public enum nC { LogIn, Reg, Exit }
        public static Dictionary<nC, string> dicNCom = new Dictionary<nC, string>
        {
            { nC.LogIn, "1" },
            { nC.Reg, "2" },
            { nC.Exit, "q" }
        };

        public enum aC { Exit, ListOfWhOrders, ListOfUsersOrders, DeliveryProductToWarehouse, DeliveryProductToUser }
        public static Dictionary<aC, string> dicACom = new Dictionary<aC, string>
        {
            { aC.Exit, "q" },
            { aC.ListOfWhOrders, "1" },
            { aC.ListOfUsersOrders, "2" },
            { aC.DeliveryProductToWarehouse, "3" },
            { aC.DeliveryProductToUser, "4" }

        };

        public enum cC { Exit, MakeOrder, ProductList, MyOrders, AddProduct}
        public static Dictionary<cC, string> dicCCom = new Dictionary<cC, string>
        {

            { cC.Exit, "q" },
            { cC.MakeOrder, "1" },
            { cC.ProductList, "2" },
            { cC.MyOrders, "3" },
            { cC.AddProduct, "4" }

        };
    }
}
