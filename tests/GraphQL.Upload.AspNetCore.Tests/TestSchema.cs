using GraphQL.Types;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Linq;

namespace GraphQL.Upload.AspNetCore.Tests
{
    public class TestSchema : Schema
    {
        public TestSchema()
        {
            Query = new Query();
            Mutation = new Mutation();
        }
    }

    public sealed class Query : ObjectGraphType
    {
        public Query()
        {
            Field<NonNullGraphType<BooleanGraphType>>("dummy")
                .Resolve(x => true);
        }
    }

    public sealed class Mutation : ObjectGraphType
    {
        public Mutation()
        {
            Field<NonNullGraphType<StringGraphType>>("singleUpload")
                .Argument<UploadGraphType>("file")
                .Resolve(context =>
                {
                    var file = context.GetArgument<IFormFile>("file");
                    return file.FileName;
                });

            Field<NonNullGraphType<StringGraphType>>("multipleUpload")
                .Argument<ListGraphType<UploadGraphType>>("files")
                .Resolve(context =>
                {
                    var files = context.GetArgument<List<IFormFile>>("files");
                    return string.Join(",", files.Select(file => file.FileName));
                });
        }

    }
}
