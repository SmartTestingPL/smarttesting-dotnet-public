using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Flurl.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace WebApplicationTests;

/// <summary>
/// Klasa implementująca prymitywny mechanizm generowania
/// dokumentacji na podstawie przepływu komunikacji HTTP.
/// </summary>
public class Doc : IAsyncDisposable
{
  private readonly string _docName;
  private readonly List<ExchangeItem> _exchange = new();
  private readonly string _description;

  public Doc(string docName, string description)
  {
    _docName = docName;
    _description = description;
  }

  /// <summary>
  /// Metoda dokumentująca żądanie od testu
  /// </summary>
  /// <param name="call">żądanie HTTP</param>
  public async Task DocumentRequest(FlurlCall call)
  {
    var text = new StringBuilder();
    text.AppendLine($"{call.HttpRequestMessage.Method} {call.HttpRequestMessage.RequestUri.LocalPath}");
    text.AppendLine($"Host: {call.HttpRequestMessage.RequestUri.Host}");
    if (call.HttpRequestMessage.Content != null)
    {
      AppendHeadersTo(text, call.HttpRequestMessage.Content.Headers);
    }

    AppendHeadersTo(text, call.HttpRequestMessage.Headers);
    text.AppendLine();
    if (call.HttpRequestMessage.Content != null)
    {
      text.Append(await FormatBody(call.HttpRequestMessage.Content));
    }

    _exchange.Add(ExchangeItem.Request(text.ToString()));
  }

  private static async Task<string> FormatBody(HttpContent content)
  {
    var body = await content.ReadAsStringAsync();
    if (content.Headers.ContentType != null &&
        content.Headers.ContentType.ToString().Contains("application/json"))
    {
      return JToken.Parse(body).ToString(Formatting.Indented);
    }
    else
    {
      return body;
    }
  }

  /// <summary>
  /// Metoda dokumentująca odpowiedź od kodu produkcyjnego
  /// </summary>
  /// <param name="call">żądanie HTTP</param>
  public async Task DocumentResponse(FlurlCall call)
  {
    var text = new StringBuilder();
    text.AppendLine($"HTTP/1.1 {(int) call.Response.StatusCode} {call.Response.StatusCode}");
    if (call.HttpResponseMessage.Content != null)
    {
      AppendHeadersTo(text, call.HttpResponseMessage.Content.Headers);
    }

    AppendHeadersTo(text, call.HttpResponseMessage.Headers);
    text.AppendLine();
    if (call.HttpResponseMessage.Content != null)
    {
      text.Append(await FormatBody(call.HttpResponseMessage.Content));
    }

    _exchange.Add(ExchangeItem.Response(text.ToString()));
  }

  /// <summary>
  /// Metoda przypinająca dokument do zdarzeń klienta HTTP
  /// </summary>
  public static void Connect(FlurlClient client, Doc doc)
  {
    //Przed żądaniem udokumentujemy żądanie
    client.Settings.BeforeCallAsync += doc.DocumentRequest;

    //Po żądaniu udokumentujemy odpowiedź
    client.Settings.AfterCallAsync += doc.DocumentResponse;
  }

  /// <summary>
  /// Przy sprzątaniu po obiekcie zapisujemy wygenerowaną zawartość
  /// do pliku w formacie markdown.
  /// </summary>
  /// <returns></returns>
  public ValueTask DisposeAsync()
  {
    var content = new StringBuilder();

    // generowanie nazwy i opisu scenariusza
    content
      .AppendLine("# " + _docName)
      .AppendLine()
      .AppendLine(_description)
      .AppendLine()
      .AppendLine("## Snippet");

    // generowanie opisu przepływu komunikacji HTTP
    foreach (var requestOrResponse in _exchange)
    {
      content
        .AppendLine()
        .AppendLine("### " + requestOrResponse.Type)
        .AppendLine()
        .AppendLine("```http")
        .AppendLine(requestOrResponse.Content)
        .AppendLine("```")
        .AppendLine(requestOrResponse.Divider);
    }

    // zapis dokumentu do pliku
    return new ValueTask(
      File.WriteAllTextAsync($"{_docName}.md", content.ToString()));
  }

  /// <summary>
  /// Dołączenie do tekstu sformatowanych nagłówków HTTP
  /// </summary>
  /// <param name="text"></param>
  /// <param name="headers"></param>
  private static void AppendHeadersTo(StringBuilder text, HttpHeaders headers)
  {
    foreach (var (key, values) in headers)
    {
      foreach (var headerValue in values)
      {
        text.AppendLine($"{key}: {headerValue}");
      }
    }
  }

  private class ExchangeItem
  {
    public string Divider { get; }

    public static ExchangeItem Request(string content)
    {
      return new ExchangeItem("Request", content, "");
    }

    public static ExchangeItem Response(string content)
    {
      return new ExchangeItem("Response", content, "\r\n___\r\n");
    }

    public readonly string Type;
    public readonly string Content;

    private ExchangeItem(string type, string content, string divider)
    {
      Divider = divider;
      Type = type;
      Content = content;
    }
  }
}