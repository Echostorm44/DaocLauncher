using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace DaocLauncher.Helpers;

public class Debouncer
{
    private List<CancellationTokenSource> StepperCancelTokens = new List<CancellationTokenSource>();
    private int MillisecondsToWait;
    private readonly object MyLock = new object(); // Use a locking object to prevent the debouncer triggering again while the action is still running

    public Debouncer(int millisecondsToWait = 300)
    {
        this.MillisecondsToWait = millisecondsToWait;
    }

    public void Debounce(Action funkyFunction)
    {
        CancelAllStepperTokens(); // Cancel all previous requests;
        var cancelToken = new CancellationTokenSource();
        lock(MyLock)
        {
            StepperCancelTokens.Add(cancelToken);
        }
        Task.Delay(MillisecondsToWait, cancelToken.Token).ContinueWith(task =>
        {
            if(!cancelToken.IsCancellationRequested) // if it hasn't been cancelled
            {
                CancelAllStepperTokens(); // Cancel any that remain (there shouldn't be any)
                StepperCancelTokens = new List<CancellationTokenSource>();
                lock(MyLock)
                {
                    funkyFunction();
                }
            }
        }, TaskScheduler.FromCurrentSynchronizationContext());
    }

    private void CancelAllStepperTokens()
    {
        foreach(var token in StepperCancelTokens)
        {
            if(!token.IsCancellationRequested)
            {
                token.Cancel();
            }
        }
    }
}
