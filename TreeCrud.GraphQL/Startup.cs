using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GraphQL;
using GraphQL.Http;
using GraphQL.Types;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TreeCrud.DataLayer.Data;
using TreeCrud.DataLayer.Models;
using TreeCrud.DataLayer.Models.GraphQL;

namespace TreeCrud.GraphQL
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddDbContext<AligaContext>(opt =>
                opt.UseSqlite(Configuration.GetConnectionString("Sqlite"))
            );

            services.AddScoped<IDependencyResolver>(s => new FuncDependencyResolver(s.GetRequiredService));

            services.AddScoped<IDocumentExecuter, DocumentExecuter>();
            services.AddSingleton<IDocumentWriter, DocumentWriter>();


            services.AddScoped<UnitatQuery>();
            services.AddTransient<UnitatType>();
            services.AddScoped<ISchema, UnitatSchema>();

            services.AddTransient<IUnitatsRepository, UnitatsRepository>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            // Afegir les dades de prova
            InitializeAsync(app.ApplicationServices).Wait();

            // app.UseHttpsRedirection();
            app.UseMvc();
        }

        /// <summary>
        /// Emplenar amb les dades de prova
        /// </summary>
        public static async Task InitializeAsync(IServiceProvider service)
        {
            using (var serviceScope = service.CreateScope())
            {
                var scopeServiceProvider = serviceScope.ServiceProvider;
                var db = scopeServiceProvider.GetService<AligaContext>();
                db.Database.Migrate();
                await InsertTestData(db);
            }
        }

        private static async Task InsertTestData(AligaContext context)
        {
            if (context.Unitats.Any())
                return;
            List<Unitat> nodes = new List<Unitat> {
                new Unitat { Id = 1, ParentId = null, Label = "Unitat 0", Description = "Hooa", Type = "Unitat" },
                new Unitat { Id = 2, ParentId = null, Label = "Unitat 1", Description = "Hooa 2", Type = "Unitat" },
                new Unitat { Id = 3, ParentId = null, Label = "Unitat 2", Description = "Hooa 2", Type = "Unitat" },
                new Unitat { Id = 4, ParentId = null, Label = "Unitat 3", Description = "Hooa 2", Type = "Unitat" },
            };

            var counter = nodes.Count + 1;
            List<int> nodesWithChildren = new List<int>();
            while (counter < 1000)
            {
                var rng1 = new Random();
                int elementAt = rng1.Next(0, nodes.Count());
                int ParentId = nodes.ElementAt(elementAt).Id;
                if (!nodesWithChildren.Contains(ParentId))
                {
                    nodesWithChildren.Add(ParentId);
                    var rng = new Random();
                    int notesToApped = rng.Next(0, 5);
                    IEnumerable<Unitat> children = Enumerable
                                            .Range(0, notesToApped)
                                            .Select(x =>
                                                   new Unitat
                                                   {
                                                       Id = counter + x,
                                                       ParentId = ParentId,
                                                       Label = nodes
                                                                           .Where(y => y.Id == ParentId)
                                                                           .Select(y => y.Label)
                                                                           .FirstOrDefault() + x.ToString(),
                                                       Description = "Hooa",
                                                       Type = "Unitat"
                                                   }
                                                    );
                    nodes = nodes.Concat(children).ToList<Unitat>();
                    counter += notesToApped;
                }
            }

            context.AddRange(nodes);
            await context.SaveChangesAsync();
        }
    }
}
