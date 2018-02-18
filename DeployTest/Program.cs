#define filmfetch
//#define actorfetch
//#define dmmfetchtest

//#define downloadimg

//#define downloadimg2

//#define runscript

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
#if downloadimg2
            using (var filmContext = new FilmContext()) {
                var download = true;
                var interval = 1000;
                var queryable = filmContext.Actors
                    .Where(actor => actor.ShotTag == null)
                    .Take(interval);
                Task.Run(() => {
                    while (download && queryable.Any()) {
                        var actors = queryable.ToList();
                        var leave = 0;
                        var concurrentBag = new ConcurrentBag<Actor>(actors);
                        for (var i = 0; i < 8; i++) {
                            Task.Run(() => {
                                try {
                                    using (var webClient = new WebClient()) {
                                        while (concurrentBag.TryTake(out Actor actor)) {
                                            if (actor.ShotUrl!=null) {
                                                var lastIndexOf = actor.ShotUrl.LastIndexOf("/");
                                                var name = actor.ShotUrl.Substring(lastIndexOf,actor.ShotUrl.Length-lastIndexOf);
                                                webClient.DownloadFile(actor.ShotUrl,
                                                    $"{Actor.ShotDirName}\\{name}");
                                                actor.ShotTag = name;
                                            }
                                            else {
                                                actor.ShotTag = Actor.UndefinedShotName;

                                            }
                                            
                                        }
                                    }
                                }
                                catch (Exception exception) {
                                    Console.WriteLine(exception.Message);
                                }
                                finally {
                                    Interlocked.Increment(ref leave);
                                }
                            });
                        }
                        while (leave < 4) {
                            Thread.Sleep(500);
                        }
                        filmContext.SaveChanges();
                        Console.WriteLine("loop end,next....");
                    }
                });
                Console.ReadLine();
                download = false;
                Console.WriteLine("waiting end");
                Console.ReadLine();
            }  
#endif

#if downloadimg
            using (var filmContext = new FilmContext()) {
                bool download = true;
                int interval = 1000;
                var queryable = filmContext.Films
                    .Where(film => film.ShotTag == null)
                    .Take(interval);
                Task.Run(() => {
                    while (download && queryable.Any()) {
                        var films = queryable.ToList();
                        int leave = 0;
                        var concurrentBag = new ConcurrentBag<Film>(films);
                        for (int i = 0; i < 8; i++) {
                            Task.Run(() => {
                                try {
                                    using (var webClient = new WebClient()) {
                                        while (concurrentBag.TryTake(out Film film)) {
                                            webClient.DownloadFile(film.ShotUrl,
                                                $"{Film.ShotDirPath}\\{film.Code}.jpeg");
                                            film.ShotTag = film.Code;
                                        }
                                    }
                                }
                                finally {
                                    Interlocked.Increment(ref leave);
                                }
                            });
                        }
                        while (leave < 4) {
                            Thread.Sleep(500);
                        }
                        filmContext.SaveChanges();
                        Console.WriteLine("loop end,next....");
                    }
                    Console.WriteLine("all ended!");
                });
                Console.ReadLine();
                download = false;
                Console.WriteLine("waiting end");
                Console.ReadLine();
            }
#endif

#if filmfetch
            using (var filmContext = new FilmContext()) {
                var entityFrameworkPersistence = new EntityFrameworkPersistence(filmContext);
                var dmmReptile = new DmmReptile(entityFrameworkPersistence);
                dmmReptile.FilmsGrabAsync(() => { Console.WriteLine("end"); });
                Console.WriteLine("startting......");
                Console.ReadLine();
                dmmReptile.Dispose();
            }
#endif


#if runscript


            using (var filmContext = new FilmContext()) {
                //filmContext.Actors.Add(new Actor() {
                //    Name = "测试名称",
                //    LastUpdateTime = DateTime.Now
                //});
                //filmContext.SaveChanges();
                filmContext.Configuration.AutoDetectChangesEnabled = false;
                foreach (var film in filmContext.Films) {
                    film.IsOneActor = film.Actors.Count == 1;
                }
                filmContext.Configuration.AutoDetectChangesEnabled = true;
                //var queryable = filmContext.Films.Where(actor => actor.IsOneActor);
                //Console.WriteLine(queryable.Count());
                filmContext.SaveChanges();

                //                filmContext.Configuration.AutoDetectChangesEnabled = false;
                //                foreach (var filmContextActor in filmContext.Actors) {
                //                    filmContextActor.FilmSearchUrl =
                //                        $"http://www.dmm.co.jp/mono/dvd/-/actress/=/keyword={filmContextActor.Code}/";
                //                }
                //                filmContext.Configuration.AutoDetectChangesEnabled = true;
                //                filmContext.SaveChanges();
                Console.WriteLine("done!");
                Console.ReadLine();
            }

#endif

#if actorfetch

      
            
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
                        var regex =
new Regex(@"<li><a href=""(/mono/dvd/-/list/=/article=actress/id=[0-9]+?/)""><img src=""(http://pics.dmm.co.jp/mono/actjpgs/medium/\w+?.jpg)"" alt="""" width=""100"" height=""100""><br>(\S+?)</a></li>");
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