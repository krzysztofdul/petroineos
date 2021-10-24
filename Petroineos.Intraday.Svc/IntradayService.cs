using System.ServiceProcess;
using System.Threading;

namespace Petroineos.Intraday.Svc
{
    public partial class IntradayService : ServiceBase
    {
        private readonly IReportSchedulerService _service;

        public IntradayService(IReportSchedulerService service)
        {
            _service = service;
            ServiceName = "Petroineos.Intraday.Svc";
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            _service.StartAsync(CancellationToken.None);
        }

        protected override void OnStop()
        {
            _service.StopAsync(CancellationToken.None);
        }
    }
}
