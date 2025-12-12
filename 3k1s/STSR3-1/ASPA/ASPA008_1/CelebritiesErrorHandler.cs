namespace ASPA008_1
{
    public class CelebritiesErrorHandler
    {
        private readonly RequestDelegate _next;
        private readonly string _prefix;
        public CelebritiesErrorHandler(RequestDelegate next, string prefix = "Celebrities")
        {
            this._next = next;
            this._prefix = prefix;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await this._next(context);
            }
            catch (Exception ex)
            {
                context.Response.StatusCode = 500;
                await context.Response.WriteAsync($"{this._prefix}:{ex.ToString()}");
            }
        }
    }
}
