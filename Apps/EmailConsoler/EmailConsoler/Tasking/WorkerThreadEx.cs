using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Threading;

namespace EmailConsoler.Tasking
{   
    /// <summary>
    /// Encapsulates a .NET Thread object and manages the WaitHandles associated with controlling its behavior
    /// </summary>
    public class WorkerThreadEx
    {
        private ManualResetEvent abortEvent;
        private ManualResetEvent signalEvent;
        private Thread thread;

        private Action<WorkerThreadEx> _performTask = null;
        private int _performInterval = 0;

        public WorkerThreadEx(string workerName, Action<WorkerThreadEx>  performTask = null, int performInterval = 0)
        {
            if (string.IsNullOrEmpty(workerName))
                workerName = "WorkerThreadEx";

            abortEvent = new ManualResetEvent(false);
            signalEvent = new ManualResetEvent(false);

            thread = new Thread(ThreadProc);
            thread.Name = workerName + "_" + thread.ManagedThreadId;

            _performTask = performTask;
            _performInterval = performInterval;
        }

        public void Start()
        {
            thread.Start();
        }

        public void Abort()
        {
            abortEvent.Set();

            thread.Join();
        }

        /// <summary>
        /// Clears the signaling WaitHandle, causing the thread to pause after completing its current
        /// iteration
        /// </summary>
        public void Pause()
        {
            signalEvent.Reset();
        }

        /// <summary>
        /// Sets the signaling WaitHandle, causing it to resume operation (if paused)
        /// </summary>
        public void Signal()
        {
            signalEvent.Set();
        }

        public bool IsSignaled
        {
            get { return signalEvent.WaitOne(0); }
        }


        public string Name
        {
            get {
                return thread.Name;
            }
        }

        /// <summary>
        /// The overall thread method, consisting of an infinite loop that exits upon signaling the abort event
        /// </summary>
        private void ThreadProc()
        {
            WaitHandle[] handles = new WaitHandle[] { signalEvent, abortEvent };

            while (true)
            {
                switch (WaitHandle.WaitAny(handles))
                {
                    case 0: // signal
                        {
                            PerformTask();
                        }
                        break;
                    case 1: // abort
                        {
                            return;
                        }
                }
            }
        }

        private void PerformTask()
        {
            if (_performTask != null)
            {
                _performTask(this);
                if (_performInterval > 0)
                    Thread.Sleep(_performInterval);
            }
        }
    }
}
