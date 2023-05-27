namespace AdvancedAsync.API.Endpoints.Internal
{
    public interface IEndpoints
    {
        public static abstract void AddServices(IServiceCollection services);
        public static abstract void DefineEndpoints(IEndpointRouteBuilder app);
    }
}
