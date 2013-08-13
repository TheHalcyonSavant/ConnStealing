using System;
using System.Collections.Generic;
using System.Text;

using PacketDotNet;
using SharpPcap;

namespace ConnStealing
{
    public abstract class WrapperBase
    {
        public string Msg;
    }

    public class PacketWrapper : WrapperBase
    {
        private IpPacket ipPacket;
        private RawCapture rawCapture;
        private TcpPacket tcpPacket;

        public int Count { get; protected set; }
        public string SrcIp { get { return ipPacket.SourceAddress.ToString(); } }
        public ushort SrcPort { get { return tcpPacket.SourcePort; } }
        public string DestIp { get { return ipPacket.DestinationAddress.ToString(); } }
        public ushort DestPort { get { return tcpPacket.DestinationPort; } }
        public string Protocol { get { return ipPacket.Protocol.ToString(); } }
        public string Length { get { return String.Format("{0}({1})", Msg.Length, tcpPacket.PayloadData.Length); } }
        public string Time
        {
            get
            {
                DateTime time = rawCapture.Timeval.Date;
                return String.Format("{0}:{1}:{2},{3}", time.Hour, time.Minute, time.Second, time.Millisecond);
            }
        }

        public PacketWrapper(TcpRecon tcpRecon)
        {
            Count = ++CaptureForm.packetCount;
            ipPacket = tcpRecon.IpPacket;
            rawCapture = tcpRecon.RawCapture;
            tcpPacket = tcpRecon.TcpPacket;
            Msg = tcpRecon.Msg.ToString();
        }
    }

    public class PacketWrapper2 : WrapperBase
    {
        public RawCapture rawPacket;
        public IpPacket ipPacket;
        public TcpPacket tcpPacket;

        public int Count { get; protected set; }
        public ushort MyPort { get; private set; }
        public string Destination { get; private set; }
        public string Flags { get; private set; }
        public uint Sequence { get; private set; }
        public uint Acknowledgment { get; private set; }
        public int Len { get; private set; }
        public ulong Time { get; private set; }

        public PacketWrapper2(RawCapture rawCapture)
        {
            rawPacket = rawCapture;
            Packet packet = Packet.ParsePacket(rawCapture.LinkLayerType, rawCapture.Data);
            ipPacket = IpPacket.GetEncapsulated(packet);
            tcpPacket = TcpPacket.GetEncapsulated(packet);

            Count = ++CaptureForm.packetCount;
            if (ipPacket.SourceAddress.Equals(CaptureForm.MyIp))
            {
                MyPort = tcpPacket.SourcePort;
                Destination = "sent";
            }
            else
            {
                MyPort = tcpPacket.DestinationPort;
                Destination = "received";
            }

            Flags = "[";
            if (tcpPacket.Syn) Flags += "SYN";
            else if (tcpPacket.Psh) Flags += "PSH";
            else if (tcpPacket.Fin) Flags += "FIN";
            if (tcpPacket.Ack)
            {
                if (Flags.Length > 1) Flags += ",";
                Flags += "ACK";
            }
            Flags += "]";

            Sequence = tcpPacket.SequenceNumber;
            Acknowledgment = tcpPacket.AcknowledgmentNumber;
            Len = tcpPacket.PayloadData.Length;
            Time = rawCapture.Timeval.MicroSeconds;
            Msg = Encoding.ASCII.GetString(tcpPacket.PayloadData);
        }
    }
}
