﻿using k8s;

namespace AdvancedAsync.API.Endpoints;

public class KubeEndpoints : IEndpoints
{
    public static void AddServices(IServiceCollection services) { }

    public static void DefineEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet("api/kube/pods", GetPods).WithName("Get Kubernetes Pods").WithOpenApi();
    }

    internal static IResult GetPods([FromServices] ISchedulerFactory schedulerFactory, CancellationToken cancellationToken = default)
    {
        var config = KubernetesClientConfiguration.InClusterConfig();
        var client = new Kubernetes(config);

        var namespaces = client.CoreV1.ListNamespace();
        var itemList = new List<string>();
        foreach (var ns in namespaces.Items)
        {
            Console.WriteLine(ns.Metadata.Name);
            var list = client.CoreV1.ListNamespacedPod(ns.Metadata.Name);
            foreach (var item in list.Items)
            {
                Console.WriteLine(item.Metadata.Name);
                itemList.Add($"{ns.Metadata.Name}:{item.Metadata.Name}");
            }
        }

        return Results.Ok(itemList);
    }
}
