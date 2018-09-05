using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using GraphQL.Common.Request;
using GraphQL.Client;
using Newtonsoft.Json;

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

        public Task<TreeNode[]> GetNodesToAsync(int Id)
        {

            List<TreeNode> aux_nodes = new List<TreeNode>();
            TreeNode auxNode = new TreeNode();
            auxNode.ParentId = Id;
            do
            {
                auxNode = nodes.Where(x => x.Id == auxNode.ParentId).FirstOrDefault();
                if (auxNode != null)
                {
                    aux_nodes.Add(auxNode);
                }
            } while (auxNode != null && auxNode.ParentId != null);
            aux_nodes.Reverse();
            return Task.FromResult(aux_nodes.ToArray());
        }

        public Task<TreeNode> GetNodeAsync(int Id)
        {


            TreeNode aux_node = nodes.Where(x => x.Id == Id).FirstOrDefault();

            return Task.FromResult(aux_node);
        }

        public Task<TreeNode> AddUnitatAsync(TreeNode unitat)
        {
            unitat.Id = nodes.Select(X => X.Id).Max() + 1;
            nodes.Add(unitat);
            return Task.FromResult(unitat);
        }
    }
}
