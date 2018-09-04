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

        public NodesController(IDocumentExecuter documentExecuter,
                               IDocumentWriter writer,
                               ISchema schema)
        {
            _documentExecuter = documentExecuter;
            _writer = writer;
            _schema = schema;
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

                //var httpResult = result.Errors?.Count > 0
                //    ? HttpStatusCode.BadRequest
                //    : HttpStatusCode.OK;

                var json = _writer.Write(result);

                return Ok(result);
                //            var response = request.CreateResponse(httpResult);
                //            response.Content = new StringContent(json, Encoding.UTF8, "application/json");
                //            return response;
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }

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
