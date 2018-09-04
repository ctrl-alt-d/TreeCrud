using System.Collections.Generic;
using GraphQL.Types;
using TreeCrud.DataLayer.Data;

namespace TreeCrud.DataLayer.Models.GraphQL
{
    public class UnitatQuery : ObjectGraphType
    {
        public UnitatQuery(IUnitatsRepository repo)
        {
            Field<UnitatType>(
                "node",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<IntGraphType>> { Name = "id", Description = "Node id" }
                ),
                resolve: context => repo.GetNodeAsync(context.GetArgument<int>("id")).Result
            );

            Field<ListGraphType<UnitatType>>(
                "nodes",
                resolve: context => repo.GetRootAsync().Result
            );

        }
    }
}