using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using Test_Aplicacion_Consola_C_.Net_Framework.Data;
using System.Runtime.ConstrainedExecution;
using Core.Dtos;
using System.Data.Common;
using System.Data.SqlClient;
using System.Data;
using System.Reflection;
using System.Runtime.CompilerServices;
using static Test_Aplicacion_Consola_C_.Net_Framework.Data.ExtensionMethod;
using System.Security.Claims;
using LanguageExt.ClassInstances;
using System.ComponentModel;
//using MailKit.Net.Pop3;
using S22.Imap;
using LanguageExt;
using OpenPop.Pop3;
using System.Web.UI.WebControls.WebParts;

namespace Test_Aplicacion_Consola_C_.Net_Framework
{

    internal class Program
    {
        
        public static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public static List<int> newList = new List<int>();
        static void Main(string[] args)
        {
            Console.WriteLine("ññññññ ÑÑÑÑÑ");
            Console.ReadLine();
            return;
            double dou = 2.23;
            decimal dec = 2.23m;
            int innt = 2;
            UInt16 uin16 = 23;
            Console.WriteLine(dou.IsNumericType());
            Console.WriteLine(dec.IsNumericType());
            Console.WriteLine(uin16.IsNumericType());
            Console.ReadLine();
            return;

            Prueba6();
            return;


            List<int> list = new List<int>() { 1, 2, 3, 4 };

            list.ForEach(async x =>
            {
                Console.WriteLine($"x = {x}");
                var watch = System.Diagnostics.Stopwatch.StartNew();
                MetodoAsync(x);
                watch.Stop();
                var elapsedMs = watch.ElapsedMilliseconds;
                Console.WriteLine($" {x} takes {elapsedMs} ms");
            });
            newList.ForEach(d => Console.WriteLine($"order {d}"));
            Console.ReadLine();
            return;

            var tc = new TestClassRecibe();
            var tc2 = new TestClassOrigen() { Name = "Pueba c2", ListInt = new List<int> { 2, 3, 4 }, SubClass = new SubTestClassOrigen() { SubName = "Test sub name origen" } };
            tc = tc2.CastTo<TestClassRecibe>();
            //tc.SubClass.SubName
            using (SqlConnection connection = new SqlConnection("Data Source=LPT080066\\SQLEXPRESS;Initial Catalog=ENDESADTC_PORTUGAL;User ID=loseguera;Password=@p455w0rd;TrustServerCertificate=true"))
            {
                using (SqlCommand cmd = connection.CreateCommand())//_db.Database.GetDbConnection().CreateCommand()
                {

                    bool wasOpen = cmd.Connection?.State == ConnectionState.Open;
                    if (!wasOpen) cmd.Connection?.Open();
                    try
                    {
                        cmd.CommandText = $"EXEC SP_Cargaleads";

                        using (DbDataReader reader = cmd.ExecuteReader())
                        {
                            //DbDataReader reader = cmd.ExecuteReader();
                            var res = reader.ConvertToDto<List<CargaLeadsDto>>();

                            reader.Close();
                        }
                    }
                    finally
                    {
                        if (!wasOpen) cmd.Connection?.Close();
                    }
                }
            }



            TestClassRecibe r = new TestClassRecibe() { Name = "Prueba" };

            foreach (var property in r.GetType().GetProperties())
            {
                var ptype = property.PropertyType;
                string customPropertyValue = string.Empty;
                object[] attribute = property.GetCustomAttributes(typeof(DbReaderMapAttribute), true);
                if (attribute.Length > 0)
                {
                    DbReaderMapAttribute myAttribute = (DbReaderMapAttribute)attribute[0];
                    customPropertyValue = myAttribute.Name;
                }
                Console.WriteLine("La propiedad [{0}] tiene el valor [{1}], tiene el custom name [{2}]", property.Name, property.GetValue(r, null), customPropertyValue);
            }


            var p = r.GetType().GetProperties().FirstOrDefault(xs => validateP(xs, "ValueXsp"));
            //PropertyInfo propertyInfo = typeof(TestClass).GetProperty("Name");
            //object[] attribute = propertyInfo.GetCustomAttributes(typeof(MyCustomAttribute), true);
            //if (attribute.Length > 0)
            //{
            //    MyCustomAttribute myAttribute = (MyCustomAttribute)attribute[0];
            //    string propertyValue = myAttribute.SomeProperty;
            //}

            //.GetProperty().GetValue(car, null);
            int val = 5;
            int factor = 2;
            Console.WriteLine("El valor multiplicado de {0} por {1} es {2}", val, factor, val.MultiplyBy(factor));
            Console.ReadLine();
        }
        public static bool validateP(PropertyInfo xs, string cpName)
        {
            object[] attribute = xs.GetCustomAttributes(typeof(DbReaderMapAttribute), true);
            if (attribute.Length > 0)
            {
                DbReaderMapAttribute myAttribute = (DbReaderMapAttribute)attribute[0];
                return myAttribute.Name == cpName;
            }
            else
            {
                return xs.Name == cpName;
            }
        }

        private static bool MetodoAsync(int valor)
        {
            if (valor == 2) {
                for (int i = 0; i < 1000000; i++) {
                    valor = (valor + 1) % 2;
                    valor += valor * 2;
                    for (int y = 0; y < 1000; y++)
                    {
                        valor = (valor + 1) % 2;
                        valor += valor * y + i;
                    }

                }
            }
            newList.Add(valor);
            return true;
        }


        private static void Prueba1()
        {
            Dictionary<string, object> dictionary = new Dictionary<string, object>
            {
                { "prueba 1", 2 },
                { "prueba 2", "text" }
            };
            foreach (var d in dictionary)
            {

                Console.WriteLine(d.Value.GetType().Name);
            }

        }
        private static void Prueba2()
        {

            TestClassRecibe tc = new TestClassRecibe();
            tc.CastTo<object>();
            var ttc = tc.GetType();
            var x = Activator.CreateInstance(null, "Core.Dtos.CargaLeadsDto");
            var uw = x.Unwrap();
            tc = tc;
            //return Class.forName(className).getConstructor(String.class).newInstance(arg);

            //Class<?> clazz = Class.forName(className);
            //    Constructor <?> ctor = clazz.getConstructor(String.class);
            //    Object object = ctor.newInstance(new Object[] { ctorArgument
            //        });
            Console.ReadLine();
        }

        private static void Prueba3()
        {

            var client = new Pop3Client();
            client.Connect("pop.gmail.com", 995, true);
            client.Authenticate("lenar.oseguera.ext@abaigroup.com", "Madrid1707.");
            var infos = client.GetMessageInfos();// GetMessageCount();
            var messagesIds = infos.Select(mi => mi.Number).ToList().OrderByDescending(min => min);
            OpenPop.Mime.Message message = null;
            foreach(var mnumber in messagesIds)
            {
                

                message = client.GetMessage(mnumber);
                Console.WriteLine($"Mensaje {mnumber} {message.Headers.From.MailAddress.Address} {message.Headers.DateSent.ToString()}");
                if (message.Headers.From.MailAddress.Address == "notificaciones.2mares@abaigroup.com")
                {
                    break;
                }
            }
            if (message != null)
            {
                foreach (var ma in message.FindAllAttachments())
                {
                    Console.WriteLine(ma.FileName.ToString());
                }
            }
            //OpenPop.Mime.Message message = client.GetMessage(count);

            Console.WriteLine(message.FindAllAttachments().First().ContentType);
        }

        private static void Prueba4()
        {
            //using (var client = new Pop3Client())
            //{
            //    client.Connect("pop.gmail.com", 995, true);

            //    // Note: since we don't have an OAuth2 token, disable
            //    // the XOAUTH2 authentication mechanism.
            //    //client.AuthenticationMechanisms.Remove("XOAUTH2");

            //    client.Authenticate("lenar.oseguera.ext@abaigroup.com", "Madrid1707.");

            //    for (int i = 0; i < client.Count; i++)
            //    {
            //        var message = client.GetMessage(i);
            //        Console.WriteLine("Subject: {0}", message.Subject);
            //    }

            //    client.Disconnect(true);
            //}
        }

        private static void Prueba5(){
            using (ImapClient cli = new ImapClient("imap.gmail.com", 993 , true))
                {
                    
                    cli.Login("lenar.oseguera.ext@abaigroup.com", "Madrid1707.", AuthMethod.Auto);
                    

                    string mb = "INBOX";

                    var ids = cli.Search(SearchCondition.All(), mb);
                    ids = ids.OrderByDescending(x => x).ToList();
                    foreach (var id in ids)
                    {
                        var fs = cli.GetMessageFlags(id, mb);
                        if (fs.Contains(MessageFlag.Draft))
                        {
                            continue;
                        }

                        
                        Console.WriteLine(id);
                        Console.WriteLine("\t{0}: {1}", "Flags", string.Join(" ", fs.Select(f => f.ToString())));
                        var m = cli.GetMessage(id, FetchOptions.HeadersOnly, false, mb);
                        
                        Console.WriteLine("\tHeaders:");
                        foreach (string h in m.Headers)
                        {
                            Console.WriteLine("\t\t{0}: {1}", h, m.Headers[h]);
                        }
                        

                        Console.WriteLine("Setting Draft flag on message {0}", id);
                        cli.SetMessageFlags(id, mb, MessageFlag.Draft, MessageFlag.Seen);
                        var fs2 = cli.GetMessageFlags(id, mb);
                        
                            Console.WriteLine("Message flags: {0}", string.Join(" ", fs2.Select(f => f.ToString())));
                            Console.WriteLine();
                        
                    }

                    
                    cli.Logout();
                }

                return ;
       
        }

        private static void Prueba6()
        {
            var imapClient = new ImapClient("imap.gmail.com", 993, true);

            // log in
            imapClient.Login("lenar.oseguera.ext@abaigroup.com", "Madrid1707.", AuthMethod.Auto);

            // search for messages
            var messageIds = imapClient.Search(SearchCondition.All(), "INBOX");

            // no messages found - return empty collection
            if (messageIds == null || messageIds.Count() == 0) return;
            messageIds = messageIds.OrderByDescending(f => f).ToList();
            foreach (var messageId in messageIds)
            {
                var fs = imapClient.GetMessageFlags(messageId, "INBOX");
                if (fs.Contains(MessageFlag.Draft))
                {
                    continue;
                }
                var m = imapClient.GetMessage(messageId, FetchOptions.HeadersOnly, false, "INBOX");
                if (m.From.Address == "notificaciones.2mares@abaigroup.com")
                {
                    int x = 1;
                }
            }
            //// get messages
            //var messages = imapClient.GetMessages(messageIds, FetchOptions.TextOnly, true, searchCriteria.Folder);

            //// check if messages downloaded properly
            //if (messages == null || messages.Count() == 0) return;

            //// create as S22ImapMessages
            //return messages.Select(m => new S22ImapMessage(m));
        }
    }
}
