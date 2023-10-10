using System.Text;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Net.Http.Headers;

namespace ApiHole.Formatters;

public class PlaintextMediaTypeFormatter : TextInputFormatter
{
    private static readonly Type StringType = typeof(string);

    public PlaintextMediaTypeFormatter()
    {
        SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse("text/plain"));
        SupportedEncodings.Add(Encoding.UTF8);
        //SupportedEncodings.Add(Encoding.Unicode);
    }

    public override async Task<InputFormatterResult> ReadRequestBodyAsync(InputFormatterContext context, Encoding effectiveEncoding)
    {
        var httpContext = context.HttpContext;

        using var reader = new StreamReader(httpContext.Request.Body, effectiveEncoding);

        return await InputFormatterResult.SuccessAsync(await reader.ReadToEndAsync());
    }
}
