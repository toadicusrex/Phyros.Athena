#define Default // or Limits

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Phyros.Athena.Composition;

namespace Phyros.Athena
{
	public class Startup
	{
		public void Configure(IApplicationBuilder app)
		{
			var serverAddressesFeature = app.ServerFeatures.Get<IServerAddressesFeature>();
			app.UseStaticFiles();

			ApplicationComposer.Compose();
			var engineManager = ApplicationComposer.GetEngineManager();
			engineManager.Start();
			
			app.Run(async (context) =>
			{
				context.Response.ContentType = "text/html";
				await context.Response
									.WriteAsync("<!DOCTYPE html><html lang=\"en\"><head>" +
											"<title></title></head><body><p>Hosted by Kestrel</p>");

				await context.Response.WriteAsync("<p>Workflow Engine Started.");

				if (serverAddressesFeature != null)
				{
					await context.Response
										.WriteAsync("<p>Listening on the following addresses: " +
												string.Join(", ", serverAddressesFeature.Addresses) +
												"</p>");
				}

				await context.Response.WriteAsync("<p>Request URL: " +
									$"{context.Request.GetDisplayUrl()}<p>");

				await context.Response.WriteAsync("</body></html>");
			});
		}
	}
}