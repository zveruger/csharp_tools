using System;
using System.Diagnostics;
using System.Management;
using System.Text;
using System.Windows.Forms;

namespace Tools.CSharp
{
    public abstract class BaseDeviceInformation
    {
        #region private
        private const int _ConvertMemoryBytesToMbConst = 1 << 20;
        private const int _ConvertMemoryKbtoMbConst = 1 << 10;
        //---------------------------------------------------------------------
        private static void _AddSeparator(StringBuilder str)
        {
            if (str.Length != 0)
            { str.AppendLine(); }
        }
        //---------------------------------------------------------------------
        private void _addProcessInformation(StringBuilder str)
        {
            str.AppendLine(GetProcessInformationTitle());

            var process = Process.GetCurrentProcess();
            str.AppendLine(GetProcessNameMessage(process.ProcessName));
            str.AppendLine(GetProcessMemoryInMBMessage(process.PagedMemorySize64 / _ConvertMemoryBytesToMbConst));
        }
        private void _addDeviceProcessorInformation(StringBuilder str)
        {
            str.AppendLine(GetProcessorsInformationTitle());
            using (var processorInformationCollection = new ManagementObjectSearcher(new ObjectQuery("SELECT * FROM Win32_Processor")).Get())
            {
                var processorInformationCounter = 0;
                foreach (var processorInformation in processorInformationCollection)
                {
                    if (processorInformationCounter != 0) { str.AppendLine(); }

                    str.AppendLine(GetProcessorNameMesage(processorInformation["Name"].ToString(), processorInformation["Description"].ToString()));

                    processorInformationCounter++;
                }
            }
        }
        private void _addDeviceMemoryInformation(StringBuilder str)
        {
            str.AppendLine(GetDeviceMemoreInformationTitle());
            using (var memoryInformationCollection = new ManagementObjectSearcher(new ObjectQuery("SELECT * FROM Win32_OperatingSystem")).Get())
            {
                var memoryInformationCounter = 0;
                foreach (var memoryInformation in memoryInformationCollection)
                {
                    if (memoryInformationCounter != 0) { str.AppendLine(); }

                    str.AppendLine(GetDeviceMemoryTotalVisibleSizeInMBMessage((ulong)memoryInformation["TotalVisibleMemorySize"] / _ConvertMemoryKbtoMbConst));
                    str.AppendLine(GetDeviceMemoryFreePhysicalSizeInMBMessage((ulong)memoryInformation["FreePhysicalMemory"] / _ConvertMemoryKbtoMbConst));
                    str.AppendLine(GetDeviceMemoryTotalVirtualSizeInMBMessage((ulong)memoryInformation["TotalVirtualMemorySize"] / _ConvertMemoryKbtoMbConst));
                    str.AppendLine(GetDeviceMemoryFreeVirtualSizeInMBMessage((ulong)memoryInformation["FreeVirtualMemory"] / _ConvertMemoryKbtoMbConst));

                    memoryInformationCounter++;
                }
            }
        }
        private void _addVideoInformation(StringBuilder str)
        {
            str.AppendLine(GetDeviceVideosInformationTitle());
            using (var videoInformationCollection = new ManagementObjectSearcher(new ObjectQuery("SELECT * FROM Win32_VideoController")).Get())
            {
                var videoInformationCounter = 0;
                foreach (var videoInformation in videoInformationCollection)
                {
                    if (videoInformationCounter != 0) { str.AppendLine(); }

                    str.AppendLine(GetDeviceVideoNameMessage(videoInformation["Name"].ToString()));
                    str.AppendLine(GetDeviceVideoProcessorNameMessage(videoInformation["VideoProcessor"].ToString()));
                    str.AppendLine(GetDeviceVideoDescriptionMessage(videoInformation["VideoModeDescription"].ToString()));

                    videoInformationCounter++;
                }
            }
        }
        private void _addDeviceScreenInformation(StringBuilder str)
        {
            str.AppendLine(GetDeviceScreensInformationTitle());

            var screens = Screen.AllScreens;
            for (var i = 0; i < screens.Length; i++)
            {
                if (i != 0) { str.AppendLine(); }

                var screen = screens[i];
                str.AppendLine(GetDeviceScreenNameMessage(screen.DeviceName));
                str.AppendLine(GetDeviceScreenPrimaryMessage(screen.Primary));
                str.AppendLine(GetDeviceScreenBoundMessage(screen.Bounds.Width, screen.Bounds.Height));
                str.AppendLine(GetDeviceScreenWorkingAreaMessage(screen.WorkingArea.Width, screen.WorkingArea.Height));
            }
        }
        private void _addOsInformation(StringBuilder str)
        {
            str.AppendLine(GetOSInformationTitle());
            using (var osInformationCollection = new ManagementObjectSearcher(new ObjectQuery("SELECT * FROM Win32_OperatingSystem")).Get())
            {
                var osInformationCouner = 0;
                foreach (var osInformation in osInformationCollection)
                {
                    if (osInformationCouner != 0) { str.AppendLine(); }

                    str.AppendLine(GetOSNameMessage(osInformation["Caption"].ToString(), osInformation["Version"].ToString()));
                    str.AppendLine(GetOSArchitectureMessage(osInformation["OSArchitecture"].ToString()));
                    str.AppendLine(GetOSLanguageMessage(osInformation["OSLanguage"].ToString()));
                    str.AppendLine(GetOSManufacturerMessage(osInformation["Manufacturer"].ToString()));

                    osInformationCouner++;
                }
            }
        }
        #endregion
        #region protected
        protected abstract string GetProcessInformationTitle();
        protected abstract string GetProcessNameMessage(string processName);
        protected abstract string GetProcessMemoryInMBMessage(long memorySize);
        //---------------------------------------------------------------------
        protected abstract string GetProcessorsInformationTitle();
        protected abstract string GetProcessorNameMesage(string name, string description);
        //---------------------------------------------------------------------
        protected abstract string GetDeviceMemoreInformationTitle();
        protected abstract string GetDeviceMemoryTotalVisibleSizeInMBMessage(ulong memorySize);
        protected abstract string GetDeviceMemoryFreePhysicalSizeInMBMessage(ulong memorySize);
        protected abstract string GetDeviceMemoryTotalVirtualSizeInMBMessage(ulong memorySize);
        protected abstract string GetDeviceMemoryFreeVirtualSizeInMBMessage(ulong memorySize);
        //---------------------------------------------------------------------
        protected abstract string GetDeviceVideosInformationTitle();
        protected abstract string GetDeviceVideoNameMessage(string name);
        protected abstract string GetDeviceVideoProcessorNameMessage(string name);
        protected abstract string GetDeviceVideoDescriptionMessage(string description);
        //---------------------------------------------------------------------
        protected abstract string GetDeviceScreensInformationTitle();
        protected abstract string GetDeviceScreenNameMessage(string name);
        protected abstract string GetDeviceScreenPrimaryMessage(bool primary);
        protected abstract string GetDeviceScreenBoundMessage(int width, int height);
        protected abstract string GetDeviceScreenWorkingAreaMessage(int width, int heigth);
        //---------------------------------------------------------------------
        protected abstract string GetOSInformationTitle();
        protected abstract string GetOSNameMessage(string name, string version);
        protected abstract string GetOSArchitectureMessage(string architecture);
        protected abstract string GetOSLanguageMessage(string language);
        protected abstract string GetOSManufacturerMessage(string manufacturer);
        #endregion
        //---------------------------------------------------------------------
        public string GetInformation(bool processInformation = true)
        {
            var str = new StringBuilder();

            if (processInformation)
            { _addProcessInformation(str); }

            _AddSeparator(str);
            _addDeviceProcessorInformation(str);

            _AddSeparator(str);
            _addDeviceMemoryInformation(str);

            _AddSeparator(str);
            _addVideoInformation(str);

            _AddSeparator(str);
            _addDeviceScreenInformation(str);

            _AddSeparator(str);
            _addOsInformation(str);

            return str.ToString();
        }
        //---------------------------------------------------------------------
    }
}