//#define filmfetch
//#define actorfetch
//#define dmmfetchtest
#define runscript
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Threading.Tasks;
using Castle.Components.DictionaryAdapter;
using Castle.Windsor;
using DeployTest.Service;
using FORCEBuild.Message.Base;
using XSystem.Core.Infrastructure;
using Reptile;
using XSystem.Core.Domain;
using Xunit;

namespace DeployTest
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            //using (var windsorContainer = new WindsorContainer())
            //{
            //    windsorContainer.Install(new DomainInstaller());
            //    var resolve = windsorContainer.Resolve<Deploy>();
            //    // resolve.RelationChange();
            //    var series = resolve.Load();
            //    Console.WriteLine(series);
            //}
#if filmfetch
           
#endif


#if runscript

//            using (var filmContext = new FilmContext()) {
//                var queryable = filmContext.Actors.Where(actor => actor.FetchRegion=="DMM");
//                Console.WriteLine(queryable.Count());
////                filmContext.Configuration.AutoDetectChangesEnabled = false;
////                foreach (var filmContextActor in filmContext.Actors) {
////                    filmContextActor.FilmSearchUrl =
////                        $"http://www.dmm.co.jp/mono/dvd/-/actress/=/keyword={filmContextActor.Code}/";
////                }
////                filmContext.Configuration.AutoDetectChangesEnabled = true;
////                filmContext.SaveChanges();
//                Console.WriteLine("done!");
//                Console.ReadLine();
//            }

#endif

#if actorfetch

            ActorsGrab();
            
#endif


#if dmmfetchtest

            var webRequest = WebRequest.Create("http://www.dmm.co.jp/mono/dvd/-/actress/=/keyword=wa/");
            using (var webResponse = webRequest.GetResponse())
            {
                using (var responseStream = webResponse.GetResponseStream())
                {
                    using (var streamReader = new StreamReader(responseStream))
                    {
                        var readToEnd = streamReader.ReadToEnd();
                        var regex = new Regex(@"<li><a href=""(/mono/dvd/-/list/=/article=actress/id=[0-9]+?/)""><img src=""(http://pics.dmm.co.jp/mono/actjpgs/medium/\w+?.jpg)"" alt="""" width=""100"" height=""100""><br>(\S+?)</a></li>");
                        var value = regex.Matches(readToEnd);
                        //.Cast<Match>()
                        //.Max(match => int.Parse(match.Groups[1].Value));

                        Console.WriteLine(value.Count);
                    }
                }
            }

#endif

        }

       
    }


}
