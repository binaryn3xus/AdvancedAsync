using System.Reflection;

namespace AdvancedAsync.API.Endpoints.Internal
{
    public static class EndpointsExtensions
    {
        public static void AddEndpoints<TMarker>(this IServiceCollection services)
        {
            AddEndpoints(services, typeof(TMarker));
        }

        public static void AddEndpoints(this IServiceCollection services, Type typeMarker)
        {
            var endpointTypes = GetEndPointTypesFromAssemblyContaining(typeMarker);

            foreach (var endpointType in endpointTypes)
            {
                endpointType.GetMethod(nameof(IEndpoints.AddServices))!.Invoke(null, new object[] { services });
            }
        }

        public static void UseEndpoints<TMarker>(this IApplicationBuilder app)
        {
            UseEndpoints(app, typeof(TMarker));
        }

        public static void UseEndpoints(this IApplicationBuilder app, Type typeMarker)
        {
            var endpointTypes = GetEndPointTypesFromAssemblyContaining(typeMarker);

            foreach (var endpointType in endpointTypes)
            {
                endpointType.GetMethod(nameof(IEndpoints.DefineEndpoints))!.Invoke(null, new object[] { app });
            }
        }

        private static IEnumerable<TypeInfo> GetEndPointTypesFromAssemblyContaining(Type typeMarker)
        {
            return typeMarker.Assembly.DefinedTypes
                        .Where(x => !x.IsAbstract && !x.IsInterface &&
                        typeof(IEndpoints).IsAssignableFrom(x));
        }
    }
}
