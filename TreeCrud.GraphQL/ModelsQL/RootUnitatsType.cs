using GraphQL.Types;
using TreeCrud.DataLayer.Data;

namespace TreeCrud.DataLayer.Models.GraphQL
{
    public class RootUnitatsType : ObjectGraphType<UnitatList>
    {
        public RootUnitatsType()
        {
            Field<ListGraphType<UnitatType>>("node");
        }
    }
}
