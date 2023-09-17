using Core.Scoring.Credit;
using Core.Scoring.domain;

namespace BikService.Credit;

public class RabbitCreditInfoListener : ICreditInfoListener
{
  private readonly ILogger<RabbitCreditInfoListener> _log;
  private readonly ICreditInfoRepository _repository;

  public RabbitCreditInfoListener(ICreditInfoRepository repository, ILogger<RabbitCreditInfoListener> log)
  {
    _repository = repository;
    _log = log;
  }

  public async Task StoreCreditInfo(Pesel pesel, CreditInfo? creditInfo)
  {
    await _repository.SaveCreditInfo(pesel, creditInfo);
  }

  public async Task OnMessage(CreditInfoDocument creditInfoDocument)
  {
    _log.LogInformation($"Got message from credit info queue [{creditInfoDocument}]");
    await StoreCreditInfo(creditInfoDocument.Pesel, creditInfoDocument.CreditInfo);
  }
}