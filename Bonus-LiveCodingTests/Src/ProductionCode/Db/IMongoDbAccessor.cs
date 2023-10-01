namespace ProductionCode.Db;

/// <summary>
/// Interfejs służący do komunikacji z dokumentową bazą danych.
/// Posłuży nam do przykładów zastosowania stubów.
/// </summary>
public interface IMongoDbAccessor
{
  decimal GetPromotionDiscount(string promotionName);
  decimal GetMinCommission();
}