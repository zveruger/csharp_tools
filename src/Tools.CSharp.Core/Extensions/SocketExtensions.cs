using System;
using System.Net.Sockets;
using System.Runtime.InteropServices;

namespace Tools.CSharp.Extensions
{
    public static class SocketExtensions
    {
        //---------------------------------------------------------------------
        /// <summary>
        /// ��������� keep-alive ��� ������.
        /// </summary>   
        /// <param name="socket">�����</param>
        /// <param name="enable">��������\��������� keep-alive.</param>
        /// <param name="intervalBetweenAcknowledgementPackets">
        /// ���������� ����-���, � �������������, ����� ����� ��������������� �������� keep-alive. 
        /// ������������, ���� �������� <paramref name="enable"/> = false.</param>
        /// <param name="intervalBetweenNoAcknowledgementPackets">
        /// ����������� ����-���, � �������������, ����� ����� �� ��������������� �������� keep-alive. 
        /// ������������, ���� �������� <paramref name="enable"/> = false.
        /// ���������� ������� ������������ ���������� TcpMaxDataRetransmissions � ������� Windows � �� ��������� ����� 5. 
        /// ����� ��������� ���������� � TcpMaxDataRetransmissions ��. <see cref="https://support.microsoft.com/en-us/kb/170359?wa=wsignin1.0"/>.
        /// </param>
        /// <exception cref="ArgumentNullException">���� socket ����������������, �.� <paramref name="socket"/> = null.</exception>
        /// <exception cref="SocketException">���� �� ������� ��������\��������� keep-alive.</exception>
        public static void SetTcpKeepAlive(this Socket socket, bool enable, int intervalBetweenAcknowledgementPackets = 720000, int intervalBetweenNoAcknowledgementPackets = 1000)
        {
            if (socket == null)
            { throw new ArgumentNullException(nameof(socket)); }

            /*  
                ��������� ��������� ��� SIO_KEEPALIVE_VALS (��. ���� MSTcpIP.h � Microsoft SDK)
                struct tcp_keepalive
                {
                    ULONG onoff;
                    ULONG keepalivetime;
                    ULONG keepaliveinterval;
                };
                ULONG - 4 �����.
            */

            var tcpKeepAliveTime = 0;
            var tcpKeepAliveInterval = 0;

            if (enable)
            {
                if (intervalBetweenAcknowledgementPackets < 0)
                { throw new ArgumentOutOfRangeException(nameof(intervalBetweenAcknowledgementPackets)); }
                if (intervalBetweenNoAcknowledgementPackets < 0)
                { throw new ArgumentOutOfRangeException(nameof(intervalBetweenNoAcknowledgementPackets)); }

                tcpKeepAliveTime = intervalBetweenAcknowledgementPackets;
                tcpKeepAliveInterval = intervalBetweenNoAcknowledgementPackets;
            }

            var bytesPerLong = Marshal.SizeOf(typeof(int)); // 4 �����
            var optionInValue = new byte[bytesPerLong * 3]; //size = bytesPerLong * 3 = 12
            var onoff = enable ? 1 : 0;

            BitConverter.GetBytes(onoff).CopyTo(optionInValue, 0);
            BitConverter.GetBytes(tcpKeepAliveTime).CopyTo(optionInValue, bytesPerLong); 
            BitConverter.GetBytes(tcpKeepAliveInterval).CopyTo(optionInValue, bytesPerLong * 2);

            //socket.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.KeepAlive, true);
            socket.IOControl(IOControlCode.KeepAliveValues, optionInValue, null);
        }
        //---------------------------------------------------------------------
    }
}