using Microsoft.AspNetCore.StaticFiles;

namespace UI.AnchorCalculator.Extensions;

public class RemovePathBaseStaticFilesMiddleware
{
	private readonly RequestDelegate m_Next;
	private readonly StaticFileMiddleware m_StaticFileMiddleware;

	public RemovePathBaseStaticFilesMiddleware(RequestDelegate next, StaticFileMiddleware staticFileMiddleware)
	{
		m_Next = next;
		m_StaticFileMiddleware = staticFileMiddleware;
	}

	public async Task InvokeAsync(HttpContext context)
	{
		var originalPathBase = context.Request.PathBase;

		if (!string.IsNullOrEmpty(originalPathBase))
			context.Request.PathBase = PathString.Empty;

		// Check if current request is a ling to a static file
		var contentTypeProvider = new FileExtensionContentTypeProvider();
		var isStaticFile = contentTypeProvider.TryGetContentType(context.Request.Path, out var _);

		if (isStaticFile)
		{
			// Removing base path from from generated link
			var originalPath = context.Request.Path;
			context.Request.Path = originalPath.Value.Replace(originalPathBase, PathString.Empty);

			// Rewinding original values for base path and request path
			context.Request.PathBase = originalPathBase;
			context.Request.Path = originalPath;
		}
		await m_StaticFileMiddleware.Invoke(context);
		await m_Next(context);
	}
}
