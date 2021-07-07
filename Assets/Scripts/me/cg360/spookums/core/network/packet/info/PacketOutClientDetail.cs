using System;
using me.cg360.spookums.utility;

namespace me.cg360.spookums.core.network.packet.info
{
    // 1 byte: OS Platform
    // 1 byte: Is 64-bit?
    // Small String: OS Version String
    // Small String: Client Version String
    public class PacketOutClientDetail : NetworkPacket
    {
        protected override byte GetPacketTypeID()
        {
            return VanillaProtocol.PACKET_SERVER_DETAIL;
        }

        protected override ushort EncodeBody()
        {
            Body.Reset();
            ushort packetSize = 0;

            OperatingSystem os = Environment.OSVersion;
            
            switch (os.Platform)
            {
                // Modern Windows - ID 1+
                case PlatformID.Win32NT:
                    String verString = $"{os.Version.Major}.{os.Version.Minor}";
                    switch (verString)
                    {
                        // Windows XP
                        case "5.1":
                        case "5.2": // 64-bit
                            Body.Put(2);
                            break;
                        
                        // Windows Vista
                        case "6.0":
                            Body.Put(3);
                            break;
                        
                        // Windows 7
                        case "6.1":
                            Body.Put(4);
                            break;
                        
                        // Windows 8
                        case "6.2":
                        case "6.3": // 8.1
                            Body.Put(5);
                            break;
                        
                        // Windows 10 (Possibly Windows 11?)
                        case "10.0":
                            Body.Put(6);
                            break;
                            
                        case "11.0":
                            Body.Put(7);
                            break;
                        
                        //Unknown Windows NT
                        default:
                            Body.PutUnsignedByte(1);
                            break;
                    }
                    break;
                
                // ------------------------------
                
                // Unix Types - 20+
                case PlatformID.MacOSX:
                case PlatformID.Unix:
                    Body.PutUnsignedByte(10);
                    break;
                
                // ------------------------------
                
                // Old/Unsupported Versions - ID 100+, keep em out of the way
                case PlatformID.Win32Windows:
                case PlatformID.Win32S:
                case PlatformID.WinCE:
                    Body.PutUnsignedByte(100);
                    break;
                
                default:
                    Body.PutUnsignedByte(255); // Unknown
                    break;
            }

            Body.PutUnsignedByte((byte) (Environment.Is64BitOperatingSystem ? 1 : 0));
            packetSize += Body.PutSmallUTF8String(os.VersionString);
            packetSize += Body.PutSmallUTF8String(Constants.VERSION_STRING);

            packetSize += 2; // Accounts for: Platform, 64bit?
            
            return packetSize;
        }

        protected override void DecodeBody(ushort inboundSize)
        {
            throw new System.NotImplementedException();
        }
    }
}