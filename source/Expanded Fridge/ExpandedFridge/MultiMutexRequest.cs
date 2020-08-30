using System;
using System.Collections.Generic;

using StardewValley.Network;


namespace ExpandedFridge
{
    /// <summary>
    /// Manages multiple mutex request like StardewValley does internally.
    /// </summary>
    public class MultiMutexRequest
    {
        private int _reportedCount;
        private List<NetMutex> _acquiredLocks;
        private List<NetMutex> _mutexList;
        private Action _onSuccess;
        private Action _onFailure;

        public MultiMutexRequest(IEnumerable<NetMutex> mutexes, Action successCallback = null, Action failureCallback = null)
        {
            _onSuccess = successCallback;
            _onFailure = failureCallback;
            _acquiredLocks = new List<NetMutex>();
            _mutexList = new List<NetMutex>(mutexes);
            RequestMutexes();
        }

        public void ReleaseLocks()
        {
            for (int index = 0; index < _acquiredLocks.Count; ++index)
                _acquiredLocks[index].ReleaseLock();
            _acquiredLocks.Clear();
        }

        private void RequestMutexes()
        {
            if (_mutexList == null)
            {
                if (_onFailure == null)
                    return;
                _onFailure();
            }
            else if (_mutexList.Count == 0)
            {
                if (_onSuccess == null)
                    return;
                _onSuccess();
            }
            else
            {
                for (int index = 0; index < _mutexList.Count; ++index)
                {
                    if (_mutexList[index].IsLocked())
                    {
                        if (_onFailure == null)
                            return;
                        _onFailure();
                        return;
                    }
                }
                for (int index = 0; index < _mutexList.Count; ++index)
                {
                    NetMutex mutex = _mutexList[index];
                    mutex.RequestLock((Action)(() => OnLockAcquired(mutex)), (Action)(() => OnLockFailed(mutex)));
                }
            }
        }

        private void OnLockAcquired(NetMutex mutex)
        {
            ++_reportedCount;
            _acquiredLocks.Add(mutex);
            if (_reportedCount < _mutexList.Count)
                return;
            FinalizeLocks();
        }

        private void OnLockFailed(NetMutex mutex)
        {
            ++_reportedCount;
            if (_reportedCount < _mutexList.Count)
                return;
            FinalizeLocks();
        }

        private void FinalizeLocks()
        {
            if (_acquiredLocks.Count < _mutexList.Count)
            {
                ReleaseLocks();
                _onFailure();
            }
            else
                _onSuccess();
        }
    }
}