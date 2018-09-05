using System;
using GraphQL;
using GraphQL.Types;

namespace TreeCrud.DataLayer.Models.GraphQL
{
    public class UnitatSchema : Schema
    {
        public UnitatSchema(IDependencyResolver resolver)
            : base(resolver)
        {
            Query = resolver.Resolve<UnitatQuery>();
        }
    }
}