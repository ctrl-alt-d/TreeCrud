using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TreeCrud.DataLayer.Models;
using GraphQL;
using GraphQL.Types;
using Microsoft.AspNetCore.Mvc;
using GraphQL.Http;
using System.Net.Http;
using GraphQL.Validation.Complexity;
using GraphQL.Instrumentation;
using TreeCrud.DataLayer.Data;

namespace TreeCrud.GraphQL.Controllers
{


    [Route("api/[controller]")]
    [ApiController]
    public class NodesController : Controller
    {

        private readonly IDocumentExecuter _documentExecuter;
        private readonly ISchema _schema;
        private readonly IDocumentWriter _writer;
        private readonly IDictionary<string, string> _namedQueries;

        private readonly IUnitatsRepository _repository;

        public NodesController(IDocumentExecuter documentExecuter,
                               IDocumentWriter writer,
                               ISchema schema,
                               IUnitatsRepository repo)
        {
            _documentExecuter = documentExecuter;
            _writer = writer;
            _schema = schema;
            _repository = repo;
        }

        [HttpGet]
        public Task<IActionResult> GetAsync(HttpRequestMessage request)
        {
            return Post(new GraphQLQuery { Query = "query foo { node }", Variables = null });
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody]GraphQLQuery query)
        {
            if (query == null) { throw new ArgumentNullException(nameof(query)); }

            var inputs = query.Variables.ToInputs();
            var queryToExecute = query.Query;

            try
            {
                var result = await _documentExecuter.ExecuteAsync(_ =>
                {
                    _.Schema = _schema;
                    _.Query = queryToExecute;
                    _.OperationName = query.OperationName;
                    _.Inputs = inputs;

                    _.ComplexityConfiguration = new ComplexityConfiguration { MaxDepth = 15 };
                    _.FieldMiddleware.Use<InstrumentFieldsMiddleware>();

                }).ConfigureAwait(false);

                if (result.Errors?.Count > 0)
                {
                    return BadRequest(result);
                }

                var json = _writer.Write(result);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }

        }

        [HttpPost]
        [Route("add")]
        public async Task<IActionResult> Add([FromBody]Unitat value)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _repository.Add(value);
            return Ok("Created");
        }
    }

    public class GraphQLQuery
    {
        public string OperationName { get; set; }
        public string NamedQuery { get; set; }
        public string Query { get; set; }
        public Newtonsoft.Json.Linq.JObject Variables { get; set; }
    }
}
