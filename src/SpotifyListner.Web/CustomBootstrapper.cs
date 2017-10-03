﻿using Nancy;
using Nancy.Bootstrapper;
using Nancy.Conventions;
using Nancy.TinyIoc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpotifyListner.Web
{
    public class CustomBootstrapper : DefaultNancyBootstrapper
    {
        protected override void ApplicationStartup(TinyIoCContainer container, IPipelines pipelines)
        {
            base.ApplicationStartup(container, pipelines);
            pipelines.AfterRequest += (ctx) =>
            {
                ctx.Response.Headers.Add("Access-Control-Allow-Origin", "*");
                ctx.Response.Headers.Add("Access-Control-Allow-Headers", "Origin, X-Requested-With, Content-Type, Accept");
            };
        }

        protected override void ConfigureConventions(NancyConventions nancyConventions)
        {
            nancyConventions.StaticContentsConventions.Add(StaticContentConventionBuilder.AddDirectory("/", "Content"));
            base.ConfigureConventions(nancyConventions);
        }
        protected override void RequestStartup(TinyIoCContainer container, IPipelines pipelines, NancyContext context)
        {
            //context.Response.WithHeader("Access-Control-Allow-Origin", "*")
            //    .WithHeader("Access-Control-Allow-Methods", "POST,GET")
            //    .WithHeader("Access-Control-Allow-Headers", "Accept, Origin, Content-type");
            //CORS Enable
            //pipelines.AfterRequest.AddItemToEndOfPipeline((ctx) =>
            //{
            //    ctx.Response.WithHeader("Access-Control-Allow-Origin", "*")
            //                .WithHeader("Access-Control-Allow-Methods", "POST,GET")
            //                .WithHeader("Access-Control-Allow-Headers", "Accept, Origin, Content-type");
            //});
            base.RequestStartup(container,pipelines,context);
        }
    }
}
