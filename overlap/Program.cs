using R54IN0;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace overlap
{
    public class Program
    {
        public static void Do1<T>() where T : class
        {
            var db = LexDb.GetDbInstance();
            var iofmts = db.LoadAll<IOStockFormat>();
            var customers = db.LoadAll<T>();
            ILookup<string, IField> customerLookup = customers.ToLookup(x => ((IField)x).Name, y => (IField)y);

            string path = typeof(T).Name + "ID";

            foreach (var customer in customerLookup)
            {
                string name = customer.Key;
                var list = customer.ToList();
                if (list.Count() == 1)
                    continue;

                Console.WriteLine("중복된 이름: " + name);

                string id = list.First().ID;
                list.RemoveAt(0);
                foreach (var fmt in iofmts)
                {
                    if (list.Any(x => x.ID == fmt.GetType().GetProperty(path).GetValue(fmt) as string))
                        fmt.GetType().GetProperty(path).SetValue(fmt, id);
                }
                list.ForEach(x => db.DeleteByKey<T>(x.ID));
            }
            db.Save(iofmts);
        }

        public static void Do2<T>() where T : class
        {
            var db = LexDb.GetDbInstance();
            var iofmts = db.LoadAll<InventoryFormat>();
            var customers = db.LoadAll<T>();
            ILookup<string, IField> customerLookup = customers.ToLookup(x => ((IField)x).Name, y => (IField)y);

            string path = typeof(T).Name + "ID";

            foreach (var customer in customerLookup)
            {
                string name = customer.Key;
                var list = customer.ToList();
                if (list.Count() == 1)
                    continue;

                Console.WriteLine("중복된 이름: " + name);

                string id = list.First().ID;
                list.RemoveAt(0);
                foreach (var fmt in iofmts)
                {
                    if (list.Any(x => x.ID == fmt.GetType().GetProperty(path).GetValue(fmt) as string))
                        fmt.GetType().GetProperty(path).SetValue(fmt, id);
                }
                list.ForEach(x => db.DeleteByKey<T>(x.ID));
            }
            db.Save(iofmts);
        }

        static void Main(string[] args)
        {
            Console.WriteLine("시작");
            Do1<Customer>();
            Do1<Employee>();
            Do1<Product>();
            Do1<Project>();
            Do1<Supplier>();
            Do1<Warehouse>();

            Do2<Maker>();
            Do2<Measure>();
            Console.WriteLine("완료");
            Console.ReadKey();
        }
    }
}
