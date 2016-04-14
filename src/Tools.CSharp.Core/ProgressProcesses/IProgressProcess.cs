using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using Tools.CSharp.StateMachines;

namespace Tools.CSharp.ProgressProcesses
{
    public interface IProgressProcess : IStateble<ProgressProcessState>, IDisposable
    {
        //---------------------------------------------------------------------
        string Title { get; }
        string Description { get; }
        //---------------------------------------------------------------------
        int MaxProgressValue { get; }
        int ProgressPercentage { get; }
        //---------------------------------------------------------------------
        bool IsDisposed { get; }
        //---------------------------------------------------------------------
        ProgressProcessPriority Priority { get; }
        bool IsStartLoadAvailable { get; }
        //---------------------------------------------------------------------
        Task StartLoadAsync(CancellationToken cancellationToken);
        //---------------------------------------------------------------------
        event EventHandler TitleChanged;
        event EventHandler DescriptionChanged;
        event EventHandler MaxProgressValueChanged;
        event ProgressChangedEventHandler ProgressChanged;
        //---------------------------------------------------------------------
    }
}
