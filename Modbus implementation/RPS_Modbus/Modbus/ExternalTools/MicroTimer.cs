using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace External.MicroTimer
{
    public class MicroTimer
    {
        private bool _isRunning { get; set; } = false;

        // The stopwatch uses the system's high performance counter
        private readonly Stopwatch sw = new Stopwatch();

        // Stopwatch IsRunning used for a MicroTimer IsRunning
        public bool IsRunning { get { return _isRunning; } }

        // Store the timeout period in microseconds
        private int microSeconds = 100;

        // And the equivalent CPU ticks (1000000/100=10000)
        private long microSecondsInCPUTicks = Stopwatch.Frequency / 10000;

        // Accessors for the microseconds property and conversion to the CPU ticks equivalent
        public int MicroSeconds
        {
            get
            {
                return microSeconds;
            }
            set
            {
                microSeconds = value;
                microSecondsInCPUTicks = value * Stopwatch.Frequency / 1000000;
            }
        }

        public MicroTimer()
        {
            sw.Start();
        }

        // Stop the timer
        public void Stop()
        {
            _isRunning = false;
        }

        // Start the MicroTimer (async for new thread)
        public async void Start()
        {
            // Start if delegate set
            if (OnTimeout != null)
            {
                _isRunning = true;
                // CPU bound task, hence Task.Run
                await Task.Run(() => Timing());
            }
        }

        // Run the timer
        private void Timing()
        {
            // Calculates the timing difference
            long interval;
            // Loop until stopped
            while (_isRunning)
            {
                // Get interval to wait
                interval = sw.ElapsedTicks + microSecondsInCPUTicks;
                // Loop until interval has passed
                while (interval - sw.ElapsedTicks > 0)
                {
                    // Chance for kernel to yield the tight CPU loop
                    Thread.Sleep(0);
                }
                // Run the external code
                TriggerOnTimeout();
            }
        }

        // The delegate used to call an external function
        public delegate void ExternalCode(object sender);
        public event ExternalCode OnTimeout;
        protected virtual void TriggerOnTimeout() { OnTimeout?.Invoke(this); }
    }
}
