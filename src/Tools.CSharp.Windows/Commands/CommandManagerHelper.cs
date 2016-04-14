using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace Tools.CSharp.Windows.Commands
{
    internal static class CommandManagerHelper
    {
        //---------------------------------------------------------------------
        public static void CallWeakReferenceHandlers(List<WeakReference> handlers)
        {
            if (handlers != null)
            {
                var callees = new EventHandler[handlers.Count];
                var count = 0;

                for(var handlerCounter = handlers.Count - 1; handlerCounter >= 0; handlerCounter--)
                {
                    var reference = handlers[handlerCounter];
                    var handler = reference.Target as EventHandler;
                    if (handler == null)
                        handlers.RemoveAt(handlerCounter);
                    else
                    {
                        callees[count] = handler;
                        count++;
                    }
                }

                for(var calleeCounter = 0; calleeCounter < count; calleeCounter++)
                {
                    var handler = callees[calleeCounter];
                    handler(null, EventArgs.Empty);
                }
            }
        }
        //---------------------------------------------------------------------
        public static void AddHandlersToRequerySuggested(List<WeakReference> handlers)
        {
            if (handlers != null)
            {
                foreach(var handlerReference in handlers)
                {
                    var handler = handlerReference.Target as EventHandler;
                    if (handler != null)
                    { CommandManager.RequerySuggested += handler; }
                }
            }
        }
        public static void RemoveHandlersFromRequerySuggested(List<WeakReference> handlers)
        {
            if (handlers != null)
            {
                foreach(var handlerReference in handlers)
                {
                    var handler = handlerReference.Target as EventHandler;
                    if (handler != null)
                    { CommandManager.RequerySuggested -= handler; }
                }
            }
        }
        //---------------------------------------------------------------------
        public static void AddWeakReferenceHandler(ref List<WeakReference> handlers, EventHandler handler, int defaultListSize)
        {
            if (handlers == null)
            { handlers = (defaultListSize > 0) ? new List<WeakReference>(defaultListSize) : new List<WeakReference>(); }

            handlers.Add(new WeakReference(handler));
        }
        public static void AddWeakReferenceHandler(ref List<WeakReference> handlers, EventHandler handler)
        {
            AddWeakReferenceHandler(ref handlers, handler, -1);
        }
        public static void RemoveWeakRefenceHandler(List<WeakReference> handlers, EventHandler handler)
        {
            if (handlers != null)
            {
                for(var handlerCounter = handlers.Count - 1; handlerCounter >= 0; handlerCounter--)
                {
                    var refence = handlers[handlerCounter];
                    var existingHandler = refence.Target as EventHandler;

                    if ((existingHandler == null) || (existingHandler == handler))
                    { handlers.RemoveAt(handlerCounter); }
                }
            }
        }
        //---------------------------------------------------------------------
    }
}