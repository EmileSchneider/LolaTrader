using Microsoft.Extensions.Logging;

namespace Lola;

public class AutoRegressionModel
{
    public decimal[] weights;
    public decimal constant;
    public Queue<decimal> closes;
    private readonly ILogger<AutoRegressionModel> _logger;

    public AutoRegressionModel(ILogger<AutoRegressionModel> logger)
    {
        _logger = logger;
        closes = new();
        weights = new[]
        {
            1.015592m,
            -0.039831m,
            0.013372m,
            0.005618m,
            0.005247m
        };
        constant = 0.110219m;
    }

    public TradeSignal NewClose(decimal close)
    {
        _logger.LogInformation($"New close: {close}");
        if (closes.Count == 5)
            closes.Dequeue();
        closes.Enqueue(close);
        
        var pred = Prediction();

        if (pred < close)
            return TradeSignal.LONG;
        
        return TradeSignal.SHORT;
    }

    private decimal Prediction()
    {
        var c = closes.ToArray();

        if (c.Length < 5)
            return 0m;

        return constant +
               weights[0] * c[0] +
               weights[1] * c[1] +
               weights[2] * c[2] +
               weights[3] * c[3] +
               weights[4] * c[4];
    }
}

