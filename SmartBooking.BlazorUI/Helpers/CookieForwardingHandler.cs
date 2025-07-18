namespace SmartBooking.BlazorUI.Helpers;

public class CookieForwardingHandler : DelegatingHandler
{
    private readonly IHttpContextAccessor _ctx;
    public CookieForwardingHandler(IHttpContextAccessor ctx) => _ctx = ctx;

    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var cookieHeader = _ctx.HttpContext?.Request.Headers["Cookie"].ToString();
        if (!string.IsNullOrEmpty(cookieHeader))
            request.Headers.Add("Cookie", cookieHeader);

        return base.SendAsync(request, cancellationToken);
    }
}