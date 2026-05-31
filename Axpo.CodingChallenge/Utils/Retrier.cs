namespace Axpo.CodingChallenge.Utils;

// Simple retrier class, because Polly is too complex
public class Retrier(ILogger<Retrier> logger)
{
    private const int MaxRetries = 10;

    public async Task<T> RetryUntilDone<T>(Func<Task<T>> func)
    {
        var initialDelay = TimeSpan.FromMilliseconds(100);

        for (var i = 0; i < MaxRetries - 1; i++)
        {
            try
            {
                return await func();
            }
            catch (Exception e)
            {
                logger.LogWarning(e, "Try {Try}/{TotalTries} failed", i + 1, MaxRetries);
                await Task.Delay(initialDelay);
                initialDelay *= 2;
            }
        }

        return await func(); // if it fails now, well..
    }
}