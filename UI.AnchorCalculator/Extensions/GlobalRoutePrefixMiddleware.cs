namespace UI.AnchorCalculator.Extensions;

public class GlobalRoutePrefixMiddleware
{
	private readonly RequestDelegate m_Next;
	private readonly string m_RoutePrefix;

	public GlobalRoutePrefixMiddleware(RequestDelegate next, string routePrefix)
	{
		m_Next = next;
		m_RoutePrefix = routePrefix;
	}

	public async Task InvokeAsync(HttpContext context)
	{
		context.Request.PathBase = new PathString(m_RoutePrefix);
		await m_Next(context);
	}
}
