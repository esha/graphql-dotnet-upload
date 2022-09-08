using GraphQL;
using GraphQL.Types;
using GraphQL.Upload.AspNetCore;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FileUploadSample
{
    public class SampleSchema : Schema
    {
        public SampleSchema(IServiceProvider provider)
            : base(provider)
        {
            Query = provider.GetRequiredService<Query>();
            Mutation = provider.GetRequiredService<Mutation>();
        }
    }

    public sealed class Query : ObjectGraphType
    {
        public Query(UploadRepository uploads)
        {
            Field<ListGraphType<FileGraphType>>("uploads")
                .Resolve(ctx => uploads.Files);
        }
    }

    public sealed class Mutation : ObjectGraphType
    {
        public Mutation(UploadRepository uploads)
        {
            Field<FileGraphType>("singleUpload")
                .Argument<UploadGraphType>("file")
                .ResolveAsync(async context =>
                {
                    var file = context.GetArgument<IFormFile>("file");
                    return await uploads.Save(file);
                });

            Field<ListGraphType<FileGraphType>>("multipleUpload")
                .Argument<ListGraphType<UploadGraphType>>("files")
                .ResolveAsync(async context =>
                {
                    var files = context.GetArgument<IEnumerable<IFormFile>>("files");
                    return await Task.WhenAll(files.Select(file => uploads.Save(file)));
                });
        }
    }

    public class File
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string MimeType { get; set; }
        public string Path { get; set; }
    }

    public sealed class FileGraphType : ObjectGraphType<File>
    {
        public FileGraphType()
        {
            Field(f => f.Id).Name("id");
            Field(f => f.Name).Name("filename");
            Field(f => f.MimeType).Name("mimetype");
            Field(f => f.Path).Name("path");
        }
    }
}
