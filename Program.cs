using Microsoft.Owin.Hosting;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KatanaIntro
{
    using System.IO;
    using System.Web.Http;
    using AppFunc = Func<IDictionary<string, object>, Task>;


    class Program
    {
        static void Main(string[] args)
        {
            string uri = "http://localhost:8080";

            using (WebApp.Start<StartUp>(uri))
            {
                Console.WriteLine("Started");
                Console.WriteLine("Press any key to quit");
                Console.ReadKey();
                Console.WriteLine("Ending");
            }
        }
    }

    public class StartUp
    {
        public void Configuration(IAppBuilder appBuilder)
        {
            //middleware registration


            appBuilder.Use(async (env, next) =>
            {
                Console.WriteLine("Requesting: " + env.Request.Path);

                await next();

                Console.WriteLine("Response: " + env.Response.StatusCode);
            });

            ConfigureWenApi(appBuilder);

            appBuilder.Use<HelloWorldComponent>();

        }

        private void ConfigureWenApi(IAppBuilder appBuilder)
        {
            var config = new HttpConfiguration();
            config.Routes.MapHttpRoute("Default",
                                        "api/{controller}/{id}",
                                        new { id = RouteParameter.Optional });
            appBuilder.UseWebApi(config);
        }
    }

    public class HelloWorldComponent
    {
        AppFunc _next;

        public HelloWorldComponent(AppFunc next) 
        {
            _next = next;
        }

        public  Task Invoke(IDictionary<string, object> enviroment)
        {
            Stream response = enviroment["owin.ResponseBody"] as Stream;
            using (StreamWriter sw = new StreamWriter(response))
            {
                return sw.WriteAsync("Hello!!!");
            }
        }
    }
}
