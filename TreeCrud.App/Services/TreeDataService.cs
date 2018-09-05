using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using GraphQL.Common.Request;
using GraphQL.Client;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;

namespace TreeCrud.App.Services
{
    public class TreeDataService
    {

        List<TreeNode> nodes = new List<TreeNode>();



        public async Task<TreeNode[]> GetNodesAsync(int? ParentId = null)
        {
            GraphQLRequest request;

            if (ParentId == null)
            {
                request = new GraphQLRequest
                {
                    Query = @"{
                              nodes {
                                  id,
                                  parentId,
                                  label,
                               }
                            }"
                };
            }
            else
            {
                request = new GraphQLRequest
                {
                    Query = @"
                        {
                        node(id:" + ParentId + @") {
                            id,
                            parentId,
                            label,
                                children {
                                    id,
                                    parentId,
                                    label
                                }
                            }
                        }"
                };
            }

            var graphQLClient = new GraphQLClient("http://localhost:6000/api/nodes");
            var graphQLResponse = await graphQLClient.PostAsync(request);

            TreeNode[] aux_nodes;

            if (ParentId == null)
            {
                var x = graphQLResponse.Data.nodes;
                System.Console.WriteLine(x);
                aux_nodes = x.ToObject<TreeNode[]>();
            }
            else
            {
                var x = graphQLResponse.Data.node.children;
                System.Console.WriteLine(x);
                aux_nodes = x.ToObject<TreeNode[]>();
            }

            return aux_nodes;

        }

        public async Task<TreeNode[]> GetNodesToAsync(int Id)
        {

            List<TreeNode> aux_nodes = new List<TreeNode>();
            TreeNode auxNode = new TreeNode();
            auxNode.ParentId = Id;
            do
            {
                auxNode = await GetNodeAsync(auxNode.ParentId.Value);
                if (auxNode != null)
                {
                    aux_nodes.Add(auxNode);
                }
            } while (auxNode != null && auxNode.ParentId != null);
            aux_nodes.Reverse();
            return aux_nodes.ToArray();
        }

        public Task<TreeNode> GetNodeAsync(int Id)
        {
            TreeNode aux_node = null;

            GraphQLRequest request = new GraphQLRequest
            {
                Query = @"
                        {
                          node(id:" + Id + @") {
                            id,
                            parentId,
                            label,
                            date
                            }
                        }"
            };

            var graphQLClient = new GraphQLClient("http://localhost:6000/api/nodes");
            System.Console.WriteLine($"-- to POST {Id}");
            try
            {
                var graphQLResponse = graphQLClient.PostAsync(request).GetAwaiter().GetResult();
                //var graphQLResponse = graphQLClient.Post(request);
                var x = graphQLResponse.Data.node;
                System.Console.WriteLine(x);
                aux_node = x.ToObject<TreeNode>();
            }
            catch (Exception e)
            {
                System.Console.WriteLine(e);
            }

            System.Console.WriteLine("-- after POST");

            return Task.FromResult(aux_node);
        }

        public async Task<TreeNode> AddUnitatAsync(TreeNode unitat)
        {
            TreeNode unitat_aux = null;
            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.PostAsync("http://localhost:6000/api/nodes/add", TreeNode2Json(unitat));

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                unitat_aux = JsonConvert.DeserializeObject<TreeNode>(json);
            }
            return unitat_aux;

        }

        public static StringContent TreeNode2Json(TreeNode tree)
        {
            var content = JsonConvert.SerializeObject(tree);
            var stringContent = new StringContent(content, Encoding.UTF8, "application/json");
            return stringContent;
        }
    }
}
