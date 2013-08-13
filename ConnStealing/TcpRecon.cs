using System;
using System.Text;
using PacketDotNet;
using SharpPcap;

namespace ConnStealing
{
    /// <summary>
    /// A class that represent a node in a linked list that holds partial Tcp session
    /// fragments
    /// </summary>
    internal class tcp_frag
    {
        public ulong seq = 0;
        public ulong len = 0;
        public ulong data_len = 0;
        public byte[] data = null;
        public tcp_frag next = null;
    };

    /// <summary>
    /// here we are going to try and reconstruct the data portion of a TCP session.
    /// We will try and handle duplicates, TCP fragments, and out of order packets in a smart way.
    /// 
    /// Translated from the file follow.c from WireShark source code
    /// the code can be found at: http://www.wireshark.org/download.html
    /// </summary>
    public class TcpRecon
    {
        private bool closed = false;
        private bool empty_tcp_stream = true;
        private bool incomplete_tcp_stream = false;
        private long[] src_addr = new long[2];
        private uint[] src_port = new uint[2];
        private uint[] bytes_written = new uint[2];
        private ulong[] seq = new ulong[2];             // holds the last sequence number for each direction
        private CaptureForm captureForm;
        private tcp_frag[] frags = new tcp_frag[2];     // holds two linked list of the session data, one for each direction

        public bool IncompleteStream { get { return incomplete_tcp_stream; } }
        public bool EmptyStream { get { return empty_tcp_stream; } }
        public IpPacket IpPacket;
        public RawCapture RawCapture;
        public StringBuilder Msg = new StringBuilder();
        public TcpPacket TcpPacket;

        public TcpRecon(CaptureForm capForm)
        {
            reset_tcp_reassembly();
            this.captureForm = capForm;
        }

        /// <summary>
        /// Cleans up the class and frees resources
        /// </summary>
        public void Close()
        {
            if (!closed)
            {
                reset_tcp_reassembly();
                Msg.Clear();
                closed = true;
            }
        }

        ~TcpRecon()
        {
            Close();
        }

        /// <summary>
        /// The main function of the class receives a tcp packet and reconstructs the stream
        /// </summary>
        /// <param name="tcpPacket"></param>
        public void ReassemblePacket(RawCapture rawCapture)
        {
            this.RawCapture = rawCapture;
            Packet packet = Packet.ParsePacket(rawCapture.LinkLayerType, rawCapture.Data);
            this.IpPacket = IpPacket.GetEncapsulated(packet);
            this.TcpPacket = TcpPacket.GetEncapsulated(packet);
            // if the paylod length is zero bail out
            //ulong length = (ulong)(tcpPacket.TCPPacketByteLength - tcpPacket.TCPHeaderLength);
            ulong length = (ulong)(this.TcpPacket.Bytes.Length - this.TcpPacket.Header.Length);
            if (length == 0) return;
            reassemble_tcp(length);
        }

        /// <summary>
        /// Writes the payload data to the file
        /// </summary>
        /// <param name="index"></param>
        /// <param name="data"></param>
        private void write_packet_data(int index, byte[] data)
        {
            if (data.Length == 0) return;       // ignore empty packets

            foreach (byte b in data)
            {
                if (b == 0 && this.Msg.Length > 0)
                {
                    PacketWrapper pw = new PacketWrapper(this);
                    captureForm.BeginInvoke(new System.Windows.Forms.MethodInvoker(delegate
                    {
                        captureForm.bs.Add(pw);
                    }));
                    this.Msg = new StringBuilder();
                }
                else this.Msg.Append((char)b);
            }
            bytes_written[index] += (uint)data.Length;
            empty_tcp_stream = false;
        }

        /// <summary>
        /// Reconstructs the tcp session
        /// </summary>
        /// <param name="length">The size of the original packet data</param>
        private void reassemble_tcp(ulong length)
        {
            bool first = false;
            bool synflag = this.TcpPacket.Syn;
            byte[] data = this.TcpPacket.PayloadData;
            int src_index, j;
            long srcx = BitConverter.ToInt32(this.IpPacket.SourceAddress.GetAddressBytes(), 0);
            uint srcport = (uint)this.TcpPacket.SourcePort;
            ulong data_length = (ulong)this.TcpPacket.PayloadData.Length;
            ulong newseq;
            ulong sequence = (ulong)this.TcpPacket.SequenceNumber;
            tcp_frag tmp_frag;

            src_index = -1;

            // Now check if the packet is for this connection.

            // Check to see if we have seen this source IP and port before.
            // (Yes, we have to check both source IP and port; the connection might be between two different ports on the same machine.)
            for (j = 0; j < 2; j++)
            {
                if (src_addr[j] == srcx && src_port[j] == srcport)
                    src_index = j;
            }
            // we didn't find it if src_index == -1
            if (src_index < 0)
            {
                // assign it to a src_index and get going
                for (j = 0; j < 2; j++)
                {
                    if (src_port[j] == 0)
                    {
                        src_addr[j] = srcx;
                        src_port[j] = srcport;
                        src_index = j;
                        first = true;
                        break;
                    }
                }
            }
            if (src_index < 0) throw new Exception("ERROR in reassemble_tcp: Too many addresses!");
            if (data_length < length) incomplete_tcp_stream = true;

            // now that we have filed away the srcs, lets get the sequence number stuff figured out
            if (first)
            {
                // this is the first time we have seen this src's sequence number
                seq[src_index] = sequence + length;
                if (synflag) seq[src_index]++;
                // write out the packet data
                write_packet_data(src_index, data);
                return;
            }
            // if we are here, we have already seen this src, let's try and figure out if this packet is in the right place
            if (sequence < seq[src_index])
            {
                // this sequence number seems dated, but check the end to make sure it has no more info than we have already seen
                newseq = sequence + length;
                if (newseq > seq[src_index])
                {
                    ulong new_len;
                    // this one has more than we have seen. let's get the payload that we have not seen.
                    new_len = seq[src_index] - sequence;
                    if (data_length <= new_len)
                    {
                        data = null;
                        data_length = 0;
                        incomplete_tcp_stream = true;
                    }
                    else
                    {
                        data_length -= new_len;
                        byte[] tmpData = new byte[data_length];
                        for (ulong i = 0; i < data_length; i++)
                            tmpData[i] = data[i + new_len];
                        data = tmpData;
                    }
                    sequence = seq[src_index];
                    length = newseq - seq[src_index];
                    // this will now appear to be right on time :)
                }
            }
            if (sequence == seq[src_index])
            {
                // right on time
                seq[src_index] += length;
                if (synflag) seq[src_index]++;
                if (data != null) write_packet_data(src_index, data);
                // done with the packet, see if it caused a fragment to fit
                while (check_fragments(src_index)) ;
            }
            else
            {
                // out of order packet
                if (data_length > 0 && sequence > seq[src_index])
                {
                    tmp_frag = new tcp_frag();
                    tmp_frag.data = data;
                    tmp_frag.seq = sequence;
                    tmp_frag.len = length;
                    tmp_frag.data_len = data_length;
                    if (frags[src_index] != null) tmp_frag.next = frags[src_index];
                    else tmp_frag.next = null;
                    frags[src_index] = tmp_frag;
                }
            }
        } // end reassemble_tcp

        // here we search through all the frag we have collected to see if one fits
        bool check_fragments(int index)
        {
            tcp_frag prev = null;
            tcp_frag current;
            current = frags[index];
            while (current != null)
            {
                if (current.seq == seq[index])
                {
                    // this fragment fits the stream
                    if (current.data != null)
                        write_packet_data(index, current.data);
                    seq[index] += current.len;
                    if (prev != null) prev.next = current.next;
                    else frags[index] = current.next;
                    current.data = null;
                    current = null;
                    return true;
                }
                prev = current;
                current = current.next;
            }
            return false;
        }

        // cleans the linked list
        void reset_tcp_reassembly()
        {
            int i;
            tcp_frag current, next;

            empty_tcp_stream = true;
            incomplete_tcp_stream = false;
            for (i = 0; i < 2; i++)
            {
                seq[i] = 0;
                src_addr[i] = 0;
                src_port[i] = 0;
                //tcp_port[i] = 0;
                bytes_written[i] = 0;
                current = frags[i];
                while (current != null)
                {
                    next = current.next;
                    current.data = null;
                    current = null;
                    current = next;
                }
                frags[i] = null;
            }
        }
    }

}
