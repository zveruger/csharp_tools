using System;
using System.Net.Sockets;
using System.Runtime.InteropServices;

namespace Tools.CSharp.Extensions
{
    public static class SocketExtensions
    {
        //---------------------------------------------------------------------
        /// <summary>
        /// Установка keep-alive для сокета.
        /// </summary>   
        /// <param name="socket">Сокет</param>
        /// <param name="enable">Включить\выключить keep-alive.</param>
        /// <param name="intervalBetweenAcknowledgementPackets">
        /// Определяет тайм-аут, в миллесекундах, между двумя подтвержденными пакетами keep-alive. 
        /// Игнорируется, если параметр <paramref name="enable"/> = false.</param>
        /// <param name="intervalBetweenNoAcknowledgementPackets">
        /// Опеределяет тайм-аут, в миллисекундах, между двумя не подтвержденными пакетами keep-alive. 
        /// Игнорируется, если параметр <paramref name="enable"/> = false.
        /// Количество попыток определяется параметром TcpMaxDataRetransmissions в системе Windows и по умолчанию равен 5. 
        /// Более подробную информацию о TcpMaxDataRetransmissions см. <see cref="https://support.microsoft.com/en-us/kb/170359?wa=wsignin1.0"/>.
        /// </param>
        /// <exception cref="ArgumentNullException">Если socket недействительный, т.е <paramref name="socket"/> = null.</exception>
        /// <exception cref="SocketException">Если не удалось включить\выключить keep-alive.</exception>
        public static void SetTcpKeepAlive(this Socket socket, bool enable, int intervalBetweenAcknowledgementPackets = 720000, int intervalBetweenNoAcknowledgementPackets = 1000)
        {
            if (socket == null)
            { throw new ArgumentNullException(nameof(socket)); }

            /*  
                Аргументы структуры для SIO_KEEPALIVE_VALS (см. файл MSTcpIP.h в Microsoft SDK)
                struct tcp_keepalive
                {
                    ULONG onoff;
                    ULONG keepalivetime;
                    ULONG keepaliveinterval;
                };
                ULONG - 4 байта.
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

            var bytesPerLong = Marshal.SizeOf(typeof(int)); // 4 байта
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