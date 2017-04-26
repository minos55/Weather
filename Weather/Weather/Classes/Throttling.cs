using System;
using System.Threading.Tasks;

namespace Nomnio.Weather
{
    public class Throttler
    {
        private readonly TimeSpan maxPeriod;
        private readonly System.Threading.SemaphoreSlim throttleActions;
        private readonly System.Threading.SemaphoreSlim throttlePeriods;

        public Throttler(int maxActions, TimeSpan _maxPeriod)
        {
            throttleActions = new System.Threading.SemaphoreSlim(maxActions, maxActions);
            throttlePeriods = new System.Threading.SemaphoreSlim(maxActions, maxActions);
            maxPeriod = _maxPeriod;
        }

        public Task<OpenWeatherMapServices> Queue<OpenWeatherMapServices>(Func<string, OpenWeatherMapServices> action,string arg)
        {
            return throttleActions.WaitAsync().ContinueWith<OpenWeatherMapServices>(t =>
            {
                try
                {
                    throttlePeriods.Wait();

                    // Release after period
                    // - Allow bursts up to maxActions requests at once
                    // - Do not allow more than maxActions requests per period
                    Task.Delay(maxPeriod).ContinueWith((tt) =>
                    {
                        throttlePeriods.Release(1);
                    });

                    return action(arg);
                }
                finally
                {
                    throttleActions.Release(1);
                }
            });
        }
    }
}
