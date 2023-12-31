﻿namespace BiyLineApi.Entities;
public sealed class ProductVariationEntity
{
    public int Id { get; set; }
    public string? Color { get; set; }
    public string? Size { get; set; }
    public int? Quantity { get; set; }

    public int ProductId { get; set; }
    public ProductEntity Product { get; set; }
    public ICollection<ContractOrderVariationEntity> ContractOrderVariations { get; set; } = new List<ContractOrderVariationEntity>();
}