using GraphQL.Types;
using TreeCrud.DataLayer.Data;

namespace TreeCrud.DataLayer.Models.GraphQL
{
    class UnitatType : ObjectGraphType<Unitat>
    {

        public UnitatType(IUnitatsRepository repo)
        {
            Field(x => x.Id).Description("Node id.");
            Field(x => x.ParentId, nullable: true).Description("Parent node.");
            Field(x => x.Date).Description("Creation date");
            Field(x => x.Label).Description("Node label");
            Field(x => x.Description).Description("Node description");
            Field(x => x.Type).Description("Node tipus");

            Field<ListGraphType<UnitatType>>(
                "children",
                resolve: context => repo.GetChildrenAsync(context.Source.Id).Result
            );
        }
    }
}
