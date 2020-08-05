using System;
using System.Threading.Tasks;


/// <summary>
/// some help methods to make delayed tasks more readable
/// </summary>
public class ActionDelayer
{
    /// <summary>
    /// Runs an Action on a background thread after a delay of f seconds
    /// </summary>
    public static Task RunAfterDelayAsync(float delay, Action toRun)
    {
        return Task.Delay(TimeSpan.FromSeconds(delay)).ContinueWith(previous =>
        {
            try
            {
                toRun();
            }
            catch (Exception e)
            {
                throw e;
            }
        });
    }

    /// <summary>
    /// Runs an Action on the main thread after a delay of f seconds
    /// </summary>
    public static Task RunAfterDelay(float delay, Action toRun)
    {
        return Task.Delay(TimeSpan.FromSeconds(delay)).ContinueWith(previous =>
        {
            try
            {
                toRun();
            }
            catch (Exception e)
            {
                throw e;
            }
                //Make sure this runs on the same thread which requested the delay
            }, TaskScheduler.FromCurrentSynchronizationContext());
    }
}
