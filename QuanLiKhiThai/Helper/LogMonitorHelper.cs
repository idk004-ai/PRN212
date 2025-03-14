using QuanLiKhiThai.DAO;
using QuanLiKhiThai.DAO.Interface;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace QuanLiKhiThai.Helper;

public class LogMonitorHelper : INotifyPropertyChanged
{
    private CancellationTokenSource? _cancellationTokenSource;
    private Task? _monitoringTask;
    private readonly object _logsLock = new object();
    private ObservableCollection<Log> _recentLogs;
    private DateTime _lastCheckTime;
    private bool _isMonitoring;
    private int _refreshIntervalSeconds = 5;
    private readonly ILogDAO _logDAO;

    public event PropertyChangedEventHandler? PropertyChanged;

    public ObservableCollection<Log> RecentLogs
    {
        get { return _recentLogs; }
        set
        {
            _recentLogs = value;
            OnPropertyChanged(nameof(RecentLogs));
        }
    }

    public int RefreshIntervalSeconds
    {
        get { return _refreshIntervalSeconds; }
        set
        {
            _refreshIntervalSeconds = value;
            OnPropertyChanged(nameof(RefreshIntervalSeconds));
        }
    }

    public bool IsMonitoring
    {
        get { return _isMonitoring; }
        set
        {
            _isMonitoring = value;
            OnPropertyChanged(nameof(IsMonitoring));
        }
    }

    public LogMonitorHelper(ILogDAO logDAO)
    {
        _recentLogs = new ObservableCollection<Log>();
        _lastCheckTime = DateTime.Now.AddDays(-1);
        this._logDAO = logDAO;
    }

    public void StartMonitoring()
    {
        if (_monitoringTask != null && !_monitoringTask.IsCompleted)
        {
            return; // Already monitoring
        }

        IsMonitoring = true;
        _cancellationTokenSource = new CancellationTokenSource();

        _monitoringTask = Task.Run(async () =>
        {
            await Task.Delay(TimeSpan.FromSeconds(1), _cancellationTokenSource.Token); 
            while (!_cancellationTokenSource.Token.IsCancellationRequested)
            {
                try
                {
                    await RefreshLogsAsync(clearExistingLogs: false);
                    await Task.Delay(TimeSpan.FromSeconds(_refreshIntervalSeconds), _cancellationTokenSource.Token);
                }
                catch (OperationCanceledException)
                {
                    // Ignore
                    break;
                }
                catch (Exception ex)
                {
                    // Log error
                    System.Diagnostics.Debug.WriteLine($"Error in log monitoring: {ex.Message}");
                }
            }
            IsMonitoring = false;
        }, _cancellationTokenSource.Token);
    }

    public void StopMonitoring()
    {
        if (_cancellationTokenSource != null)
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource = null;
        }
    }

    public void SetTimeFilter(DateTime fromDate)
    {
        lock (_logsLock)
        {
            _lastCheckTime = fromDate;
        }
    }

    public async Task RefreshLogsAsync(bool clearExistingLogs = false)
    {
        List<Log> newLogs = await Task.Run(() => _logDAO.GetLogSince(_lastCheckTime).ToList());

        if (newLogs.Count > 0)
        {
            DateTime lastestLogTime = newLogs.Max(log => log.Timestamp ?? DateTime.MinValue);
            if (lastestLogTime > _lastCheckTime)
            {
                _lastCheckTime = lastestLogTime;
            }

            await System.Windows.Application.Current.Dispatcher.InvokeAsync(() =>
            {
                lock (_logsLock)
                {
                    if (clearExistingLogs)
                    {
                        _recentLogs.Clear();
                    }
                    HashSet<int> existingLogIds = new HashSet<int>(_recentLogs.Select(log => log.LogId));
                    foreach (var log in newLogs)
                    {
                        if (!existingLogIds.Contains(log.LogId))
                        {
                            _recentLogs.Insert(0, log);
                            existingLogIds.Add(log.LogId);
                        }
                    }

                    const int maxLogsToDisplay = 1000;
                    while (_recentLogs.Count > maxLogsToDisplay)
                    {
                        _recentLogs.RemoveAt(_recentLogs.Count - 1);
                    }
                }
            });
        }
    }

    public void ClearLogs()
    {
        lock (_logsLock)
        {
            _recentLogs.Clear();
        }
    }

    private void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
