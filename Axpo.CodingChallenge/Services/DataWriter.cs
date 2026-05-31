using System.Globalization;
using Axpo.CodingChallenge.Configuration;
using Axpo.CodingChallenge.Models;
using Axpo.CodingChallenge.Utils;
using CsvHelper;
using Microsoft.Extensions.Options;

namespace Axpo.CodingChallenge.Services;

public class DataWriter(IOptions<WriterOptions> options) : IDataWriter
{
    public async Task WriteDataAsync(IEnumerable<AggregatedTrade> data)
    {
        var timestamp = TimeZoneInfo
            .ConvertTimeBySystemTimeZoneId(DateTimeOffset.Now, TimeUtils.LocalTimeZone)
            .ToString("yyyyMMdd_HHmm");
        var fileName = $"PowerPosition_{timestamp}.csv";
        string fullPath;
        if (string.IsNullOrWhiteSpace(options.Value.OutputDirectory))
        {
            fullPath = fileName;
        }
        else
        {
            Directory.CreateDirectory(options.Value.OutputDirectory);
            fullPath = Path.Combine(options.Value.OutputDirectory ?? string.Empty, fileName);
        }

        await using var fileWriter = new StreamWriter(fullPath);
        await using var csvWriter = new CsvWriter(fileWriter, CultureInfo.InvariantCulture);
        await csvWriter.WriteRecordsAsync(data);
    }
}
