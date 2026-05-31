using System.Globalization;
using Axpo.CodingChallenge.Models;
using CsvHelper;
using CsvHelper.Configuration;

namespace Axpo.CodingChallenge.Services;

public class DataWriter : IDataWriter
{
    public async Task WriteDataAsync(IEnumerable<AggregatedTrade> data)
    {
        await using var fileWriter = new StreamWriter("output.csv");
        await using var csvWriter = new CsvWriter(fileWriter, CultureInfo.InvariantCulture);
        await csvWriter.WriteRecordsAsync(data);
    }
}

public sealed class AggregatedTradeMap : ClassMap<AggregatedTrade>
{
    public AggregatedTradeMap()
    {        
        Map(m => m.LocalTime);
        Map(m => m.Volume);
    }
}