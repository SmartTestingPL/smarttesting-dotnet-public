using System.Collections.Generic;
using NodaTime;
using ProductionCode.Orders;

namespace ProductionCode.Db;

/// <summary>
/// Interfejs służący do komunikacji z relacyjną bazą danych.
/// Posłuży nam do przykładów zastosowania mocków i weryfikacji interakcji.
/// </summary>
public interface IPostgresAccessor
{
  void UpdatePromotionStatistics(string promotionName);

  void UpdatePromotionDiscount(string promotionName, decimal newDiscount);

  IList<Promotion> GetValidPromotionsForDate(LocalDate localDate);
}